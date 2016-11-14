using CoreEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace CoreEditor
{
	[ExtendedInspectorDrawer(typeof(ReorderableAttribute))]
	public sealed class ReorderAttributeDrawer : IDrawer
	{
		static int[] selectedIDs = new int[]{};
		static ReorderAttributeDrawer()
		{
			EditorApplication.update += ()=> {
				if ( mUsedLists.Count == 0 )
					return;
				if ( Selection.instanceIDs.Length == 0 )
				{
					selectedIDs = new int[]{};
					mUsedLists.Clear();
				}
				else if ( Selection.instanceIDs.SequenceEqual( selectedIDs ) == false )
				{
					selectedIDs = Selection.instanceIDs;
					mUsedLists.Clear();
				}
			};

		}
		public ReorderAttributeDrawer( ReorderableAttribute attribute )
		{
		}
		static Dictionary<string,ReorderableList> mUsedLists = new Dictionary<string, ReorderableList>();
		public bool DisplayProperty( SerializedProperty property, FieldInfo field, GUIContent label )
		{
			var list = GetList( property );
			float height = 0;
			for(var i = 0; i < property.arraySize; i++)
			{
				height = Mathf.Max(height, EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i)));
			}
			height+=2;
			list.elementHeight = height;
			list.drawElementCallback =  
			(Rect rect, int index, bool isActive, bool isFocused) => {
			
				var element = list.serializedProperty.GetArrayElementAtIndex(index);
				
				rect.y += 2;
				EditorGUI.LabelField( 
                     new Rect(rect.x, rect.y, 64, EditorGUIUtility.singleLineHeight),
				                     element.displayName );
				
				EditorGUI.PropertyField(
					new Rect(rect.x+64, rect.y, rect.width-64, height ),
					element, GUIContent.none, true);
				rect.height = height;
			};

			list.drawHeaderCallback = (rect)=> EditorGUI.LabelField( rect, label );
			list.DoLayoutList();

			return false;
		}

		static ReorderableList GetList( SerializedProperty property )
		{
			var key = property.serializedObject.targetObject.GetInstanceID() + property.propertyPath;

			ReorderableList list;

			if ( mUsedLists.TryGetValue(key, out list ) == false )
			{
				list = mUsedLists[key] = new ReorderableList(property.serializedObject, 
				                                             property, 
				                                             true, true, true, true);
			}
			list.serializedProperty = property;
			list.serializedProperty.GetType().GetField( "m_SerializedObject", BindingFlags.Instance|BindingFlags.NonPublic ).SetValue( list.serializedProperty, property.serializedObject );
			return list;
		}
		
		public bool IsValid( object value )
		{
			return true;
		}
		public string GetError( object value )
		{
			return string.Empty;
		}
		
		public string Tooltip
		{
			get {
				return string.Empty;
			}
		}
	}
}
