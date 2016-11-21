using System;
using System.Diagnostics;

namespace CoreEngine
{
	[Conditional("UNITY_EDITOR")]
	[Conditional("PRESERVE_ATTRIBUTES")]
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
