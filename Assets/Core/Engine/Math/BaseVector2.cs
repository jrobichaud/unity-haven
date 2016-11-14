using System;

namespace CoreEngine.Math
{
	[Serializable]
	public abstract class BaseVector2<TImpl,T> : BaseVector<TImpl,T> where T:struct, IConvertible where TImpl:BaseVector2<TImpl,T>,new()
	{
		public T x = default(T);
		public T y = default(T);

		public override T this[int i]
		{
			get {
				switch( i )
				{
					case 0: return x;
					case 1: return y;
					default: return default(T);
				}
			}
			set {
				switch( i )
				{
					case 0: x = value; break;
					case 1: y = value; break;
				}
			}
		}

		public BaseVector2()
		{
			Dimensions = 2;
		}

		public BaseVector2( T x, T y ) : this()
		{
			this.x = x;
			this.y = y;
		}
	}
}
