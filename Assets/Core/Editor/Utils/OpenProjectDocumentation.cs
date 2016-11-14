using UnityEditor;
using System.IO;

namespace CoreEditor.Utils
{
	public static class OpenProjectDocumentation
	{
		const string DocumentationFile = "readme.md";
		const string MenuItemText = "Help/Open Project Documentation ("+DocumentationFile+")";
		[MenuItem( MenuItemText, true )]
		public static bool HasDocumentation()
		{
			return File.Exists( DocumentationFile );
		}

		[MenuItem( MenuItemText)]
		public static void OpenDocumenatation()
		{
			EditorUtility.OpenWithDefaultApp( DocumentationFile );
		}
	}
}