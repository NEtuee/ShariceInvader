using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour {

	public static T instance;
	public void SetSingleton(T t){instance = default(T); instance = t;}
}

