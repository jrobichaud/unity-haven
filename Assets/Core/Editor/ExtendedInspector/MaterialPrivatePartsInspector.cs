using UnityEditor;
using UnityEngine;

namespace CoreEditor
{
	[CustomEditor(typeof(Material))]
	[CanEditMultipleObjects]
	public class MaterialPrivatePartsInspector : MaterialEditor {

	const string Tooltip =
	@"Default          =      -1
	Background = 1000
	Geometry     = 2000
	AlphaTest     = 2450
	Transparent = 3000
	Overlay         = 4000";

		SerializedProperty mCustomRenderQueue;

		public override void OnEnable ()
		{
			base.OnEnable();
			mCustomRenderQueue = serializedObject.FindProperty ("m_CustomRenderQueue");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField( mCustomRenderQueue, new GUIContent( "Custom Render Queue *", Tooltip ) );
			serializedObject.ApplyModifiedProperties();
		}
	}
}