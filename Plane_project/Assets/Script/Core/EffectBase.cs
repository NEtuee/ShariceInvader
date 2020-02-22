using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : Drawable {

	AnimationControll ani;
	ObjectBase target;

	private Vector3 _addPoint;

	Color _lerpStart;
	Color _lerpEnd;

	bool _colorLerp = false;

	private float timer = 0f;
	private float _existsTime = 0f;
	public override void firstSetting()
	{
		base.firstSetting();
		ani = new AnimationControll();

		_sprRenderer.sortingOrder = -1;
	}

	public EffectBase Active(Vector2 pos, Sprite[] sprites, bool loop = false,ObjectBase t = null)
	{
		_eulerAngle = 0f;
		_direction = Vector3.zero;
		_speed = 0f;
		_scale = new Vector3(1f,1f,1f);
		_addPoint = Vector3.zero;

		_position = pos;
		ani.SetAnimation(sprites);
		ani.InitValue(loop);
		ani.SetFps(12f);

		SetSortingOrder(-1);

		UpdateTransform();
		SetActive(true);

		target = t;

		timer = 0f;

		_sprRenderer.color = Color.white;
		_colorLerp = false;

		return this;
	}

	public EffectBase SetFps(float fps){ani.SetFps(fps); return this;}
	public EffectBase SetAddPoint(Vector3 value) {_addPoint = value; return this;}

	public EffectBase Active(Vector2 pos, Sprite sprite, float time, ObjectBase t = null)
	{
		_eulerAngle = 0f;
		_direction = Vector3.zero;
		_speed = 0f;
		_scale = new Vector3(1f,1f,1f);

		_position = pos;
		// ani.SetAnimation(sprites);
		// ani.InitValue(loop);
		ani.ChangeAni("",false);
		_sprRenderer.sprite = sprite;

		SetSortingOrder(-1);

		UpdateTransform();
		SetActive(true);

		target = t;

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
		ani.AnimationProgress(ref _sprRenderer,deltaTime);
		Move(deltaTime);

		if(target != null)
			_position = target.position + _addPoint;

		if(ani.isEnd)
		{
			SetActive(false);
		}

		if(timer != 0f)
		{
			timer -= deltaTime;

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
	}

	public override void release()
	{
		
	}

	public void Active()
	{
		
	}
}
