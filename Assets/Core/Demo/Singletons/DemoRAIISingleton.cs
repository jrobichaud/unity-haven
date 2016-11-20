using UnityEngine;
using CoreEngine;

public class DemoRAIISingleton : RAIISingleton<DemoRAIISingleton>
{
	int i = 0;
	// Using public static methods have better encapsulation for these implementations
	public static void Foo()
	{
		// This check prevents the Singleton to be created while Unity Editor is quitting play or when switching scene. It causes bad object leaks.
		if (IsAvailable)
		{
			//Call to Instance will create the instance
			Debug.Log("DemoRAIISceneSingleton Bar " + Instance.i++);
		}
	}
}
