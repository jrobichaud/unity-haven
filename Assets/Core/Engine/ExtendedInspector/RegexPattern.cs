using System;
using System.Diagnostics;

namespace CoreEngine
{
	[Conditional("UNITY_EDITOR")]
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
