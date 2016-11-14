using UnityEngine;
using System.Collections;

namespace CoreEngine
{
	public class CoroutineHelper : PersistentRAIISingleton<CoroutineHelper>
	{
		public static new void StartCoroutine( IEnumerator coroutine )
		{
			if ( HasInstance == false )
				return;

			MonoBehaviour baseComponent = Instance;
			baseComponent.StartCoroutine( StartExternalCoroutine( coroutine ) );
		}

		static IEnumerator StartExternalCoroutine( IEnumerator coroutine )
		{
			yield return new WaitForEndOfFrame();
			while ( coroutine.MoveNext() )
				yield return coroutine.Current;
		}
	}
}