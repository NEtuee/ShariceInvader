using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControll {
	public Dictionary<string, Sprite[]> animations = new Dictionary<string, Sprite[]>();

	public bool isEnd{get{return _animationEnd;}}
	public string currAni{get{return _currAniName;}}

	protected float _timer = 0f;
	protected float _fps = 12f;
	protected int _aniPos = 0;
	protected int _aniLen = 0;
	protected bool _loop = false;
	protected bool _animationEnd = false;
	protected Sprite[] _currAni;
	protected string _currAniName = "";

	public void SetFps(float value)
	{
		_fps = value;
		_timer = 0f;
	}

	public void InitValue(bool lp)
	{
		_loop = lp;
		_aniPos = 0;
		_timer = 0f;
		_animationEnd = false;
	}

	public void SetAnimation(Sprite[] sprites)
	{
		_currAni = sprites;
		_aniLen = _currAni.Length;
	}

	public void ChangeAni(string name, bool lp)
	{
		if(animations.ContainsKey(name))
		{
			if(_currAni == animations[name] && lp)
				return;
				
			SetAnimation(animations[name]);
			_currAniName = name;
		}
		else
		{
			_currAni = null;
			_currAniName = "";
		}

		InitValue(lp);
	}

	public void AddAnimation(string name, string path)
	{
		Sprite[] sprites = ResourceManager.GetInstance().GetSpriteSet(path);
		if(sprites == null)
			return;
		
		animations.Add(name, sprites);
	}

	public int AnimationProgress(ref SpriteRenderer spr, float deltaTime) //고치삼
	{
		if(_currAni == null || _animationEnd)
			return -1;
		
		_timer += deltaTime * _fps;
		_aniPos = (int)(_timer);

		if(_aniPos >= _aniLen)
		{
			if(_loop)
			{
				_aniPos = 0;
				_timer = 0f;
			}
			else
			{
				_animationEnd = true;
				_aniPos = _currAni.Length - 1;
			}
		}

		// try
		// {
			spr.sprite = _currAni[_aniPos];
		// }
		// catch(System.Exception e)
		// {
		// 	Debug.Log(_timer);
		// 	Debug.Log(_aniPos);
		// 	Debug.Log(e.Message);
		// }

		return _aniPos;
	}

}