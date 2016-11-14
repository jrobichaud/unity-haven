using System;

namespace CoreEngine.Math
{
	[Serializable]
	public abstract class BaseVector3<TImpl,T> : BaseVector2<TImpl,T> where T:struct, IConvertible where TImpl: BaseVector3<TImpl,T>, new()
	{
		public T z = default(T);
		
		public BaseVector3()
		{
			Dimensions = 3;
		}
		
		public BaseVector3( T x, T y, T z ) : this()
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		
		public override T this[int i]
		{
			get {
				switch( i )
				{
					case 0: return x;
					case 1: return y;
					case 2: return z;
					default: return default(T);
				}
			}
			set {
				switch( i )
				{
					case 0: x = value; break;
					case 1: y = value; break;
					case 2: z = value; break;
				}
			}
		}
	}
}
