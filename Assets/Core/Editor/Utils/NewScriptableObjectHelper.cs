using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;


namespace CoreEditor.Utils
{ 
	public class NewScriptableObjectHelper : EditorWindow
	{		
		[MenuItem("Assets/Create/ScriptableObject" )]
		public static void CreateScriptableObject()
		{
			NewScriptableObjectHelper.Show<ScriptableObject>();
		}

		Action m_ExecuteOnGUI = delegate {};
		Type[] m_Types = null;

		public void OnEnable()
		{
			m_ExecuteOnGUI+=SetPosition;
		}

		void SetPosition()
		{
			var rect = position;
			rect.x = Event.current.mousePosition.x;
			rect.y = Event.current.mousePosition.y;
			position = new Rect( rect );
			m_ExecuteOnGUI -= SetPosition;
		}

		public void OnGUI()
		{
			if ( m_Types == null )
			{
				Close();
				return;
			}
			m_ExecuteOnGUI(); 
			EditorGUILayout.PrefixLabel("Select type to create:" );
			EditorGUI.BeginChangeCheck();
			int item = EditorGUILayout.Popup( -1, m_Types.Select( t=>t.FullName).ToArray() );

			if ( EditorGUI.EndChangeCheck() )
			{
				Create( m_Types[item] );
				Close();
			}

		}

		public static void Create<T>()
		{
			Create( typeof(T) );
		}

		static void Create( Type type )
		{
			var assetPath = string.Empty;
			if ( Selection.activeObject != null )
			{
				var path = AssetDatabase.GetAssetPath( Selection.activeObject );
				if ( Directory.Exists( path ) )
					assetPath = path;
				else
					assetPath = Path.GetDirectoryName( path );
			}
			else
			{
				assetPath = "Assets";
			}
			
			assetPath+=string.Format("/New{0}.asset",type.Name);
			 
			var scriptableObject = ScriptableObject.CreateInstance(type.Name);
			
			AssetDatabase.CreateAsset( scriptableObject, assetPath );
			
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = scriptableObject;

		}

		public static void Show<T>( ) where T : ScriptableObject
		{
			var window = ScriptableObject.CreateInstance<NewScriptableObjectHelper>();
			window.Init<T>( );
			var rect = new Rect(0,0, 0,0 );
			window.ShowAsDropDown( rect, new Vector2( 200, EditorGUIUtility.singleLineHeight*3 ) );
			window.Focus();
		}

		void Init<T>() where T : ScriptableObject
		{
			var types = new List<Type>();

			foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies().Where( a=> a.GetName().Name.StartsWith( "Unity" )== false) )
			{
				try
				{
					foreach( var type in assembly.GetTypes() )
					{
						if ( type.IsSubclassOf( typeof(T) ) && type.IsSubclassOf( typeof(EditorWindow) ) == false && type.IsSubclassOf( typeof(Editor) ) == false && type.IsGenericType == false && type.IsAbstract == false )
							types.Add( type );
					}
				}
				catch ( System.Reflection.ReflectionTypeLoadException ) {}
			}

			m_Types = types.OrderBy(t=>t.FullName).ToArray();
		}
	}
}

