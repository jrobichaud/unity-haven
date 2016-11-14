using UnityEditor;
using UnityEngine;
using CoreEngine.Math;

namespace CoreEditor.Math
{
	[CustomPropertyDrawer(typeof(IntVector2))]
	[CustomPropertyDrawer(typeof(IntVector3))]
	[CustomPropertyDrawer(typeof(DoubleVector2))]
	[CustomPropertyDrawer(typeof(DoubleVector3))]
	public class VectorDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			var labels = new GUIContent[]{new GUIContent("X"),new GUIContent("Y"),new GUIContent("Z")};
			var properties = new string[]{"x","y","z"};
			EditorGUI.PrefixLabel(position, label );
			position.x+=EditorGUIUtility.labelWidth-1;
			float remainingWidth = position.width - EditorGUIUtility.labelWidth;
			var labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 13f;
			var splitSpace = remainingWidth / 3;
			position.width = splitSpace-1;

			for ( int i = 0; i < 3; ++ i)
			{
				var valueProperty = property.FindPropertyRelative(properties[i]);
				if ( valueProperty != null )
				{
					EditorGUI.PropertyField( position,valueProperty, labels[i] );
					position.x+=position.width+2;
				}
			}
			EditorGUIUtility.labelWidth = labelWidth;
		}
	}
}


