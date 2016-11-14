using UnityEngine;
using UnityEditor;

namespace CoreEditor
{
	public static class EditorGUILayoutExtensions
	{
		public static void DropShadowLabel( string text )
		{
			EditorGUILayout.BeginHorizontal();
			var content = new GUIContent(text);
			var rect = GUILayoutUtility.GetRect(content, EditorStyles.boldLabel);
			EditorGUI.DropShadowLabel( rect, content);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}