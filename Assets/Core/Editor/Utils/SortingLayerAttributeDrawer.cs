using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using CoreEngine;

namespace CoreEditor
{
	[CustomPropertyDrawer(typeof(SortingLayerAttribute),true)]
	public class SortingLayerAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var value = property.stringValue;

			var sortingLayers = SortingLayers;

			int index = -1;
			for ( int i = 0; i < sortingLayers.Length; ++i )
			{
				if ( sortingLayers[i] == value )
				{
					index = i;
					break;
				}
			}
			EditorGUI.BeginChangeCheck();
			index = EditorGUI.Popup( position, label.text , index,  sortingLayers );

			if ( EditorGUI.EndChangeCheck() )
			{
				if ( index>= 0 && index < sortingLayers.Length )
					property.stringValue = sortingLayers[index];
			}
		}
		static string[] SortingLayers
		{
			get
			{
				System.Type internalEditorUtilityType = typeof (InternalEditorUtility);
				PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty ("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
				
				return (string[]) sortingLayersProperty.GetValue (null, new object[0]);
			}
		}
	}
}
