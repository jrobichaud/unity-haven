using System;

namespace CoreEngine
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SliderAttribute : Attribute
	{
		public float Min{get;private set;}
		public float Max{get;private set;}
		public SliderAttribute( float min, float max )
		{
			this.Min = min;
			this.Max = max;
		}
	}
}