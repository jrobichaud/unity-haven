using UnityEngine;
using System;

namespace CoreEngine.Math
{
	[Serializable]
	public sealed class IntVector3 : BaseVector3<IntVector3,int>
	{
		static IntVector3()
		{
			OperatorAdd = (a,b)=>a+b;
			OperatorSubstract = (a,b)=>a-b;
			OperatorMultiply = (a,b)=>a*b;
			OperatorDivision = (a,b)=>a/b;
		}
		public IntVector3():base()
		{
		}
		public IntVector3 (int x, int y, int z):base(x,y,z)
		{
		}
	}
}

namespace CoreEngine.Math.Extensions
{
	public static class IntVector3Extensions
	{
		public static IntVector3 ToIntVector3( this Vector2 self )
		{
			return new IntVector3( (int)self.x, (int)self.y, 0 );
		}
		
		public static IntVector3 ToIntVector3( this Vector3 self )
		{
			return new IntVector3( (int)self.x, (int)self.y,(int)self.z );
		}
	}
}