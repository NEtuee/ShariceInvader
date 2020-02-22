using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonBase : MonoBehaviour {

	public Sprite upSprite;
	public Sprite downSprite;
	public Sprite toggleDownSprite;
	public UnityEvent buttonDown = new UnityEvent();
	public UnityEvent buttonUp = new UnityEvent();
	public UnityEvent buttonToggle = new UnityEvent();

	public bool toggleObject = false;
	public bool buttonLock = false;//{get{return buttonLock;} set{if(buttonLock) sprRenderer.sprite = downSprite;}}//} = false;

	public bool isDown{get{return _isDown;}}
	public bool toggleClick{get{return _toggleClick;}}


	[HideInInspector]
	public Collider2D coll;
	[HideInInspector]
	public SpriteRenderer sprRenderer;

	private bool _isDown = false;
	private bool _toggleClick = false;

	public void firstSetting()
	{
		coll = GetComponent<Collider2D>();
		sprRenderer = GetComponent<SpriteRenderer>();

		// buttonDown.AddListener(()=>{sprRenderer.sprite = downSprite;});
		// buttonUp.AddListener(()=>{sprRenderer.sprite = upSprite;});
	}
	public void init()
	{
		sprRenderer.sprite = upSprite;
	}

	public bool ButtonDownCheck(Vector2 point,TouchInfo touch)
	{
		if(buttonLock)
			return false;

		if(toggleObject)
		{
			if(!_isDown && OverlapCheck(point))
			{
				_isDown = true;
				touch.touchSomething = true;
				if(toggleDownSprite != null)
				{
				//	SoundManager.instance.Play("Button_press",false);
					sprRenderer.sprite = toggleDownSprite;
				}
				// if(!_toggleClick)
				// {
				// 	sprRenderer.sprite = downSprite;
				// }
				return true;
			}
		}
		else
		{
			if(!_isDown && OverlapCheck(point))
			{
				_isDown = true;
				touch.touchSomething = true;
				sprRenderer.sprite = downSprite;
				ButtonDown();

			//	SoundManager.instance.Play("Button_press",false);

				return true;
			}
		}

		return false;
	}

	public void ToggleUp()
	{
		sprRenderer.sprite = upSprite;
		_toggleClick = false;
		buttonUp.Invoke();
	}

	public void ToggleDown()
	{
		sprRenderer.sprite = downSprite;
		_toggleClick = true;
		buttonToggle.Invoke();
	}

	public void ButtonUpCheck(Vector2 point)
	{
		if(buttonLock)
			return;

		if(toggleObject)
		{
			if(_isDown)
			{
				_isDown = false;
				if(OverlapCheck(point))
				{
					if(!_toggleClick)
					{
						ToggleDown();
			//			SoundManager.instance.Play("Button_press",false);
					}
					else
					{
						ToggleUp();
			//			SoundManager.instance.Play("Button_Up",false);
					}
				}
				else
				{
					sprRenderer.sprite = upSprite;
					_toggleClick = false;
				}
			}
		}
		else
		{
//			Debug.Log("buttonCheck");
			if(_isDown)
			{
				_isDown = false;
				sprRenderer.sprite = upSprite;
				if(OverlapCheck(point))
					ButtonUp();
				
		//		SoundManager.instance.Play("Button_Up",false);
			}
		}
	}

	public bool OverlapCheck(Vector2 point) {return coll.OverlapPoint(point);}

	public void ButtonLock(bool value)
	{
		buttonLock = value;
		if(buttonLock)
			sprRenderer.sprite = downSprite;
		else
			sprRenderer.sprite = upSprite;
	}

	public void ButtonDown()
	{
		buttonDown.Invoke();
	}
	public void ButtonUp()
	{
		buttonUp.Invoke();
	}

}
