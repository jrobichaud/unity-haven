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
	public static class ExternalTools3
	{
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

		const string SaveKey = "ExternalTools3";
		const string SaveKeyLine = "ExternalTools3Line";

		[OnOpenAssetAttribute(-10)]
		static bool OnOpenAsset(int instanceID, int line) 
		{
			var file = AssetDatabase.GetAssetPath( instanceID );
			var ext = Path.GetExtension( file ).ToLower();
			if ( line<0 && EditorPrefs.HasKey( SaveKey + ext ) )
			{
				var commandLine = EditorPrefs.GetString( SaveKey + ext );
				return OpenFileInTextEditor( file, line, commandLine );
			}
			else if (line >= 0 && EditorPrefs.HasKey( SaveKeyLine + ext ) )
			{
				var commandLine = EditorPrefs.GetString( SaveKeyLine + ext );
				return OpenFileInTextEditor( file, line, commandLine );
			}

			return false;
		}

		
		static bool OpenFileInTextEditor( string path, int line, string commandLine )
		{
			string bin;
			string args;
			if ( SplitBinArgs( commandLine, out bin, out args ) )
			{
				try
				{
					var process = new Process();
					var fullPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + path.Replace( "/", Path.DirectorySeparatorChar.ToString() );
					
					process.StartInfo.FileName = bin;
					process.StartInfo.Arguments = string.Format( args, fullPath, line );
					process.Start();
				}
				catch( Exception e )
				{
					Debug.LogError( e );
					return false;
				}
				return true;
			}
			else
			{
				Debug.LogError( "Invalid command line: " + commandLine );
				return false;
			}
		}

		static bool SplitBinArgs( string input, out string bin, out string args )
		{
			var regex = new Regex( "(?:\"(?<bin>.*)\"|(?<bin>.+?))\\s(?<args>.+)" );

			var match = regex.Match(input);

			if ( match.Success)
			{
				bin = match.Groups["bin"].Value;
				args = match.Groups["args"].Value;
				return true;
			}
			else
			{
				bin = null;
				args = null;
				return false;
			}
		}

		[PreferenceItem( "External Tools 3" )]
		static void PreferencesGUI()
		{
			EditorGUILayout.LabelField( "Open files using given command lines:" );
			
			for ( int i = 0; i < SupportedExtensions.Length ; ++i )
			{
				GUILayout.BeginHorizontal();
				EditorGUI.BeginChangeCheck();
				var ext = SupportedExtensions[i];
				var enabled = CompactToggle( ext, EditorPrefs.HasKey( SaveKey + ext ) );
				GUI.enabled = enabled;
				var nolineArgument = EditorGUILayout.TextField( EditorPrefs.GetString( SaveKey + ext, string.Empty ) );
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Space( 96 );
				var lineArgument = EditorGUILayout.TextField( EditorPrefs.GetString( SaveKeyLine + ext, string.Empty ) );
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUI.enabled = true;

				if ( EditorGUI.EndChangeCheck() )
				{
					if ( enabled )
					{
						EditorPrefs.SetString( SaveKey+ext, nolineArgument );
						EditorPrefs.SetString( SaveKeyLine+ext, lineArgument );
					}
					else
					{
						EditorPrefs.DeleteKey( SaveKey+ext );
						EditorPrefs.DeleteKey( SaveKeyLine+ext );
					}
				}
			}

			GUI.enabled = false;
			
			GUI.skin.label.fontStyle = FontStyle.Bold;
			GUILayout.Label( "Format without line: \"full path\" \"{0}\"" );
			GUILayout.Label( "Format with line. Ex: \"full path\" \"{0}\" --line {1}" );
			GUI.skin.label.fontStyle = FontStyle.Normal;
			EditorGUILayout.LabelField( "*Settings are shared with all Unity projects using External Tools 3." );
		}

		static bool CompactToggle( string label, bool value )
		{
			var enabled = GUILayout.Toggle( value, string.Empty );
			GUILayout.Label( label, GUILayout.Width( 66 ) );
			return enabled;
		}

	}
}
