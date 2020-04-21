using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : Drawable {

	AnimationControllEx ani;
	ObjectBase target;

	private Vector3 _addPoint;

	Color _lerpStart;
	Color _lerpEnd;

	Action _apearEvent;

	bool _colorLerp = false;
	bool _realTimeProgress = false;
	bool _passiveDeactive = false;
	bool _singleSprite = false;
	bool _eventAction = false;
	bool _animationProgress = false;

	private float timer = 0f;
	private float _existsTime = 0f;
	private float _delayApear = 0f;
	public override void firstSetting()
	{
		base.firstSetting();
		ani = new AnimationControllEx(_sprRenderer);

		SetSortingOrder(-1);
	}

	public EffectBase Active(Vector2 pos, string path, int type, bool loop = false,ObjectBase t = null)
	{
		_eulerAngle = 0f;
		_direction = Vector3.zero;
		_speed = 0f;
		_scale = new Vector3(1f,1f,1f);
		_addPoint = Vector3.zero;

		_realTimeProgress = false;
		_passiveDeactive = false;
		_singleSprite = false;
		_eventAction = false;

		_position = pos;
		_animationProgress = true;
		ani.SetAnimation(ani.LoadAnimationToPath(path, type));
		ani.SetAnimationSprite(0);
		ani.InitValue(loop);

		SetSortingOrder(-1);

		UpdateTransform();
		SetActive(true);

		target = t;

		timer = 0f;
		_delayApear = 0f;

		_sprRenderer.color = Color.white;
		_colorLerp = false;

		return this;
	}

	//public EffectBase SetFps(float fps){ani.SetFps(fps); return this;}
	public EffectBase SetAddPoint(Vector3 value) {_addPoint = value; return this;}
	public EffectBase RealTimeProgress() {_realTimeProgress = true; return this;}
	public EffectBase PassiveDeactive() {_passiveDeactive = true; return this;}
	public EffectBase DelayApear(float time) {_delayApear = time; return this;}
	public EffectBase SetApearEvent(Action evt) {_eventAction = true; _apearEvent = evt; return this;}

	public EffectBase Active(Vector2 pos, Sprite sprite, float time, ObjectBase t = null)
	{
		_eulerAngle = 0f;
		_direction = Vector3.zero;
		_speed = 0f;
		_scale = new Vector3(1f,1f,1f);

		_position = pos;
		_sprRenderer.sprite = sprite;
		_animationProgress = false;

		_realTimeProgress = false;
		_passiveDeactive = false;
		_eventAction = false;
		_singleSprite = true;

		SetSortingOrder(-1);

		UpdateTransform();
		SetActive(true);

		target = t;
		_delayApear = 0f;

		timer = time;
		_existsTime = time;

		_sprRenderer.color = Color.white;
		_colorLerp = false;

		return this;
	}

	public EffectBase ColorLerp(Color start, Color end)
	{
		_colorLerp = true;
		_lerpStart = start;
		_lerpEnd = end;

		_sprRenderer.color = start;

		return this;
	}

	public EffectBase SetTarget(ObjectBase t)
	{
		target = t;

		return this;
	}

	public EffectBase SetTimer(float t)
	{
		timer = t;

		return this;
	}

	public override void initialize()
	{
		SetSortingOrder(-1);
	}

	public override void progress(float deltaTime)
	{
		float time = _realTimeProgress ? Timer.GetInstance().noneScaledDeltaTime : deltaTime;
		if(_delayApear != 0f)
		{
			sprRenderer.sprite = null;
			_delayApear -= time;

			if(_delayApear <= 0f)
			{
				if(_eventAction)
					_apearEvent();
				_delayApear = 0f;
			}
			else
			{
				return;
			}
		}


		if(_animationProgress)
			ani.AnimationProgress(time);
		Move(time);

		if(target != null)
		{
			if(target.deleted)
				SetActive(false);
			_position = target.position + _addPoint;
		}

		if(timer != 0f)
		{
			timer -= time;

			if(_colorLerp)
			{
				_sprRenderer.color = Color.Lerp(_lerpStart,_lerpEnd,(_existsTime - timer) / _existsTime);
			}

			if(timer <= 0f)
			{
				timer = 0f;
				SetActive(false);
			}
		}
		else if(ani.isEnd && !_passiveDeactive)
		{
			SetActive(false);
		}
	}

	public override void release()
	{
		
	}

	public void Active()
	{
		
	}
}
