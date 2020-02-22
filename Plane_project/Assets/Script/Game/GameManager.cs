using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>, Define.IManager {

	public Player player;

	public void firstSetting()
	{
		SetSingleton(this);
	}

	public void progress(float deltaTime)
	{

	}

	public void lateProgress(float deltaTime)
	{

	}
}
