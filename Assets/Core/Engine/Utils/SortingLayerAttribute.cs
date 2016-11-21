using UnityEngine;
using System.Diagnostics;

namespace CoreEngine
{
	[Conditional("UNITY_EDITOR")]
	[Conditional("PRESERVE_ATTRIBUTES")]
	public class SortingLayerAttribute : PropertyAttribute
	{
	}
}
