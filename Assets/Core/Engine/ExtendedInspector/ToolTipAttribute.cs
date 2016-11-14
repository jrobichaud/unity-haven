using System;

namespace CoreEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
	public sealed class ToolTipAttribute : Attribute
	{
		public string Description{get;private set;}

		ToolTipAttribute(){}

		public ToolTipAttribute( params string[] description )
		{
			Description = string.Join("\n",description);
		}
	}
}
