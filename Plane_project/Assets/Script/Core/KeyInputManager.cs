using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyInputManager {
	private static string _keyDown = "";
	private static string _keyPressed = "";
	private static string _keyUp = "";

	// private static string _keyDown = "";
	// private static string _keyPressed = "";
	// private static string _keyUp = "";

	public static void KeyUpdate()
	{
		KeyCheck(KeyCode.A,"|LEFT");
		KeyCheck(KeyCode.W,"|UP");
		KeyCheck(KeyCode.D,"|RIGHT");
		KeyCheck(KeyCode.S,"|DOWN");
		KeyCheck(KeyCode.Mouse0,"|ATTACK");
	}

	public static void KeyInit()
	{
		_keyDown = "";
		_keyPressed = "";
		_keyPressed = "";
	}

	public static bool KeyDown(string key){return _keyDown.Contains(key);}
	public static bool KeyPressed(string key){return _keyPressed.Contains(key);}
	public static bool KeyUp(string key){return _keyUp.Contains(key);}

	public static void KeyCheck(KeyCode key, string keyName)
	{
		if(Input.GetKeyDown(key))
			_keyDown += keyName;
		else if(Input.GetKey(key))
		{
			_keyPressed += keyName;
		}
		else if(Input.GetKeyUp(key))
			_keyPressed += keyName;
	}
	
}
