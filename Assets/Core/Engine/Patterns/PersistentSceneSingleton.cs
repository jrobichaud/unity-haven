
namespace CoreEngine
{
	public abstract class PersistentSceneSingleton<T> : SceneSingleton<T> where T : PersistentSceneSingleton<T>
	{
		public override void Awake()
		{
			base.Awake();
			DontDestroyOnLoad( gameObject );
		}
	}
}