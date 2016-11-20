using System;
using System.Diagnostics;

namespace CoreEngine
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NonNullAttribute : Attribute
	{
	}
}