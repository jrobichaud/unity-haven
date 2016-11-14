using NUnit.Framework;

namespace CoreEditor.Drawing.Tests
{
	[TestFixture]
	public class CompareTests
	{
		[Test]
		public void Idendical()
		{
			Assert.IsTrue( Compare.IsIdentical( "Assets/Core.Drawing/Tests/Editor/A0.png", "Assets/Core.Drawing/Tests/Editor/A1.png" ) );
			Assert.IsTrue( Compare.IsIdentical( "Assets/Core.Drawing/Tests/Editor/B1.png", "Assets/Core.Drawing/Tests/Editor/A1.png" ) );
			Assert.IsFalse( Compare.IsIdentical( "Assets/Core.Drawing/Tests/Editor/A0.png", "Assets/Core.Drawing/Tests/Editor/difference.png" ) );
			Assert.IsFalse( Compare.IsIdentical( "Assets/Core.Drawing/Tests/Editor/A0.png", "Assets/Core.Drawing/Tests/Editor/O.png" ) );
			Assert.IsFalse( Compare.IsIdentical( "Assets/Core.Drawing/Tests/Editor/difference.png", "Assets/Core.Drawing/Tests/Editor/differenceAlpha.png" ) );
		}

		[Test]
		public void Space()
		{
			Assert.IsTrue( Compare.IsIdentical("Assets/Core.Drawing/Tests/Editor/Test Space.png", "Assets/Core.Drawing/Tests/Editor/Test Space.png"));
		}
	}
}
