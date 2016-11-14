using System;

namespace CoreEngine
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class RegexPatternAttribute : Attribute
	{
		public string regexString;
		public RegexPatternAttribute( string regexString )
		{
			this.regexString = regexString;
		}
	}
}
