using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

using CoreEngine.Math.Extensions;

namespace CoreEngine.Math.Test
{
	[TestFixture]
	public class TestDoubleVector2
	{
		[Test]
		public void Add()
		{
			var a = new DoubleVector2( 1, 1 );
			var b = new DoubleVector2( 2, 2 );
			var c = a + b;

			Assert.AreEqual( c.x , 3 );
			Assert.AreEqual( c.y , 3 );
		}
		[Test]
		public void Substract()
		{
			var a = new DoubleVector2( 1, 1 );
			var b = new DoubleVector2( 2, 2 );
			var c = a - b;
			
			Assert.AreEqual( c.x , -1 );
			Assert.AreEqual( c.y , -1 );
		}
		[Test]
		public void Multiply()
		{
			var a = new DoubleVector2( 2, 2 );
			var b = new DoubleVector2( 3, 3 );
			var c = a * b;
			
			Assert.AreEqual( c.x , 6 );
			Assert.AreEqual( c.y , 6 );
		}
		[Test]
		public void Division()
		{
			var a = new DoubleVector2( 8, 8 );
			var b = new DoubleVector2( 2, 2 );
			var c = a / b;
			
			Assert.AreEqual( c.x , 4 );
			Assert.AreEqual( c.y , 4 );
		}
		[Test]
		public void OperatorEqual()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new DoubleVector2( 1, 2 );
			var c = new DoubleVector2( 2, 3 );
			
			Assert.IsTrue( a == b );
			Assert.IsFalse( a == c );
		}
		[Test]
		public void OperatorDifferent()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new DoubleVector2( 1, 2 );
			var c = new DoubleVector2( 2, 3 );
			
			Assert.IsFalse( a != b );
			Assert.IsTrue( a != c );
		}
		
		[Test]
		public void Equals()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new DoubleVector2( 1, 2 );
			var c = new DoubleVector2( 2, 3 );
			
			Assert.IsTrue( a.Equals( b ) );
			Assert.IsFalse( a.Equals( c ) );
		}

		
		[Test]
		public new void GetHashCode()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new DoubleVector2( 1, 2 );
			var c = new DoubleVector2( 1, 2 );

			var hashSet = new HashSet<DoubleVector2> ();
			hashSet.Add( a );
			hashSet.Add( b );
			hashSet.Add( c );

			Assert.AreEqual( hashSet.Count, 1 );
		}

		[Test]
		public void Vector2Cast()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new Vector2( 1, 2 );

			Assert.IsTrue( (Vector2)a == b );
		}

		[Test]
		public void Vector3Cast()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new Vector3( 1, 2, 0 );
			
			Assert.IsTrue( (Vector3)a == b );
		}

		[Test]
		public void Vector2CastToDoubleVector2()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new Vector2( 1, 2 );
			
			Assert.IsTrue( a == b.ToDoubleVector2() );
		}
		
		[Test]
		public void Vector3CastToDoubleVector2()
		{
			var a = new DoubleVector2( 1, 2 );
			var b = new Vector3( 1, 2, 0 );
			
			Assert.IsTrue( a == b.ToDoubleVector2() );
		}
	}
}
