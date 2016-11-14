using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

using CoreEngine.Math.Extensions;

namespace CoreEngine.Math.Test
{
	[TestFixture]
	public class TestDoubleVector3
	{
		[Test]
		public void Add()
		{
			var a = new DoubleVector3( 1, 1, 1 );
			var b = new DoubleVector3( 2, 2, 2 );
			var c = a + b;

			Assert.AreEqual( c.x , 3 );
			Assert.AreEqual( c.y , 3 );
			Assert.AreEqual( c.z , 3 );
		}
		[Test]
		public void Substract()
		{
			var a = new DoubleVector3( 1, 1, 1 );
			var b = new DoubleVector3( 2, 2, 2 );
			var c = a - b;
			
			Assert.AreEqual( c.x , -1 );
			Assert.AreEqual( c.y , -1 );
			Assert.AreEqual( c.z , -1 );
		}
		[Test]
		public void Multiply()
		{
			var a = new DoubleVector3( 2, 2, 2 );
			var b = new DoubleVector3( 3, 3, 3 );
			var c = a * b;
			
			Assert.AreEqual( c.x , 6 );
			Assert.AreEqual( c.y , 6 );
			Assert.AreEqual( c.z , 6 );
		}
		[Test]
		public void Division()
		{
			var a = new DoubleVector3( 8, 8, 8 );
			var b = new DoubleVector3( 2, 2, 2 );
			var c = a / b;
			
			Assert.AreEqual( c.x , 4 );
			Assert.AreEqual( c.y , 4 );
			Assert.AreEqual( c.z , 4 );
		}
		[Test]
		public void OperatorEqual()
		{
			var a = new DoubleVector3( 1, 2, 3 );
			var b = new DoubleVector3( 1, 2, 3 );
			var c = new DoubleVector3( 2, 3, 4 );
			
			Assert.IsTrue( a == b );
			Assert.IsFalse( a == c );
		}
		[Test]
		public void OperatorDifferent()
		{
			var a = new DoubleVector3( 1, 2, 3 );
			var b = new DoubleVector3( 1, 2, 3 );
			var c = new DoubleVector3( 2, 3, 4 );
			
			Assert.IsFalse( a != b );
			Assert.IsTrue( a != c );
		}
		
		[Test]
		public void Equals()
		{
			var a = new DoubleVector3( 1, 2, 3 );
			var b = new DoubleVector3( 1, 2, 3 );
			var c = new DoubleVector3( 2, 3, 4 );
			
			Assert.IsTrue( a.Equals( b ) );
			Assert.IsFalse( a.Equals( c ) );
		}

		
		[Test]
		public new void GetHashCode()
		{
			var a = new IntVector2( 1, 2 );
			var b = new IntVector2( 1, 2 );
			var c = new IntVector2( 1, 2 );

			var hashSet = new HashSet<IntVector2> ();
			hashSet.Add( a );
			hashSet.Add( b );
			hashSet.Add( c );

			Assert.AreEqual( hashSet.Count, 1 );
		}

		[Test]
		public void Vector2Cast()
		{
			var a = new DoubleVector3( 1, 2, 3 );
			var b = new Vector2( 1, 2 );

			Assert.IsTrue( (Vector2)a == b );
		}

		[Test]
		public void Vector3Cast()
		{
			var a = new DoubleVector3( 1, 2, 3 );
			var b = new Vector3( 1, 2, 3 );
			
			Assert.IsTrue( (Vector3)a == b );
		}

		[Test]
		public void Vector2CastToIntVector2()
		{
			var a = new DoubleVector3( 1, 2, 0 );
			var b = new Vector2( 1, 2 );
			
			Assert.IsTrue( a == b.ToDoubleVector3() );
		}
		
		[Test]
		public void Vector3CastToIntVector2()
		{
			var a = new DoubleVector3( 1, 2, 3 );
			var b = new Vector3( 1, 2, 3 );
			
			Assert.IsTrue( a == b.ToDoubleVector3() );
		}
	}
}
