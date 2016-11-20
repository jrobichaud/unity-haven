using UnityEngine;
using System.Collections;

public class DemoSingletons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DemoSceneSingleton.Foo();
		DemoSceneSingleton.Foo();
		DemoRAIISingleton.Foo();
		DemoRAIISingleton.Foo();
	}
}
