using System;

namespace CoreEngine
{
	[AttributeUsage( AttributeTargets.Class )]
	sealed public class JsonSerializableAttribute : Attribute
	{
	}
}