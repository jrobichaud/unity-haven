using UnityEngine;
using System;

namespace CoreEngine.Math
{
	public abstract class BaseVector<TImpl,T> where T:struct, System.IConvertible where TImpl:BaseVector<TImpl,T>,new()
	{
		protected static Func<T,T,T> OperatorAdd;
		protected static Func<T,T,T> OperatorSubstract;
		protected static Func<T,T,T> OperatorMultiply;
		protected static Func<T,T,T> OperatorDivision;

		protected static int Dimensions{get;set;}
		public abstract T this[int i]
		{
			get;
			set;
		}

		public static TImpl operator +( BaseVector<TImpl,T>  a, BaseVector<TImpl,T>  b )
		{
			var impl = new TImpl();
			for ( int i = 0; i< Dimensions; ++i )
				impl[i] = OperatorAdd(a[i], b[i]);
			return impl;
		}
		public static TImpl operator -( BaseVector<TImpl,T>  a, BaseVector<TImpl,T>  b )
		{
			var impl = new TImpl();
			for ( int i = 0; i< Dimensions; ++i )
				impl[i] = OperatorSubstract(a[i], b[i]);
			return impl;
		}
		public static TImpl operator *( BaseVector<TImpl,T>  a, BaseVector<TImpl,T>  b )
		{
			var impl = new TImpl();
			for ( int i = 0; i< Dimensions; ++i )
				impl[i] = OperatorMultiply(a[i], b[i]);
			return impl;
		}
		public static TImpl operator /( BaseVector<TImpl,T>  a, BaseVector<TImpl,T>  b )
		{
			var impl = new TImpl();
			for ( int i = 0; i< Dimensions; ++i )
				impl[i] = OperatorDivision(a[i], b[i]);
			return impl;
		}
		public static bool operator ==( BaseVector<TImpl,T>  a, BaseVector<TImpl,T>  b )
		{
			for ( int i = 0; i< Dimensions; ++i )
				if ( a[i].Equals( b[i] ) == false )
					return false;
			return true;
		}
		public static bool operator !=( BaseVector<TImpl,T>  a, BaseVector<TImpl,T>  b )
		{
			for ( int i = 0; i< Dimensions; ++i )
				if ( a[i].Equals( b[i] ) == false )
					return true;
			return false;
		}
		public override bool Equals (object obj)
		{
			if ( obj.GetType() != obj.GetType() )
				return false;
			for ( int i = 0; i< Dimensions; ++i )
				if ( this[i].Equals( ((TImpl)obj)[i] ) == false )
					return false;
			return true;
		}
		public override int GetHashCode ()
		{
			int hash = 0;
			for ( int i = 0; i< Dimensions; ++i )
				hash ^= this[i].GetHashCode();
			return hash;
		}

		public static explicit operator Vector2( BaseVector<TImpl,T> v )
		{
			return new Vector2( (float)Convert.ChangeType(v[0],typeof(float)), (float)Convert.ChangeType(v[1],typeof(float)) );
		}
		public static explicit operator Vector3( BaseVector<TImpl,T> v )
		{
			return new Vector3( (float)Convert.ChangeType(v[0],typeof(float)), (float)Convert.ChangeType(v[1],typeof(float)), (Dimensions>2)?(float)Convert.ChangeType(v[2],typeof(float)):0 );
		}
	}
}