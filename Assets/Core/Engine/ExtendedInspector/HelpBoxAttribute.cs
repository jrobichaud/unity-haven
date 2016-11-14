using System;

namespace CoreEngine
{
	public enum HelpBoxIconType
	{
		None,
		Info,
		Warning,
		Error
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true )]
	public sealed class HelpBoxAttribute : Attribute
	{
		public string Description{ get; private set; }
		public HelpBoxIconType Icon{ get; set; }

		HelpBoxAttribute(){}
		public HelpBoxAttribute( params string[] description  )
		{
			Description = string.Join("\n",description );
			Icon = HelpBoxIconType.None;
		}
	}
}
