using UnityEngine;
using UnityEditor;
using System;

namespace CoreEditor.AssemblyPacker
{
	[CustomEditor(typeof(Packer))]
	public class PackerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			ExtendedInspector.ShowGenericEditor( serializedObject );
			try
			{
				if ( GUILayout.Button( "Build and Pack" ) )
				{
					var compiler = (Packer)target;
					compiler.PackAssembly();
				}
				if ( GUILayout.Button( "Generate Dummy Assembly Data" ) )
				{
					var compiler = (Packer)target;
					compiler.GenerateDummyAssemblyData();
					AssetDatabase.Refresh();
				}
			}
			catch( ArgumentException )
			{
			}
		}
	}
}
