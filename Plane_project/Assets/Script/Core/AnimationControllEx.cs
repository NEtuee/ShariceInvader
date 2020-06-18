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

	public static Dictionary<string,AnimationKey[]> loadedAnimations = new Dictionary<string,AnimationKey[]>();

    public Dictionary<string, AnimationKey[]> animations = new Dictionary<string, AnimationKey[]>();
	public Dictionary<string, string> aniOriginPath = new Dictionary<string, string>();

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


	public void InitValue(bool lp, int pos = 0, float timer = 0f)
	{
		_loop = lp;
		_aniPos = pos;
		_timer = timer;
		_animationEnd = false;
	}

	public void SetAnimation(AnimationKey[] ani)
	{
		_currAni = ani;
		_aniLen = _currAni.Length;
	}

	public void SetAnimation(string path)
	{
		if(loadedAnimations.ContainsKey(path))
		{
			SetAnimation(loadedAnimations[path]);
		}
	}


	public bool AnimationExist(string name)
	{
		if(animations.ContainsKey(name))
		{
			if(animations[name] == null)
				return false;
			
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool ChangeAniSync(string name, bool lp)
	{
		return ChangeAni(name,lp,true,_aniPos,_timer);
	}

	public bool ChangeAni(string name, bool lp, bool overlapCheck = true, int aniPos = 0, float timer = 0f)
	{
		if(animations.ContainsKey(name))
		{
			if(_currAni == animations[name] && lp && overlapCheck)
			{
				return false;
			}	
			SetAnimation(animations[name]);
			_currAniName = name;
		}
		else
		{
			_currAni = null;
			_currAniName = "";

			Debug.Log("ani does not exists");

			_sprRenderer.sprite = null;
			return false;
		}

        _sprRenderer.sprite = _currAni[aniPos].sprite;

		InitValue(lp,aniPos,timer);

		return true;
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

	public void Stop()
	{
		_animationEnd = true;
	}

	public void CopyAnimation(string copy, string target)
	{
		if(!animations.ContainsKey(copy) || (!loadedAnimations.ContainsKey(target) && target != ""))
		{
			Debug.Log(" : animation does not loaded!!! : " + target);
			return;
		}

		if(target == "")
			animations[copy] = null;
		else
			animations[copy] = loadedAnimations[target];
	}

	public void CopyAnimation(string copy, AnimationKey[] keys)
	{
		if(!animations.ContainsKey(copy))
		{
			Debug.Log("animation does not loaded!!!");
			return;
		}

		animations[copy] = keys;
	}
	
	public void AddEmptyAnimation(string name)
	{
		animations.Add(name,null);
	}

	public void AddAnimation(string name, string path)
	{
		AnimationKey[] key = LoadAnimationToPath(path);

		if (key == null)
		{
			Debug.Log("animation Load Error : " + path);

			return;
		}
		else if(animations.ContainsKey(name))
		{
			Debug.Log("Same Animation already Exist " + name);

			return;
		}

		aniOriginPath.Add(name,path);
        animations.Add(name, key);
	}

	public AnimationKey[] LoadAnimationToPath(string path)
	{
		AnimationKey[] key;
		if(!loadedAnimations.ContainsKey(path))
		{
			Sprite[] sprites = ResourceManager.GetInstance().GetSpriteSet(path);
			if(sprites == null)
				return null;

        	string file = path;// path.Substring(path.LastIndexOf('/'));
			string savePath = path;
			if(file.Contains("/"))
				file = file.Substring(path.LastIndexOf('/'));
			else
			{
				savePath += "/";
			}
				
        	string pathName = "";

			pathName =  "Sprites/" + savePath + file + "_Ani";

        	string[] data = ResourceManager.GetInstance().GetSaveData(pathName);

        	if(data == null)
        	{
        	    CreateAnimationRef(pathName,0.08333f,sprites.Length);
        	}

        	key = new AnimationKey[sprites.Length];

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

			loadedAnimations.Add(path,key);
		}
		else
			key = loadedAnimations[path];

		return key;
	}

	public static void LoadAnimation(string path)
	{
		AnimationKey[] key;

		if(!loadedAnimations.ContainsKey(path))
		{
			Sprite[] sprites = ResourceManager.GetInstance().GetSpriteSet(path);
			if(sprites == null)
				return;

        	string file = path.Substring(path.LastIndexOf('/'));
        	string pathName =  "Sprites/" + path + file + "_Ani";

        	string[] data = ResourceManager.GetInstance().GetSaveData(pathName);

        	if(data == null)
        	{
				Debug.Log(path + " : " + "??");
				Debug.Log(pathName);
        	    CreateAnimationRef(pathName,0.08333f,sprites.Length);
        	}

        	key = new AnimationKey[sprites.Length];

        	for(int i = 0; i < sprites.Length; ++i)
        	{
        	    float t = 0.063f;

        	    if(data != null)
        	    {
        	        t = float.Parse(data[i]);
        	    }

        	    key[i].duration = t;
        	    key[i].sprite = sprites[i];
        	}

			loadedAnimations.Add(path,key);
		}
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

           SetAnimationSprite(_aniPos);
        }

		return _aniPos;
	}

	public void SetAnimationSprite(int pos)
	{
		if(_currAni != null)
			_sprRenderer.sprite = _currAni[pos].sprite;
		else
			_sprRenderer.sprite = null;
	}

	public void SetSpriteNull() {_sprRenderer.sprite = null;}

    public static void CreateAnimationRef(string n, float time, int count)
    {
        List<string> s = new List<string>();

        string t = time.ToString();
        for(int i = 0; i < count; ++i)
        {
            s.Add(t);
        }

        IOManager.WriteStringToFile_NoMark(s.ToArray(),"Assets/Resources/" + n + ".txt",false);
    }

    public AnimationControllEx(SpriteRenderer spr){_sprRenderer = spr;}

}
