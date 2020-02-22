using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGroupEx : MonoBehaviour {

	public ButtonBase[] buttons;

	public bool group = true;
	private TouchDetection _touch;

	private ButtonBase _currButton;

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

	public void SetButtonToggle(int num)
	{
		if(_currButton != null && group)
			_currButton.ToggleUp();
		_currButton = buttons[num];
		_currButton.ToggleDown();
	}

	public void progress(float deltaTime)
	{
		for(int i = 0; i < TouchDetection.maxTouchCount; ++i)
		{
			foreach(var button in buttons)
			{
				if(_touch.touchs[i].state == Define.TouchState.Began)
				{
					if(!button.toggleClick || !group)
						button.ButtonDownCheck(_touch.GetTouchWorldPos(i,0),_touch.touchs[i]);
				}
				else if(_touch.touchs[i].state == Define.TouchState.End)
				{
					button.ButtonUpCheck(_touch.GetTouchWorldPos(i,1));
					if(button.toggleClick && group)
					{
						if(_currButton != button)
						{
							if(_currButton != null)
								_currButton.ToggleUp();
							_currButton = button;
						}
					}
				}

			}
		}

	}

	public void release()
	{

	}
}