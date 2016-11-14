using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System;

namespace CoreEditor.Utils
{
	[InitializeOnLoad]
	public static class ExternalTools2
	{
		static ExternalTools2()
		{
			if ( Application.platform == RuntimePlatform.WindowsEditor )
				Impl = new WindowsExternalTools();
			else if ( Application.platform == RuntimePlatform.OSXEditor )
				Impl = new OSXOpenExternalTools();
		}
		interface IExternalTools2
		{
			string EditorForFile( string path );
		}

		class WindowsExternalTools:IExternalTools2
		{
			public string EditorForFile( string path )
			{
				var ext = Path.GetExtension( path ).ToLower();
				string appValue = null;
				using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey
				       (  @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + ext + @"\UserChoice", false))
				{
					if ( key != null )
						appValue = (string)key.GetValue("Progid",string.Empty);
				}

				if ( string.IsNullOrEmpty( appValue ) )
				{
					string appKey = @"HKEY_CLASSES_ROOT\" + ext;
					appValue = (string)Microsoft.Win32.Registry.GetValue( appKey, "", string.Empty );
				}

				if ( string.IsNullOrEmpty( appValue ) )
				{
					return string.Empty;
				}

				var commandKey = @"HKEY_CLASSES_ROOT\" + appValue + @"\shell\open\command";
				var commandValue = Microsoft.Win32.Registry.GetValue( commandKey, string.Empty, null ).ToString();
				var editorExe = Regex.Match( commandValue, "(.*) .*$" ).Groups[1].Value;

				return editorExe;
			}
		}

		class OSXOpenExternalTools:IExternalTools2
		{
			public string EditorForFile( string path )
			{
				return "open";
			}
		}
		static readonly string[] SupportedExtensions = new string[]{

			".cs",		
			".js",		
			".boo",	
			".shader",			
			".cginc",			
			".txt",		
			".bytes",
			".xml",
		};

		const string SaveKey = "ExternalTools2";
		
		static IExternalTools2 Impl = null;

		[OnOpenAssetAttribute]
		static bool OnOpenAsset(int instanceID, int line) 
		{
			var file = AssetDatabase.GetAssetPath( instanceID );
			var ext = Path.GetExtension( file ).ToLower();
			if ( EditorPrefs.HasKey( SaveKey + ext ) )
			{
				var multiLineSyntax = EditorPrefs.GetString( SaveKey + ext, string.Empty );
				return OpenFileInTextEditor( file, line, multiLineSyntax );
			}

			return false;
		}

		static bool OpenFileInTextEditor( string path, int line, string multiLineSyntax )
		{
			try
			{
				var process = new Process();
				var fullPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + path.Replace( "/", Path.DirectorySeparatorChar.ToString() );

				var editorExecutablePath = Impl.EditorForFile( path );

				if ( string.IsNullOrEmpty( editorExecutablePath ) )
				{
					Debug.LogError( string.Format( "No application associated with \"{0}\" extension in the Operating System.\nUsing Unity's default application.", Path.GetExtension( path ) ) );
					return false;
				}
				process.StartInfo.FileName = editorExecutablePath;

				if ( line < 0 || string.IsNullOrEmpty( multiLineSyntax ) )
					process.StartInfo.Arguments = "\"" + fullPath + "\"";
				else
					process.StartInfo.Arguments = string.Format( multiLineSyntax, path, line );

				process.Start();
				return true;
			}
			catch( Exception e )
			{
				Debug.LogError( e );
				return false;
			}
		}

		[PreferenceItem( "External Tools 2" )]
		static void PreferencesGUI()
		{
			EditorGUILayout.LabelField( "Open files using default OS applications for these extensions:" );

			GUILayout.BeginHorizontal();
			GUI.skin.label.fontStyle = FontStyle.Bold;
			GUILayout.Space( 92 );
			GUILayout.Label( "Open at line formating. Ex: \"{0}\":{1}" );
			GUI.skin.label.fontStyle = FontStyle.Normal;

			GUILayout.EndHorizontal();

			for ( int i = 0; i < SupportedExtensions.Length ; ++i )
			{
				GUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				var ext = SupportedExtensions[i];
				var enabled = CompactToggle( ext, EditorPrefs.HasKey( SaveKey + ext ) );
				GUI.enabled = enabled;
				var lineArgument = EditorGUILayout.TextField( EditorPrefs.GetString( SaveKey + ext, string.Empty ) );
				GUI.enabled = true;

				if ( EditorGUI.EndChangeCheck() )
				{
					if ( enabled )
						EditorPrefs.SetString( SaveKey+ext, lineArgument );
					else
						EditorPrefs.DeleteKey( SaveKey+ext );
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}

			GUI.enabled = false;
			EditorGUILayout.LabelField( "*Settings are shared with all Unity projects using External Tools 2." );
		}

		static bool CompactToggle( string label, bool value )
		{
			var enabled = GUILayout.Toggle( value, string.Empty );
			GUILayout.Label( label, GUILayout.Width( 66 ) );
			return enabled;
		}

	}
}
