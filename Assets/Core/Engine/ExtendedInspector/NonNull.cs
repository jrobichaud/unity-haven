using System;

namespace CoreEngine
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NonNullAttribute : Attribute
	{
	}
}