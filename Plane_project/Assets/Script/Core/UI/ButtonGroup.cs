using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonGroup : MonoBehaviour {

	
	public ButtonBase[] buttons;
	private TouchDetection _touch;

	public void firstSetting(TouchDetection t)
	{
		foreach(var button in buttons)
		{
			button.firstSetting();
			button.init();
		}

		_touch = t;
	}

	public void initialize()
	{
		foreach(var button in buttons)
		{
			button.init();
		}
	}

	public void progress(float deltaTime)
	{
		for(int i = 0; i < TouchDetection.maxTouchCount; ++i)
		{
			foreach(var button in buttons)
			{
				if(!button.gameObject.activeInHierarchy)
					continue;

				if(_touch.touchs[i].state == Define.TouchState.Began)
				{
					button.ButtonDownCheck(_touch.GetTouchWorldPos(i,0),_touch.touchs[i]);
				}
				else if(_touch.touchs[i].state == Define.TouchState.End)
				{
					button.ButtonUpCheck(_touch.GetTouchWorldPos(i,1));
				}
			}
		}

	}

	public void release()
	{

	}
}
