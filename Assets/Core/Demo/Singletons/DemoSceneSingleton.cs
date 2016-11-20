using UnityEngine;
using CoreEngine;

public class DemoSceneSingleton : SceneSingleton<DemoSceneSingleton>
{
	int i = 0;
	// Using public static methods have better encapsulation for these implementations
	public static void Foo()
	{
		// This check prevents a null reference exception when the scene singleton is not present
		if (IsAvailable) 
		{
			// Instance does not create anything
			Debug.Log("DemoSceneSingleton Bar " + Instance.i++);
		}
	}
}
