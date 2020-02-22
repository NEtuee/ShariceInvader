using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
	private static T instance;

	public static T GetInstance()
	{
		if(instance == null)
		{
			instance = new T();
		}
		return instance;
	}

	public static void DeleteSingleton()
	{
		instance = default(T);
	}
}
