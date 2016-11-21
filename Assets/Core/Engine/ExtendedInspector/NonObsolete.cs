using System;
using System.Diagnostics;

namespace CoreEngine
{
	[Conditional("UNITY_EDITOR")]
	[Conditional("PRESERVE_ATTRIBUTES")]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NonObsoleteAttribute : Attribute
	{
		public bool TreatWarningAsError{get;private set;}
		public NonObsoleteAttribute( bool treatWarningAsError = false )
		{
			TreatWarningAsError = treatWarningAsError;
		}
	}
}
