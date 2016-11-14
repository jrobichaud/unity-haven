using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;
using System.IO;
using System;
using CoreEditor.Utils;

namespace CoreEditor.Drawing
{
	public class Ditherer : AssetPostprocessor
	{
		public static readonly Dictionary<TextureFormat, string> FormatToPackerFormat = new Dictionary<TextureFormat, string>()
		{
			{TextureFormat.ARGB4444,"r4g4b4a4"},
			{TextureFormat.RGB565,"r5g6b5"},
			
			// This is a bug in Unity 4.0.0:
			{(TextureFormat) TextureImporterFormat.RGBA16,"r4g4b4a4"},
		};

		public override uint GetVersion() {return 3;}
		public override int GetPostprocessOrder() {return 10;}

		public static bool Is16Bit( Texture2D texture )
		{
			return FormatToPackerFormat.ContainsKey( texture.format );
		}

		public void OnPostprocessTexture (Texture2D texture)
		{
			var format = texture.format;

			bool isExternalLoading = AssetImportUtils.GetUserData<bool>( assetImporter, "externalLoading" );
		
			if ( Is16Bit( texture ) && !isExternalLoading )
			{		
				try{
					using ( var originalTextureWWW = new WWW( "file://" + Directory.GetCurrentDirectory().Replace(@"\","/") + "/" + assetPath ) )		
					{
						while (originalTextureWWW.isDone == false );
					
						var originalTexture = originalTextureWWW.texture;

						var tempPathPrefix = FileUtil.GetUniqueTempPathInProject();
						var destTexture = tempPathPrefix + ".Out.png";
						var pvrTexture = tempPathPrefix + ".pvr";

						bool needsResize = texture.width != originalTexture.width || texture.height != originalTexture.height;
						
						string srcTexture = assetPath;

						var ditheringParam = new PVRTexTool.PVRTexToolParams() {
							InputFile = srcTexture,
							PVROutputFile = pvrTexture,
							OutputFile = tempPathPrefix + ".png",
							Dither = true,
							ResizeFilter = PVRTexTool.ResizeFilter.Cubic,
							EncodeFormat = FormatToPackerFormat[ format ]
						};

						if ( needsResize )
						{
							ditheringParam.Width = texture.width;
							ditheringParam.Height = texture.height;
						}

						bool succeeded = PVRTexTool.RunCommand( ditheringParam );
						if ( succeeded )
						{
							using ( var www = new WWW( "file://" + Directory.GetCurrentDirectory().Replace(@"\","/") + "/" + destTexture ) )
							{
								while (www.isDone == false );
								if ( string.IsNullOrEmpty ( www.error ) == false )
									Debug.LogError( www.error );
								if (www.texture != null )
								{
									var bytes = www.texture.EncodeToPNG();				
									texture.LoadImage( bytes );
									EditorUtility.CompressTexture( texture , format, TextureCompressionQuality.Normal );
									EditorUtility.SetDirty( texture );
								}
							}
							File.Delete( destTexture );
							File.Delete( pvrTexture );
						}
					}
				}
				catch
				{
				}
			}
		}	
	}
}
