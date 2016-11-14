using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Serializable = System.SerializableAttribute;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Debug = UnityEngine.Debug;

namespace CoreEditor.AssemblyPacker
{
	public class Packer : ScriptableObject
	{
		[SerializeField]
		Packer[] m_Dependencies;
		
		[SerializeField]
		PackerAssembly[] m_Assemblies;
		
		public Object[] m_ExtraPackageObjects;
		
		string m_PreviousLocation;
		
		List<string> m_ExtraGeneratedFiles = new List<string>(); 

		class AssemblyGenerationManifest
		{
			Dictionary<string,string> mGeneratedAssemblies = new Dictionary<string, string>();
			
			public List<string> GeneratedAssemblies
			{
				get
				{
					var assemblies = new List<string>();
					foreach ( var path in mGeneratedAssemblies.Keys )
						assemblies.Add( path );
					return assemblies;
				}
			}
			
			public void Prepare( string path )
			{
				if ( File.Exists( path ) )
				{
					if ( IsDummyAssembly( path ) )
					{
						var tempFolderPath = FileUtil.GetUniqueTempPathInProject() + "/";
						Directory.CreateDirectory( tempFolderPath );
						var tempDllPath = tempFolderPath + Path.GetFileName( path );
						
						File.Move( path, tempDllPath );
						if ( File.Exists( path + ".mdb" ))
							File.Move( path + ".mdb", tempDllPath + ".mdb" );
						
						mGeneratedAssemblies.Add( path, tempDllPath );
					}
					else
					{
						File.Delete( path );
						File.Delete( path + ".mdb" );
						mGeneratedAssemblies.Add( path, string.Empty );
					}
				}
				else
				{
					mGeneratedAssemblies.Add( path, string.Empty );
				}
			}
			
			public void Restore()
			{
				foreach( var assembly in mGeneratedAssemblies )
				{
					if ( File.Exists( assembly.Value ) )
					{
						File.Copy( assembly.Value, assembly.Key, true );
						if ( File.Exists( assembly.Value+".mdb" ))
							File.Copy( assembly.Value+".mdb", assembly.Key + ".mdb", true );
						Directory.Delete( Path.GetDirectoryName( assembly.Value ), true );
					}
				}
			}
		}

		public void PackAssembly ()
		{
			try 
			{
				EditorApplication.LockReloadAssemblies();
				EditorUtility.DisplayProgressBar( "Assembly Packer", "Building Assemblies", 0f );
				var manifest = CreateAssemblies();
				EditorUtility.DisplayProgressBar( "Assembly Packer", "Building Package", 0.5f );
				CreatePackage( manifest.GeneratedAssemblies.ToArray() );
				manifest.Restore();
				GenerateDummyAssemblyData();

				m_ExtraGeneratedFiles.Clear();
			}
			catch ( System.Exception e )
			{
				Debug.LogException( e, this );
			}
			finally
			{
				m_ExtraGeneratedFiles.Clear();
				EditorUtility.ClearProgressBar();
				EditorApplication.UnlockReloadAssemblies();
			}
		}
		
		List<PackerAssembly> AssemblyDependencies
		{
			get{
				var usedAssemblies = new HashSet<PackerAssembly>();
				var assemblyDependencies = new List<PackerAssembly>();
				foreach( var dependency in m_Dependencies )
				{
					var assemblies = new List<PackerAssembly>();
					assemblies.AddRange( dependency.AssemblyDependencies );
					assemblies.AddRange( dependency.m_Assemblies );
					foreach( var assembly in assemblies )
					{
						if ( usedAssemblies.Contains( assembly ) == false )
						{
							usedAssemblies.Add( assembly );
							assemblyDependencies.Add( assembly );
						}
					}
				}
				return assemblyDependencies;
			}
		}
		
		HashSet<Object> AssetsDependencies
		{
			get{
				var usedAssets = new HashSet<Object>();
				foreach( var dependency in m_Dependencies )
				{
					usedAssets.UnionWith( dependency.AssetsDependencies );
					usedAssets.UnionWith( dependency.m_ExtraPackageObjects );
				}
				return usedAssets;
			}
		}

		List<PackerAssembly> AllAssemblies
		{
			get
			{
				var assemblies = new List<PackerAssembly>();
				assemblies.AddRange( AssemblyDependencies );
				assemblies.AddRange( m_Assemblies );
				return assemblies.OrderBy( a=> a.name.Contains("Editor")).ToList();
			}
		}


		AssemblyGenerationManifest CreateAssemblies( )
		{
			var assemblyPaths = new List<string>();
			var assemblies = AllAssemblies;
			var manifest = new AssemblyGenerationManifest();
			foreach ( var assembly in assemblies )
			{
				var paths = new List<string>();
				foreach ( var sourceObject in assembly.m_Sources )
				{
					var path = AssetDatabase.GetAssetPath( sourceObject );
					if ( Directory.Exists( path ) )
						paths.AddRange( Directory.GetFiles( path, "*.cs", SearchOption.AllDirectories ) );
					else
						if ( Path.GetExtension( path ) == ".cs" )
							paths.Add( path );
					else
						Debug.LogError( string.Format( "Invalid source in Compiler: {0}", path ), this );
				}
				var sources = paths.ToArray();
				var references = new List<string>() {
					GetFrameWorksFolder() + "Managed/UnityEngine.dll",
					UnityEngineUIAssemblyPath,
				};
				
				bool isEditor = assembly.name.Contains("Editor");
				if ( isEditor )
					references.Add( GetFrameWorksFolder() + "Managed/UnityEditor.dll" );
				references.AddRange( assembly.m_AssemblyReferences.Select( o => AssetDatabase.GetAssetPath( o ) ) );
				references.AddRange( assemblyPaths );
				var defines = assembly.Defines;
				var dllPath = assembly.AssemblyPath;
				manifest.Prepare( dllPath );
				Directory.CreateDirectory( Path.GetDirectoryName( dllPath ) );
				var output = EditorUtility.CompileCSharp( sources, references.ToArray(), defines.ToArray(), dllPath );
				if ( OutputContainsError( output ) == false )
				{
					AssetDatabase.ImportAsset( dllPath );
					assemblyPaths.Add( dllPath );
					if ( assembly.m_ExportInlineDocumentation )
					{
						var docFile = dllPath.Replace( ".dll", ".xml" );
						GenerateDoc( docFile, dllPath, sources, references, defines );
						if ( File.Exists( docFile ) )
						{
							AssetDatabase.ImportAsset( docFile );
							m_ExtraGeneratedFiles.Add( docFile );
						}
					}
					
					if ( assembly.m_IsDebug )
					{
						var mdbFile = dllPath + ".mdb";
						AssetDatabase.ImportAsset( mdbFile );
						m_ExtraGeneratedFiles.Add( mdbFile );
					}
				}
				else
				{
					Debug.LogError( string.Join( "\n", output ), this );
				}
			}
			return manifest;
		}

		static bool OutputContainsError( string[] output )
		{
			foreach ( var line in output )
			{
				if ( line.Contains( "error CS" ) )
					return true;
			}
			return false;
		}
		
		static string UnityEngineUIAssemblyPath
		{
			get
			{
				return Assembly.GetAssembly( typeof( UnityEngine.UI.Image ) ).Location;
			}
		}
		
		void CreatePackage( params string[] extraFiles )
		{
			var paths = new HashSet<string>();
			var allObjects = AssetsDependencies;
			allObjects.UnionWith( m_ExtraPackageObjects );
			foreach( var obj in allObjects )
			{
				if ( obj != null )
					paths.Add( AssetDatabase.GetAssetPath( obj ) );
			}

			foreach ( var docFile in m_ExtraGeneratedFiles )
				paths.Add( docFile );
			
			paths.UnionWith( extraFiles );
			string file = EditorUtility.SaveFilePanel( "Package save location", Directory.GetCurrentDirectory(), name, "unityPackage" );
			
			if ( string.IsNullOrEmpty( file ) == false)
			{
				ExportPackageOptions flags = ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies;
				AssetDatabase.ExportPackage( paths.ToArray(), file, flags );
			}
		}
		
		public static string GetFrameWorksFolder()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return (Path.GetDirectoryName(EditorApplication.applicationPath) + "/Data/");
			}
			return (EditorApplication.applicationPath + "/Contents/Frameworks/");
		}
		
		public static string MonoFolder{
			get
			{
				return GetFrameWorksFolder() + "Mono/bin";
			}
		}
		void GenerateDoc( string docFile, string dllPath, string[] srcs , List<string> references, List<string> defines )
		{
			var sb = new StringBuilder();
			foreach( var src in srcs )
				sb.Append( string.Format( "\"{0}\" ", src ) );
			
			foreach( var reference in references )
				sb.Append( string.Format( "-r:\"{0}\" ", reference ) );
			foreach( var define in defines )
				sb.Append( string.Format( "-define:\"{0}\" ", define ) );
			
			var commandLineArgs = string.Format( "{0} -sdk:2 -t:library -out:{1} -doc:{2}", sb, "Temp/"+ Path.GetFileName( dllPath ),  docFile); 
			
			var proc = new Process();
			if (Application.platform == RuntimePlatform.WindowsEditor)
				proc.StartInfo.FileName = "\"" + MonoFolder + "/gmcs.bat\"";
			else
				proc.StartInfo.FileName = "mcs";
			proc.StartInfo.Arguments = commandLineArgs;
			proc.StartInfo.CreateNoWindow = true;
			proc.StartInfo.WorkingDirectory =  Directory.GetCurrentDirectory();
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.UseShellExecute = false;
			if (Application.platform == RuntimePlatform.WindowsEditor)
				proc.StartInfo.EnvironmentVariables["PATH"] = System.Environment.GetEnvironmentVariable("PATH")+";" + "\""+MonoFolder+"\"";
			else
				proc.StartInfo.EnvironmentVariables["PATH"] = System.Environment.GetEnvironmentVariable("PATH")+":" + MonoFolder;
			try
			{
				proc.Start();
				proc.WaitForExit();			
			}
			catch( System.Exception e )
			{
				Debug.LogException( e );
			}
		}
		
		public void GenerateDummyAssemblyData ()
		{
			var dummySrcPath = FileUtil.GetUniqueTempPathInProject() + ".cs";
			File.WriteAllText( dummySrcPath,"internal class Dummy{}");

			foreach ( var assembly in AllAssemblies )
			{
				var assemblyPath = assembly.AssemblyPath;
				Directory.CreateDirectory( Path.GetDirectoryName( assemblyPath ) );
				if ( File.Exists( assemblyPath ) == false || IsDummyAssembly( assemblyPath ) == false )
					EditorUtility.CompileCSharp( new string[]{dummySrcPath}, new string[]{}, assembly.Defines.ToArray(), assemblyPath );
				ImportAssetWithMeta( assemblyPath, assembly.m_AssemblyMeta );
				if ( assembly.m_IsDebug )
					ImportAssetWithMeta( assemblyPath + ".mdb", assembly.m_AssemblyMDBMeta );
				else
					AssetDatabase.DeleteAsset( assemblyPath + ".mdb" );

				var docPath = assemblyPath.Substring(0, assemblyPath.Length - 4 ) + ".xml";
				if ( assembly.m_ExportInlineDocumentation )
				{
					File.WriteAllText(docPath,string.Format( @"<?xml version=""1.0""?><doc><assembly><name>{0}</name></assembly><members></members></doc>"
					                                        , Path.GetFileNameWithoutExtension( docPath ) ) );
					ImportAssetWithMeta( docPath, assembly.m_DocumentationMeta );
				}
				else
				{
					AssetDatabase.DeleteAsset( docPath );
				}
			}
			File.Delete( dummySrcPath );
		}

		static void ImportAssetWithMeta( string path, PackerAssembly.Meta meta )
		{
			AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceSynchronousImport );

			if ( meta.IsGUIDSpecified )
			{
				var metaPath = path + ".meta";
				var currentMeta = File.ReadAllText( metaPath );
				var newMeta = Regex.Replace( currentMeta, @"guid:\s[0-9a-f]{32}", string.Format( @"guid: {0}", meta.GUID ) );
				if ( currentMeta != newMeta )
				{
					File.WriteAllText( metaPath, newMeta );
					AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceSynchronousImport );
				}
			}
		}

		static bool IsDummyAssembly( string path )
		{
			try
			{
				var assembly = Assembly.LoadFile( path );
				return assembly.GetType( "Dummy", false ) != null;
			}
			catch
			{
				return false;
			}
		}
	}
}
