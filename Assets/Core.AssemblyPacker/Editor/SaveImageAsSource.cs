using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace CoreEditor.AssemblyPacker
{
	public static class SaveImageAsSource
	{

const string ScriptTemplate = 
@"
public static partial class TextureByteArray
{{
	public static readonly byte[] {0} = new byte[]{{0x{1}}};
}}
";

		[MenuItem("Assets/Save Image as Source", true)]
		public static bool Validate()
		{
			return Selection.objects.Length>0;
		}
		[MenuItem("Assets/Save Image as Source",false)]
		public static void Run()
		{
			foreach( var obj in Selection.objects )
			{
				if ( obj is Texture2D )
				{
					var path = AssetDatabase.GetAssetPath( obj );
					var bytes = File.ReadAllBytes( path );

					string hex = BitConverter.ToString(bytes);
					hex = hex.Replace("-",",0x");

					var name = Path.GetFileNameWithoutExtension( path ).Replace( " ", "_");

					var fullSource = string.Format( ScriptTemplate, name, hex );

					var destination = Path.GetDirectoryName( path) + "/" + Path.GetFileNameWithoutExtension(path) + ".cs";
					File.WriteAllText( destination, fullSource );
					AssetDatabase.ImportAsset( destination );
				}
			}
		}

	}
}
