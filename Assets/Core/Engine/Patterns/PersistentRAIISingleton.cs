using UnityEngine;


namespace CoreEngine
{
	public abstract class PersistentRAIISingleton<T> : RAIISingleton<T> where T : PersistentRAIISingleton<T>
	{
		protected override GameObject InstanceOwner
		{
			get
			{
				if ( _persistentInstanceOwner == null && IsApplicationQuitting == false )
				{
					_persistentInstanceOwner = new GameObject( "Persistent RAII Singletons" );
					DontDestroyOnLoad( _persistentInstanceOwner );
				}
				return _persistentInstanceOwner;
			}
		}
	}
}
