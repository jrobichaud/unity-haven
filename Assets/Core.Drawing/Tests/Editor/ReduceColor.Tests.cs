using NUnit.Framework;
using System.IO;

namespace CoreEditor.Drawing.Tests
{
	[TestFixture]
	public class ReduceColorTests
	{
		[Test]
		public void ReduceColorTo64()
		{
			var testFilePath = "Assets/Core.Drawing/Tests/Editor/TestImageCopy.png";

			File.Delete( testFilePath );
			File.Copy( "Assets/Core.Drawing/Tests/Editor/TestImage.png", testFilePath );

			var param = new ReduceColor.ReduceColorParams();
			param.InputFile = testFilePath;
			param.NumberOfColors = 64;

			var inputLength = new FileInfo (param.InputFile).Length;
			ReduceColor.RunCommand(param);
			var outputLength = new FileInfo (param.InputFile).Length;
			Assert.IsTrue( inputLength > outputLength );

			File.Delete( testFilePath );
		}

		[Test]
		public void ReduceColorTo64Space()
		{
			var testFilePath = "Assets/Core.Drawing/Tests/Editor/Test SpaceCopy.png";

			File.Delete( testFilePath );
			File.Copy( "Assets/Core.Drawing/Tests/Editor/Test Space.png", testFilePath );

			var param = new ReduceColor.ReduceColorParams();
			param.InputFile = testFilePath;
			param.NumberOfColors = 64;

			var inputLength = new FileInfo (param.InputFile).Length;
			ReduceColor.RunCommand(param);
			var outputLength = new FileInfo (param.InputFile).Length;
			Assert.IsTrue( inputLength > outputLength );

			File.Delete( testFilePath );
		}

	}
}