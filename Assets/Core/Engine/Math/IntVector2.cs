using UnityEngine;
using System;

namespace CoreEngine.Math
{
	[Serializable]
	public sealed class IntVector2 : BaseVector2<IntVector2,int>
	{
		static IntVector2()
		{
			OperatorAdd = (a,b)=>a+b;
			OperatorSubstract = (a,b)=>a-b;
			OperatorMultiply = (a,b)=>a*b;
			OperatorDivision = (a,b)=>a/b;
		}
		public IntVector2():base()
		{
		}
		public IntVector2 (int x, int y):base(x,y)
		{
		}
	}
}

namespace CoreEngine.Math.Extensions
{
	public static class IntVector2Extensions
	{
		public static IntVector2 ToIntVector2( this Vector2 self )
		{
			return new IntVector2( (int)self.x, (int)self.y );
		}
		
		public static IntVector2 ToIntVector2( this Vector3 self )
		{
			return new IntVector2( (int)self.x, (int)self.y );
		}
	}
}