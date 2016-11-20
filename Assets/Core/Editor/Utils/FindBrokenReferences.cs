using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Reflection;

namespace CoreEditor.Utils
{
	[Serializable]
	public class FindBrokenReferences  : EditorWindow
	{	
		[Serializable]
		class Result
		{
			public int line;
			public string meta;		
			public string path;
		}

		[SerializeField]
		List<Result> mResults = new List<Result>(); 

		[SerializeField]
		Vector2 mScrollPosition = Vector2.zero;
		
		public void OnEnable()
		{
			hideFlags = HideFlags.HideAndDontSave;
			Refresh();
		}

		public void OnGUI()
		{
			if ( GUILayout.Button( "Refresh" ) )
				Refresh();
			EditorGUILayout.BeginVertical();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.SelectableLabel("guid", EditorStyles.boldLabel, GUILayout.Width( 230 ));
			EditorGUILayout.SelectableLabel("Type", EditorStyles.boldLabel, GUILayout.Width( 70 ));
			EditorGUILayout.SelectableLabel("Object", EditorStyles.boldLabel, GUILayout.Width( 150 ));
			GUILayout.Space( 90 );
			EditorGUILayout.SelectableLabel("Line", EditorStyles.boldLabel, GUILayout.Width( 40 ));
			EditorGUILayout.SelectableLabel("Path", EditorStyles.boldLabel);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			mScrollPosition = EditorGUILayout.BeginScrollView( mScrollPosition);
			
			foreach ( var result in mResults )
			{
				var meta = result.meta;
				var path = result.path;
				var line = result.line;
				var asset = AssetDatabase.LoadAssetAtPath( path, typeof(Object) );
				if ( asset == null )
					continue;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel( meta, GUILayout.Width( 230 ) );
				EditorGUILayout.SelectableLabel( asset.GetType().ToString().Replace("UnityEngine.",string.Empty), GUILayout.Width( 70 ) );
				EditorGUILayout.ObjectField( asset, typeof(object), false, GUILayout.Width( 150 ) );

				if( GUILayout.Button( "Inspect", GUILayout.ExpandWidth( false ) ))
					AssetDatabase.OpenAsset( asset );
				
				if( GUILayout.Button( "Edit", GUILayout.ExpandWidth( false ) ))
					OpenFileInTextEditor(path);
				
				EditorGUILayout.SelectableLabel( line.ToString(), GUILayout.Width( 40 ) );
				EditorGUILayout.SelectableLabel( path );
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
		}

		static string UnityUIGUID
		{
			get{
				return AssetDatabase.AssetPathToGUID( AppDomain.CurrentDomain.GetAssemblies().
					SingleOrDefault(assembly => assembly.GetName().Name == "UnityEngine.UI").Location );
			}
		}

		void Refresh()
		{
			var guidRegex = new Regex( @"guid: ([0-9a-f]{32})" );
			
			var metaFiles = from item in AssetDatabase.GetAllAssetPaths()
				select item + ".meta";

			var guids = new HashSet<string>(){"0000000000000000d000000000000000","0000000000000000e000000000000000","0000000000000000f000000000000000", UnityUIGUID};

			foreach ( var metaFile in metaFiles )
			{
				if ( File.Exists( metaFile ) )
				{
					var matches = guidRegex.Matches( File.ReadAllText(metaFile) );
					foreach( Match match in matches )
						guids.Add( match.Groups[1].Value.Substring( 0, 32 ) );
				}
			}
			
			var extensions = new HashSet<string>{".prefab",".mat", ".unity", ".GUISkin"};
			
			var paths = from item in AssetDatabase.GetAllAssetPaths()
				where (extensions.Contains( Path.GetExtension( item ) ))
					orderby AssetDatabase.LoadAssetAtPath(item,typeof(Object)).GetType().ToString(), item
					select item ;
			
			mResults = new List<Result>();
			
			foreach ( var path in paths )
			{
				var allLines = File.ReadAllLines(path);
				for ( int line = 0; line<allLines.Length; line++ )
				{
					var matches = guidRegex.Matches( allLines[line] );
					foreach( Match match in matches )
					{
						var meta = match.Groups[1].Value.Substring( 0, 32 );
						if ( guids.Contains( meta ) == false )
							mResults.Add( new Result(){meta=meta,path = path,line=line+1} );		
					}
				}		
			}
		}
		
		[MenuItem("Assets/Open Asset in Text Editor",true,25)]	
		[MenuItem("Assets/Open Meta in Text Editor",true,26)]	
		public static bool Validate ()
		{
			return WhereUsed.IsSelectionAssets();
		}

		[MenuItem("Assets/Open Asset in Text Editor",false,25)]	
		public static void OpenAssetInTextEditor()
		{
			foreach ( var obj in Selection.objects )
			{
				var path = AssetDatabase.GetAssetPath( obj );
				OpenFileInTextEditor( path );
			}
		}
		[MenuItem("Assets/Open Meta in Text Editor",false,26)]	
		public static void OpenMetaInTextEditor()
		{
			foreach ( var obj in Selection.objects )
			{
				var path = AssetDatabase.GetAssetPath( obj ) + ".meta";
				OpenFileInTextEditor( path );
			}
		}

		[MenuItem("Window/Tool/Find Broken References",false)]	
		public static void FindBrokenReferencesToWindow()
		{
			var window = EditorWindow.GetWindow<FindBrokenReferences>();
			window.Show();
		}

		static void OpenFileInTextEditor( string path )
		{
			var process = new Process();
			var fullPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + path.Replace( "/", Path.DirectorySeparatorChar.ToString() );
			if ( Application.platform == RuntimePlatform.OSXEditor )
			{
				process.StartInfo.FileName = "open";
				process.StartInfo.Arguments = "-t \"" + fullPath + "\"";
			}
			else
			{
				const string appKey = @"HKEY_CLASSES_ROOT\.txt";
				var appValue = Microsoft.Win32.Registry.GetValue( appKey, "", null ).ToString();
				var commandKey = @"HKEY_CLASSES_ROOT\" + appValue + @"\shell\open\command";
				var commandValue = Microsoft.Win32.Registry.GetValue( commandKey, string.Empty, null ).ToString();
				var textEditorExe = Regex.Match( commandValue, "(.*) .*$" ).Groups[1].Value;
				process.StartInfo.FileName = textEditorExe;
				process.StartInfo.Arguments = " \"" + fullPath + "\"";
			}
			process.Start();
		}
	}
}
