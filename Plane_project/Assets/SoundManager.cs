using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager> {

	public class SoundRequestItem
	{
		public string path;
		public int random;
		public float volumeRatio;
		public float timer = 0f;
		public bool slowMo = true;
	}

	public bool bgm = true;
	public bool se = true;

	public float masterVol = 1f;
	public float seVol = 1f;
	public float bgmVol = 1f;

	public GameObject audioBase;
	private Define.SimpleCache<SoundOption> _audioCache;
	private List<SoundRequestItem> _playList = new List<SoundRequestItem>();
	private Queue<SoundRequestItem> _requestCache = new Queue<SoundRequestItem>();
	private bool _listCheck = false;
	private bool _bgmFade = false;

	private float _targetVol = 0f;

	private SoundOption _loop;
	private AudioClip _changeBGM;
	private SoundOption _scoreGet;

	public void Awake()
	{
		if(instance != null)
		{
			Destroy(this);
			return;
		}
		
		SetSingleton(this);
		
        _audioCache = new Define.SimpleCache<SoundOption>(audioBase,delegate{});
		_audioCache.SetParent(transform);
		_audioCache.CreateObject(30);

		DontDestroyOnLoad(this);

		LoadBGM();
	}

	public void Update()
	{
		if(_listCheck)
		{
			_listCheck = false;

			for(int i = 0; i < _playList.Count;)
			{
				_playList[i].timer -= Time.deltaTime;
				if(_playList[i].timer <= 0f)
				{
					Play(_playList[i].path,false,_playList[i].random,
							_playList[i].volumeRatio,_playList[i].slowMo);
					
					_playList.RemoveAt(i);
				}
				else
				{
					_listCheck = true;
					++i;
				}
			}

			// foreach(var request in _playList)
			// {
			// 	request.timer -= Time.deltaTime;
			// 	if(request.timer <= 0f)
			// 	{
			// 		Play(request.path,false,request.random,request.volumeRatio,request.slowMo);
			// 	}
			// 	else
			// 	{
			// 		_listCheck = true;
			// 	}

			// 	_requestCache.Enqueue(request);
			// }

			//_playList.Clear();
		}

		_audioCache.Loop((audio)=>{
			if(audio.slowMo)
			{
				var scale = Timer.timeScale;
				scale = scale <= 0.1f ? 0.1f : scale;
				audio.mainAudioItem.pitch = Mathf.Lerp(audio.mainAudioItem.pitch,scale,0.1f);
			}

			if(audio.type == SoundOption.SoundType.BackgroundMusic && !_bgmFade)
			{
				audio.mainAudioItem.volume = (bgmVol * masterVol) * audio.volRatio;
			}
			else if(audio.type == SoundOption.SoundType.SoundEffect)
			{
				audio.mainAudioItem.volume = (seVol * masterVol) * audio.volRatio;
			}

			if(!audio.mainAudioItem.isPlaying && audio.type == SoundOption.SoundType.SoundEffect)
				audio.gameObject.SetActive(false);
		});

		if(_loop != null)
		{
			_loop.mainAudioItem.mute = !bgm;

			if(_bgmFade)
			{
				if(_changeBGM != null)
				{
					_loop.mainAudioItem.volume -= Time.deltaTime * 1f;
					if(_loop.mainAudioItem.volume <= 0f)
					{
						_loop.mainAudioItem.volume = 0f;
						_loop.mainAudioItem.clip = _changeBGM;
						_loop.mainAudioItem.Play();
						_changeBGM = null;
					}
				}
				else
				{
					_loop.mainAudioItem.volume += Time.deltaTime * 1f;
					float target = (bgmVol * masterVol) * _loop.volRatio;
					if(_loop.mainAudioItem.volume >= target)
					{
						_loop.mainAudioItem.volume = target;
						_bgmFade = false;
					}
				}
				
			}
		}
	}

	public void StopLoop()
	{
		if(_loop != null)
		{
			_loop.mainAudioItem.Stop();
			_loop = null;
		}
	}

	public void PlayRequest(string path, int randomPath = -1, float volRatio = 1f, bool slow = true, float timer = 0f)
	{
		if(!se)
			return;
	
		if(timer == 0f)
		{
			foreach(var play in _playList)
			{
				if(play.path == path)
				{
					if(play.volumeRatio < volRatio)
						play.volumeRatio = volRatio;

					return;
				}
			}
		}

		var item = GetRequestCache();
		
		_listCheck = true;
		item.path = path;
		item.random = randomPath;
		item.volumeRatio = volRatio;
		item.slowMo = slow;
		item.timer = timer;

		_playList.Add(item);
	}

	public SoundRequestItem GetRequestCache()
	{
		SoundRequestItem item = null;
		if(_requestCache.Count != 0)
		{
			item = _requestCache.Dequeue();
		}
		else
		{
			item = new SoundRequestItem();
		}

		return item;
	}

	public void SetBGMVolume(float value)
	{
		bgmVol = value;
	}

	public void SetSEVolume(float value)
	{
		seVol = value;
	}

	public void LoadBGM()
	{
		ResourceManager.GetInstance().GetAudioClip("BGM/PracticeTheme");
		// ResourceManager.GetInstance().GetAudioClip("Boss");
		// ResourceManager.GetInstance().GetAudioClip("Title");

	}

	public SoundOption PlayBGM(string path, bool fade, float volRatio = 1f, bool slow = true)
	{
		if(_loop == null)
		{
			_loop = _audioCache.ActiveObject();
		}
		else
		{
			if(_loop.path == path)
				return _loop;
			
			if(fade)
			{
				_changeBGM = ResourceManager.GetInstance().GetAudioClip(path);
				_loop.path = path;
				_bgmFade = true;

				return _loop;
			}
		}

		_loop.path = path;

		volRatio = volRatio >= 1f ? 1f : (volRatio <= 0f ? 0f : volRatio);

		_loop.gameObject.SetActive(true);
		_loop.mainAudioItem.clip = ResourceManager.GetInstance().GetAudioClip(path);
		_loop.volRatio = volRatio;
		_targetVol = (bgmVol * masterVol) * volRatio;
		_loop.mainAudioItem.loop = true;
		_loop.mainAudioItem.mute = !bgm;
		_loop.mainAudioItem.pitch = slow ? Timer.timeScale : 1f;
		_loop.mainAudioItem.Stop();
		_loop.mainAudioItem.Play();

		_loop.type = SoundOption.SoundType.BackgroundMusic;

		_loop.slowMo = slow;
		_loop.mainAudioItem.volume = fade ? 0f : _targetVol;
		_bgmFade = fade;

		return _loop;
	}

	public SoundOption Play(string path, bool loop, int randomPath = -1, float volRatio = 1f, bool slow = true)
	{
		if(!se && !loop)
			return null;

		if(randomPath > 0)
			path += Random.Range(0,randomPath);

		SoundOption audio = _audioCache.ActiveObject();

		volRatio = volRatio >= 1f ? 1f : (volRatio <= 0f ? 0f : volRatio);

		audio.gameObject.SetActive(true);
		audio.volRatio = volRatio;
		audio.mainAudioItem.clip = ResourceManager.GetInstance().GetAudioClip(path);
		audio.mainAudioItem.volume = (seVol * masterVol) * volRatio;
		audio.mainAudioItem.loop = loop;
		//audio.mainAudioItem.mute = loop ? !bgm : !se;
		audio.mainAudioItem.pitch = slow ? Timer.timeScale : 1f;
		audio.mainAudioItem.Stop();
		audio.mainAudioItem.Play();

		audio.slowMo = slow;

		if(audio.mainAudioItem.clip == null)
			Debug.Log(path);

		// if(loop)
		// {
		// 	if(_loop != null)
		// 		_loop.gameObject.SetActive(false);
		// 	_loop = audio;
		// 	audio.mainAudioItem.volume = bgmVol * masterVol;
		// }

		return audio;
	}

	public void PlayScoreGagueSE()
	{
		SoundOption audio = _audioCache.ActiveObject();

		audio.gameObject.SetActive(true);
		audio.mainAudioItem.clip = ResourceManager.GetInstance().GetAudioClip("score_one");
		audio.mainAudioItem.volume = seVol * masterVol;
		audio.mainAudioItem.loop = true;
		audio.mainAudioItem.mute = !se;
		audio.mainAudioItem.Stop();
		audio.mainAudioItem.Play();

		_scoreGet = audio;
	}

	public void StopScoreGagueSE()
	{
		if(_scoreGet != null)
		{
			_scoreGet.mainAudioItem.Stop();
			_scoreGet = null;
		}
	}

}