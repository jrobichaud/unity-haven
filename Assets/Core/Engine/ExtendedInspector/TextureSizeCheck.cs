using System;

namespace CoreEngine
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class TextureSizeCheckAttribute : Attribute
	{
		public int Width{get;private set;}
		public int Height{get;private set;}
		public TextureSizeCheckAttribute( int width, int height )
		{
			this.Width = width;
			this.Height = height;
		}
	}
}
