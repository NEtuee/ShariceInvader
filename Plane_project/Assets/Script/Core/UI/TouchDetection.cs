using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDetection : SingletonMono<TouchDetection> { //나중에 고칠것 지금 비효율적임

	public Camera baseCam;
	public TouchInfo[] touchs;
	public static int maxTouchCount = 3;

	public void Awake()
	{
		SetSingleton(this);
		touchs = new TouchInfo[maxTouchCount];

		for(int i = 0; i < maxTouchCount; ++i)
			touchs[i] = new TouchInfo();
	}

	public void Update()
	{
		TouchUpdate();
		//DragCheck();
	}

	public void Active()
	{
		enabled = true;
	}

	public void TouchUpdate()
	{
		int i = 0;
		for(i = 0; i < maxTouchCount; ++i)
		{
			touchs[i].BeforeUpdate();
		}

		#if UNITY_STANDALONE_WIN
			InputCheck_Win();
		#elif UNITY_EDITOR
			InputCheck_Win();
		#elif UNITY_ANDROID
			InputCheck_Android();
		#endif


		for(i = 0; i < maxTouchCount; ++i)
		{
			touchs[i].LateUpdate();
		}
	}

	public TouchInfo FindTouchInfo(int count)
	{
		for(int i = 0; i < maxTouchCount; ++i)
		{
			if(touchs[i].touchCount == count)
				return touchs[i];
		}

		return null;
	}

	public TouchInfo GetEmptyTouch()
	{
		for(int i = 0; i < maxTouchCount; ++i)
		{
			if(touchs[i].state == Define.TouchState.Began &&
				!touchs[i].touchSomething)
				return touchs[i];
		}

		return null;
	}

	public void InputCheck_Win()
	{
		if(Input.GetMouseButtonDown(0))
		{
			FindTouchInfo(-1).TouchStart(0);
		}
		else if(Input.GetMouseButton(0))
		{
			FindTouchInfo(0).TouchMoved(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(0))
		{
			FindTouchInfo(0).TouchUp();
		}
	}

	public void InputCheck_Android()
	{
		if(Input.touchCount > 0)
		{
			for(int i = 0; i < Input.touchCount; ++i)
			{
				Touch touchs = Input.GetTouch(i);

				if(touchs.phase == TouchPhase.Began)
				{
					TouchInfo info = FindTouchInfo(-1);

					if(info != null)
						info.TouchStart(i);
				}
				else if(touchs.phase == TouchPhase.Moved)
				{
					FindTouchInfo(i).TouchMoved(touchs.position);
				}
				else if(touchs.phase == TouchPhase.Ended)
				{
					FindTouchInfo(i).TouchUp();
				}
			}
		}
	}

	public Vector2 GetTouchWorldPos(int count,int target) //0 = touched, 1 = up, 2 = moved
	{
		Vector3 pos = PointToWorld(target == 0 ? touchs[count].touchedPos : (target == 1 ? touchs[count].upPos : touchs[count].movedPos));
		pos.z = 0f;
		return pos;
	}

	public Vector2 PointToWorld(Vector2 pos)
	{
		return baseCam.ScreenToWorldPoint(pos);
	}

	// public void TouchCancel()
	// {
	// 	state = Define.TouchState.None;
	// 	_touchedPos = Input.mousePosition;
	// 	_movedPos = Input.mousePosition;
	// }
}
