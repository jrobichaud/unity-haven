using UnityEngine;

namespace CoreEngine
{
	public abstract class RAIISingleton : CommonSingleton
	{
		protected static bool IsApplicationQuitting {get; private set;}
		
		static GameObject _instanceOwner;
		protected static GameObject _persistentInstanceOwner;

		protected virtual GameObject InstanceOwner
		{
			get
			{
				if ( _instanceOwner == null && IsApplicationQuitting == false )
				{
					_instanceOwner = new GameObject( "RAII Singletons" );
				}
				return _instanceOwner;
			}
		}

		public virtual void OnApplicationQuit()
		{
			IsApplicationQuitting = true;
		}
		
		protected static bool HasInstance
		{
			get
			{
				if ( Application.isEditor == false )
					return true;

				return Application.isPlaying && IsApplicationQuitting == false;
			}
		}
	}

	/// <summary>
	/// Singleton s'instanciant lors de son utilisation
	/// RAII : Resource Acquisition Is Instantiation
	/// </summary>/
	public abstract class RAIISingleton<T> : RAIISingleton where T : RAIISingleton<T>
	{
		static T _instance = null;
		protected static T Instance
		{
			get
			{
				if ( _instance == null && IsApplicationQuitting == false )
				{
					var type = typeof( T );
					var gameObject = new GameObject( GetGameObjectNameFromType( type ), type );
					gameObject.transform.parent = gameObject.GetComponent<T>().InstanceOwner.transform;
				}
				return _instance;
			}

			private set
			{
				_instance = value;
			}
		}

		public virtual void Awake()
		{
			Instance = this as T;
		}

		public virtual void OnDestroy()
		{
			Instance = null;
		}

		#region Bad usage diagnostic
		static void ChangeCallback()
		{
			if ( IsPlayingEditor == false )
			{
				playmodeStateChangedWrapper((a,del,d)=>	System.Delegate.Remove( del, d ) );
				if ( _instance != null )
				{
					Debug.LogError( _instance.GetType() + " should never be called for the first time during Application Shutdown Process. Please avoid this behaviour. Could be referenced in a ApplicationQuit/OnDestroy/OnDisable of a MonoBehaviour. " );
					if ( _instance.gameObject.transform.parent != null )
					{
						var parent =  _instance.gameObject.transform.parent;
						GameObject.DestroyImmediate( _instance.gameObject );
						if ( parent != null && parent.childCount == 0 )
							GameObject.DestroyImmediate( parent.gameObject );
					}
					_instance =  null;
				}
			}
		}
		static RAIISingleton()
		{
			if ( Application.isEditor )
				playmodeStateChangedWrapper((playmodeChangedMethod,del,d)=> playmodeChangedMethod.SetValue( null, System.Delegate.Combine( del, d ) ));
		}

		static void playmodeStateChangedWrapper(System.Action<System.Reflection.FieldInfo,System.Delegate,System.Delegate> strategy )
		{
			var editorApplicationType = Types.GetType( "UnityEditor.EditorApplication", "UnityEditor" );
			var playmodeChangedMethod = editorApplicationType.GetField( "playmodeStateChanged", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static );
			if ( playmodeChangedMethod != null )
			{
				var del = playmodeChangedMethod.GetValue( null ) as System.Delegate;
				var callback = typeof( RAIISingleton<T> ).GetMethod( "ChangeCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static );
				var d = System.Delegate.CreateDelegate( playmodeChangedMethod.FieldType, null, callback );
				strategy(playmodeChangedMethod,del,d);
			}
		}

		static bool IsPlayingEditor
		{
			get
			{
				return (bool)Types.GetType( "UnityEditor.EditorApplication", "UnityEditor" ).GetProperty("isPlaying", System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public).GetValue(null,new object[]{});
			}
		}
		#endregion
	}
}