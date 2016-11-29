using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace CoreEditor.Utils
{
	public static class WhereUsed
	{
		[MenuItem( "Assets/Find Reference(s)", true, 21 ),MenuItem( "Assets/Safe Delete %#\b", true, 20 )]
		public static bool IsSelectionAssets()
		{
			if ( Selection.objects.Length == 0 )
				return false;

			foreach ( var selectedObject in Selection.objects )
			{
				if ( !EditorUtility.IsPersistent( selectedObject ) )
					return false;
			}
			return true;
		}

		[MenuItem( "Assets/Find Reference(s)", false, 21 )]
		public static void Print()
		{
			PrintReferences( Selection.objects );
		}

		public static void PrintReferences( params Object[] objects )
		{
			var references = new HashSet<string>();
			var directReferences = new HashSet<string>();
			var indirectReferences = new HashSet<string>();

			var sb = new StringBuilder();

			sb.AppendLine( "[ Selected File(s) ]" );
			foreach ( var selectedObject in objects )
			{
				sb.AppendLine( AssetDatabase.GetAssetPath( selectedObject ) );
				references.UnionWith( ExtractDirectAndIndirect( FindReferences( selectedObject ), ref directReferences, ref indirectReferences ) );
			}

			sb.AppendLine();

			if ( references.Count == 0 )
				sb.AppendLine( "No reference" );
			else
			{
				AppendReferencesToStringBuilder( directReferences, indirectReferences, ref sb );
			}
			Log( sb.ToString() );
			EditorUtility.ClearProgressBar();
		}

		[MenuItem( "Edit/Find Unreferenced Asset(s)", false, 0 )]
		public static void FindUnreferencedAssets()
		{
			Log( GetUnreferencedAssets().ToSortedLines() );
		}

		[MenuItem( "Edit/Select Unreferenced Asset(s)", false, 1 )]
		public static void SelectUnreferencedAssets()
		{
			var assets = new List<Object>();
			foreach ( var asset in GetUnreferencedAssets() )
				assets.Add( AssetDatabase.LoadAssetAtPath( asset, typeof( Object ) ) );
			Selection.objects = assets.ToArray();
		}

		static HashSet<string> GetUnreferencedAssets()
		{
			var referencesOutsideResources = new HashSet<string>();
			var otherAssets = new HashSet<string>();

			var ignoreExtensions = new HashSet<string>( new [] {
				".asset",
				".prefs" ,
				".cs",
				".boo",
				".js",
				".rsp",
				".m",
				".mm",
				".py",
				".h",
				".plist",
				".a",
				".dll",
				".pyc",
				".scpt",
				".userprefs",
				".jar" ,
				".json",
				".zip",
				".xib",
				".c",
				".o" ,
				".patch",
				".icns",
				""
			} );

			var assets = AssetDatabase.GetAllAssetPaths().Where( file => !ignoreExtensions.Contains( Path.GetExtension( file ).ToLower() ) ).ToArray();
			int total = assets.Length;
			float invTotal = 1.0f / total;
			int count = 0;
			var totalString = " / " + total;

			foreach ( var asset in assets )
			{
				if ( count % 20 == 0 && EditorUtility.DisplayCancelableProgressBar( "Find Unreferenced Asset(s)", "Processing... " + count + totalString, ( count * invTotal ) ) )
					break;
				if ( IsResourceOrScene( asset ) )
				{
					var dependencies = AssetDatabase.GetDependencies( new []{ asset } )
										.Where( dependency => !dependency.StartsWith( "Assets/Resources" ) && dependency != asset );
					referencesOutsideResources.UnionWith( dependencies );
				}
				else
				{
					otherAssets.Add( asset );
				}
			}

			otherAssets.ExceptWith( referencesOutsideResources );
			otherAssets.ExceptWith( LibraryReferences() );
			EditorUtility.ClearProgressBar();

			return otherAssets;
		}

		static void AppendReferencesToStringBuilder( HashSet<string> directReferences, HashSet<string> indirectReferences, ref StringBuilder sb )
		{
			sb.AppendLine( "[ Direct Reference(s) ] " );
			sb.AppendLine( directReferences.ToSortedLines() );

			if ( indirectReferences.Count > 0 )
			{
				sb.AppendLine( "[ Indirect Reference(s) or Rare Assets that have both direct AND indirect reference on the selected asset(s) ]" );
				sb.AppendLine( indirectReferences.ToSortedLines() );
			}
		}

		static HashSet<string> ExtractDirectAndIndirect( HashSet<string> references, ref HashSet<string> directReferences, ref HashSet<string> indirectReferences )
		{
			int total = references.Count;
			float invTotal = 1.0f / total;
			int count = 0;
			var totalString = " / " + total;
			foreach ( var reference in references )
			{
				count++;
				if ( count % 20 == 0 && EditorUtility.DisplayCancelableProgressBar( "Find Reference(s)", "Splitting direct and indirect reference(s)" + count + totalString, ( count * invTotal ) ) )
					break;
				if ( ( references.Intersect( AssetDatabase.GetDependencies( new string[ ]{ reference } ) ).Count() <= 1 ) )
					directReferences.Add( reference );
				else
					indirectReferences.Add( reference );
			}
			return references;
		}

		static HashSet<string> ExtractSelfAndParents( string path )
		{
			var selfAndParents = new HashSet<string>();
			var parent = path;
			while ( parent != "Assets" )
			{
				selfAndParents.Add( parent );
				parent = Path.GetDirectoryName( parent );
			}
			;
			return selfAndParents;
		}

		static HashSet<string> FindReferences( Object obj )
		{
			var path = AssetDatabase.GetAssetPath( obj );
			var selfAndParents = ExtractSelfAndParents( path );

			var pathAsDir = path;
			bool isDir = Directory.Exists( "./" + path );
			if ( isDir )
				pathAsDir += "/";

			var referencedBy = new HashSet<string>();
			var assets = AssetDatabase.GetAllAssetPaths().Where( file => !file.StartsWith( pathAsDir ) && file != path ).ToArray();

			var total = assets.Length;
			var invTotal = 1f / total;
			var count = 0;
			var totalString = " / " + total;
			foreach ( var asset in assets )
			{
				count++;
				if ( count % 20 == 0 && EditorUtility.DisplayCancelableProgressBar( "Find Reference(s)", "Looking for reference(s) " + count + totalString, ( count * invTotal ) ) )
					break;
				var dependencies = AssetDatabase.GetDependencies( new string[ ]{ asset } );
				Func<string,bool> referenceSelfParentOrChildren = (dep ) => ( ( selfAndParents.Contains( dep ) && dep != asset ) || ( isDir && dep.StartsWith( pathAsDir ) ) );
				if ( dependencies.FirstOrDefault( referenceSelfParentOrChildren ) != null )
					referencedBy.Add( asset );
			}

			referencedBy.UnionWith( LibraryReferences( obj ) );

			return referencedBy;
		}

		static HashSet<string> LibraryReferences( Object obj )
		{
			const string libraryPath = "ProjectSettings/ProjectSettings.asset";
			var foundLibraryReferences = new HashSet<string>();
			var type = typeof( PlayerSettings );

			foreach ( var property in type.GetProperties( BindingFlags.Static | BindingFlags.Public ) )
			{
				if ( property.PropertyType == typeof( Object ) || property.PropertyType.IsSubclassOf( typeof( Object ) ) )
				{
					var val = type.InvokeMember( property.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, property.PropertyType, null ) as Object;
					if ( val == obj )
					{
						foundLibraryReferences.Add( libraryPath + "/" + type.Name + "/" + property.Name );
					}
				}
			}

			foreach ( var nestedClass in type.GetNestedTypes( BindingFlags.Static | BindingFlags.Public ) )
			{
				foreach ( var property in nestedClass.GetProperties( BindingFlags.Static | BindingFlags.Public ) )
				{
					if ( property.PropertyType == typeof( Object ) || property.PropertyType.IsSubclassOf( typeof( Object ) ) )
					{
						var val = nestedClass.InvokeMember( property.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, property.PropertyType, null ) as Object;
						if ( val == obj )
						{
							foundLibraryReferences.Add( libraryPath + "/" + type.Name + "/" + nestedClass.Name + "/" + property.Name );
						}
					}
				}
			}

			if ( obj is Texture2D )
			{
				foreach ( var target in EnumUtil.GetValues<BuildTargetGroup>() )
				{
					foreach ( var icon in  PlayerSettings.GetIconsForTargetGroup( target ) )
					{
						if ( obj == icon )
						{
							foundLibraryReferences.Add( libraryPath + "/" + target + "/Icons" );
						}
					}
				}
			}

			var editor = PlayerSettingsEditor;
			foreach ( var field in editor.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance ) )
			{
				if ( field.FieldType == typeof( SerializedProperty ) )
				{
					var prop = field.GetValue( editor ) as SerializedProperty;
					if ( prop != null )
					{
						if ( prop.type.StartsWith( "PPtr" ) && prop.objectReferenceValue == obj )
						{
							foundLibraryReferences.Add( libraryPath + "/PlayerSettings/" + field.Name.Replace( "m_", "" ) );
						}
					}
				}
			}
			Object.DestroyImmediate( editor );

			return foundLibraryReferences;
		}

		static Editor PlayerSettingsEditor
		{
			get {
				var assembly = AppDomain.CurrentDomain.GetAssemblies().First( a => a.GetName().Name == "UnityEditor" );
				var playerSettingsType = assembly.GetType( "UnityEditor.Unsupported" );
				var playerSettings = playerSettingsType.InvokeMember( "GetSerializedAssetInterfaceSingleton", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, typeof( Object ), new object[]{ "PlayerSettings" } ) as Object;
				var activeEditorTrackerType = assembly.GetType( "UnityEditor.ActiveEditorTracker" );
				return activeEditorTrackerType.InvokeMember( "MakeCustomEditor", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, typeof( Editor ), new object[]{ playerSettings } ) as Editor;
			}
		}

		static HashSet<string> LibraryReferences()
		{
			var libraryReferences = new HashSet<string>();
			var type = typeof( PlayerSettings );

			foreach ( var property in type.GetProperties( BindingFlags.Static | BindingFlags.Public ) )
			{
				if ( property.PropertyType == typeof( Object ) || property.PropertyType.IsSubclassOf( typeof( Object ) ) )
				{
					var val = type.InvokeMember( property.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, property.PropertyType, null ) as Object;
					if ( val != null )
						libraryReferences.Add( AssetDatabase.GetAssetPath( val ) );
				}
			}

			foreach ( var nestedClass in type.GetNestedTypes( BindingFlags.Static | BindingFlags.Public ) )
			{
				foreach ( var property in nestedClass.GetProperties( BindingFlags.Static | BindingFlags.Public ) )
				{
					if ( property.PropertyType == typeof( Object ) || property.PropertyType.IsSubclassOf( typeof( Object ) ) )
					{
						var val = nestedClass.InvokeMember( property.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, property.PropertyType, null ) as Object;
						if ( val != null )
							libraryReferences.Add( AssetDatabase.GetAssetPath( val ) );
					}
				}
			}

			foreach ( var target in EnumUtil.GetValues<BuildTargetGroup>() )
			{
				foreach ( var icon in PlayerSettings.GetIconsForTargetGroup( target ) )
				{
					if ( icon != null )
						libraryReferences.Add( AssetDatabase.GetAssetPath( icon ) );
				}
			}

			var editor = PlayerSettingsEditor;
			foreach ( var field in editor.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance ) )
			{
				if ( field.FieldType == typeof( SerializedProperty ) )
				{
					var prop = field.GetValue( editor ) as SerializedProperty;
					if ( prop != null )
					{
						if ( prop.type.StartsWith( "PPtr" ) && prop.objectReferenceValue != null )
							libraryReferences.Add( AssetDatabase.GetAssetPath( prop.objectReferenceValue ) );
					}
				}
			}
			Object.DestroyImmediate( editor );

			return libraryReferences;
		}

		static void Log( string content, bool error = false )
		{
			Action<string> log;
			if ( error )
				log = Debug.LogError;
			else
				log = Debug.Log;

			const int unityLogMaxSize = 16092;

			if ( content.Length > unityLogMaxSize )
			{
				var filepath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FileUtil.GetUniqueTempPathInProject() + ".txt";

				if ( string.IsNullOrEmpty( filepath ) == false )
				{
					File.WriteAllText( filepath, content );
	
					Process.Start( filepath );
					content = "Content is too big for the Console. See full content in the opened file: \n" + filepath + "\n...\n\n" + content;
					log( content );
				}
			}
			else
			{
				log( content );
			}
		}

		static bool IsResourceOrScene(string path)
		{
			return path.StartsWith( "Assets/Resources" ) || path.EndsWith( ".unity" );
		}
	}

	static class EnumUtil
	{
		public static IEnumerable<T> GetValues<T>()
		{
			return Enum.GetValues( typeof( T ) ).Cast<T>();
		}
	}

	static class HashsetExtensions
	{
		public static string ToSortedLines( this HashSet<string> references )
		{
			var sortedReferences = references.ToList();
			sortedReferences.Sort();
			var sb = new StringBuilder();
			foreach ( var depends in sortedReferences )
				sb.AppendLine( depends );
			return sb.ToString();
		}
	}
}