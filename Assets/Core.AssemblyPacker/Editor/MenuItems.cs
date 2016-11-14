using UnityEditor;
using CoreEditor.Utils;

namespace CoreEditor.AssemblyPacker
{
	public static class MenuItems
	{
		[MenuItem("Assets/Create/AssemblyPacker/Packer", false, 800 )]
		public static void CreateBuildSequence()
		{
			NewScriptableObjectHelper.Create<Packer>();
		}
		
		[MenuItem("Assets/Create/AssemblyPacker/Assembly", false, 801 )]
		public static void CreateBuildStep()
		{
			NewScriptableObjectHelper.Create<PackerAssembly>();
		}
	}
}


