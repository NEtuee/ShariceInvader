using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllEx
{
    public struct AnimationKey
    {
        public float duration;
        public Sprite sprite;
    };

    public Dictionary<string, AnimationKey[]> animations = new Dictionary<string, AnimationKey[]>();

	public bool isEnd{get{return _animationEnd;}}
	public string currAni{get{return _currAniName;}}

	protected float _timer = 0f;
	protected int _aniPos = 0;
	protected int _aniLen = 0;
	protected bool _loop = false;
	protected bool _animationEnd = false;
	protected AnimationKey[] _currAni;
	protected string _currAniName = "";

    public SpriteRenderer _sprRenderer;


	public void InitValue(bool lp)
	{
		_loop = lp;
		_aniPos = 0;
		_timer = 0f;
		_animationEnd = false;
	}

	public void SetAnimation(AnimationKey[] ani)
	{
		_currAni = ani;
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

        _sprRenderer.sprite = _currAni[0].sprite;

		InitValue(lp);
	}

    public void ClearAnimationList()
    {
		// Debug.Log("start");
        // foreach(var ani in animations)
        // {
		// 	Debug.Log("key null");
        //     animations[ani.Key] = null;
		// 	Debug.Log("complete");
        // }

        animations.Clear();
    }

	public void AddAnimation(string name, string path)
	{
		Sprite[] sprites = ResourceManager.GetInstance().GetSpriteSet(path);
		if(sprites == null)
			return;

        string file = path.Substring(path.LastIndexOf('/'));
        string pathName =  IOManager.PathForDocumentsFile("Assets\\Resources\\Sprites\\SpriteSet\\") + path + file + ".shrani";

        string[] data = IOManager.ReadStringFromFile(pathName);

        if(data == null)
        {
            CreateAnimationRef(pathName,0.08333f,sprites.Length);
        }

        AnimationKey[] key = new AnimationKey[sprites.Length];

        for(int i = 0; i < sprites.Length; ++i)
        {
            float t = 0.08333f;

            if(data != null)
            {
                t = float.Parse(data[i]);
            }

            key[i].duration = t;
            key[i].sprite = sprites[i];
        }

        animations.Add(name, key);
	}

	public int AnimationProgress(float deltaTime) //고치삼
	{
		if(_currAni == null || _animationEnd)
			return -1;
		
        _timer += deltaTime;

        if(_timer >= _currAni[_aniPos].duration)
        {
            _timer -= _currAni[_aniPos].duration;
            ++_aniPos;

            if(_aniPos >= _currAni.Length)
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

            _sprRenderer.sprite = _currAni[_aniPos].sprite;
        }

		return _aniPos;
	}

    public void CreateAnimationRef(string name, float time, int count)
    {
        List<string> s = new List<string>();

        string t = time.ToString();
        for(int i = 0; i < count; ++i)
        {
            s.Add(t);
        }

        IOManager.WriteStringToFile_NoMark(s.ToArray(),name,false);
    }

    public AnimationControllEx(SpriteRenderer spr){_sprRenderer = spr;}

}
