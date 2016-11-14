using UnityEngine;
using UnityEditor;

namespace CoreEditor
{
	[CustomEditor( typeof(MonoBehaviour), true ), CanEditMultipleObjects]
	public class MonoBehaviourExtendedInspector : BaseExtendedInspector
	{
	}
}