using UnityEngine;
using CoreEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CoreEditor.AssemblyPacker
{
	public class PackerAssembly: ScriptableObject
	{
		[System.Serializable]
		public class Meta
		{
			[RegexPattern("^$|^[a-f0-9]{32}$")]
			public string GUID = string.Empty;

			public bool IsGUIDSpecified
			{
				get
				{
					return Regex.IsMatch( GUID, "^[a-f0-9]{32}$" );
				}
			}
		}

		public Object[] m_Sources = null;
		public Object[] m_AssemblyReferences = null;
		public string[] m_Defines = null;
		public bool m_ExportInlineDocumentation = false;
		public bool m_IsDebug = false;
		public Meta m_AssemblyMeta = new Meta();
		public Meta m_AssemblyMDBMeta = new Meta();
		public Meta m_DocumentationMeta = new Meta();


		public string AssemblyPath
		{
			get
			{
				var dllPath = name + ".dll";
				if ( IsEditor )
					return "Assets/Plugins/Editor/" + dllPath;
				else
					return "Assets/Plugins/" + dllPath;
			}
		}

		public bool IsEditor
		{
			get
			{
				return name.Contains("Editor");
			}
		}

		public List<string> Defines
		{
			get{
				var defines = new List<string>() {
				};
				
				if ( m_IsDebug == false )
					defines.Add( "WORKAROUND_TO_DISABLE_DEBUG_COMPILATION -debug-" );
				
				if ( m_Defines.Length > 0 )
					defines.AddRange( m_Defines );
				return defines;
			}
		}
	}
}
