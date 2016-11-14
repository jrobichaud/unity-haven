using UnityEngine;
using UnityEditor;

namespace CoreEditor
{
	[CustomEditor( typeof(ScriptableObject), true ), CanEditMultipleObjects]
	public class ScriptableObjectExtendedInspector : BaseExtendedInspector
	{
	}
}