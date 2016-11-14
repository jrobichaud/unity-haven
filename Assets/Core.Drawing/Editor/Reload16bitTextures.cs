using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;


namespace CoreEditor.Drawing
{
	public class Reload16bitTextures
	{
		[ MenuItem( "TextureDithering/Reload all 16 textures", true, 20 ) ]
		public static bool ReloadValidate()
		{
			return PVRTexTool.IsInstalled;
		}
		
		[ MenuItem( "TexturePacker/Reload all 16 textures", false, 20 ) ]
		public static void Reload()
		{
			try 
			{
				var all16bitTextures = 
					(from asset in AssetDatabase.GetAllAssetPaths()
						where is16bit( asset )
						select asset).ToArray() ;
				int count = 0;
				if ( all16bitTextures.Length == 0 )
					return;
				float increment = 1.0f/(float)all16bitTextures.Length;
				foreach( var asset in all16bitTextures )
				{
					count++;
					if ( EditorUtility.DisplayCancelableProgressBar( "Importing", asset + " ( "+count+" / "+all16bitTextures.Length+" )", (count) * increment ) )
						return;
					AssetDatabase.ImportAsset( asset, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport );
				}
			}
			catch
			{}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}
		
		static bool is16bit( string path )
		{
			var ext = new HashSet<string>()
			{
				".png", ".jpg", ".jpeg"
			};
			if ( ext.Contains( Path.GetExtension( path ) ) == false )
				return false;
			var ass = AssetDatabase.LoadMainAssetAtPath( path ) as Texture2D;
			
			if ( ass.format == TextureFormat.ARGB4444 || ass.format == TextureFormat.RGB565 )
				return true;
			else
				return false;
		}
	}
}