using UnityEngine;
using NUnit.Framework;
using UnityEditor;
using System.IO;

namespace CoreEditor.Drawing.Tests
{
	[TestFixture]
	public class ConvertTests
	{
		[Test]
		public void Resize()
		{
			var param = new Convert.ConvertParams();
			param.InputFile = "Assets/Core.Drawing/Tests/Editor/TestImage.png";
			param.OutputFile = FileUtil.GetUniqueTempPathInProject() + ".png";
			param.ResizeArgs = "50%";
			Convert.RunCommand(param);
			var bytes = File.ReadAllBytes( param.OutputFile );
			var outputTexture = new Texture2D( 4, 4 );
			var inputTexture = AssetDatabase.LoadAssetAtPath( param.InputFile, typeof( Texture2D ) ) as Texture2D;
			Assert.IsTrue( outputTexture.LoadImage( bytes ) );

			Assert.AreEqual( inputTexture.width, outputTexture.width * 2 );
			Assert.AreEqual( inputTexture.height, outputTexture.height * 2 );
			Object.DestroyImmediate( outputTexture );
		}

		[Test]
		public void Space()
		{
			
			var param = new Convert.ConvertParams();
			param.InputFile = "Assets/Core.Drawing/Tests/Editor/Test Space.png";
			param.OutputFile = FileUtil.GetUniqueTempPathInProject() + ".png";

			param.ResizeArgs = "50%";
			Convert.RunCommand(param);
			Assert.IsTrue( File.Exists( param.OutputFile ) );

		}
	}
}