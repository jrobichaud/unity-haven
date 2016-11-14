using System;

namespace CoreEngine
{
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
