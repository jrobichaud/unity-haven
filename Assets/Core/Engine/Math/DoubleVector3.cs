using UnityEngine;
using System;

namespace CoreEngine.Math
{
	[Serializable]
	public sealed class DoubleVector3 : BaseVector3<DoubleVector3,double>
	{
		static DoubleVector3()
		{
			OperatorAdd = (a,b)=>a+b;
			OperatorSubstract = (a,b)=>a-b;
			OperatorMultiply = (a,b)=>a*b;
			OperatorDivision = (a,b)=>a/b;
		}
		public DoubleVector3():base()
		{
		}
		public DoubleVector3 (double x, double y, double z):base(x,y,z)
		{
		}
	}
}

namespace CoreEngine.Math.Extensions
{
	public static class DoubleVector3Extensions
	{
		public static DoubleVector3 ToDoubleVector3( this Vector2 self )
		{
			return new DoubleVector3( self.x, self.y, 0 );
		}
		
		public static DoubleVector3 ToDoubleVector3( this Vector3 self )
		{
			return new DoubleVector3( self.x, self.y, self.z );
		}
	}
}