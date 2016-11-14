using UnityEngine;

namespace CoreEngine
{
	public abstract class CommonSingleton : MonoBehaviour
	{
		protected static string GetGameObjectNameFromType( System.Type type )
		{
			string name = type.Name;

			if ( string.IsNullOrEmpty( type.Namespace ) == false )
				name = type.Namespace + "/" + name;

			return name;
		}
	}
}