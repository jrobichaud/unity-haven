using UnityEditor;

namespace CoreEditor
{
	public class BaseExtendedInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			if ( serializedObject.targetObject != null )
			{
				ExtendedInspector.ShowGenericEditor( serializedObject );
			}
			else
			{
				EditorGUILayout.HelpBox( "Missing Script.\nFix \"Script\"'s reference or remove Component.", MessageType.Error );
				base.DrawDefaultInspector();
			}
		}
	}
}

