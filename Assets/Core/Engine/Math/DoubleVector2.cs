using UnityEngine;
using System;

namespace CoreEngine.Math
{
	[Serializable]
	public sealed class DoubleVector2 : BaseVector2<DoubleVector2,double>
	{
		static DoubleVector2()
		{
			OperatorAdd = (a,b)=>a+b;
			OperatorSubstract = (a,b)=>a-b;
			OperatorMultiply = (a,b)=>a*b;
			OperatorDivision = (a,b)=>a/b;
		}
		public DoubleVector2():base()
		{
		}
		public DoubleVector2 (double x, double y):base(x,y)
		{
		}
	}
}

namespace CoreEngine.Math.Extensions
{
	public static class DoubleVector2Extensions
	{
		public static DoubleVector2 ToDoubleVector2( this Vector2 self )
		{
			return new DoubleVector2( self.x, self.y );
		}
		
		public static DoubleVector2 ToDoubleVector2( this Vector3 self )
		{
			return new DoubleVector2( self.x, self.y );
		}
	}
}