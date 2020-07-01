using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager> {

	public class SoundRequestItem
	{
		public string path;
		public int random;
		public float volumeRatio;
	}

	public bool bgm = true;
	public bool se = true;

	public float masterVol = 1f;
	public float seVol = 1f;
	public float bgmVol = 1f;

	public GameObject audioBase;
	private Define.SimpleCache<AudioSource> _audioCache;
	private List<SoundRequestItem> _playList = new List<SoundRequestItem>();
	private Queue<SoundRequestItem> _requestCache = new Queue<SoundRequestItem>();
	private bool _listCheck = false;

	private AudioSource _loop;
	private AudioSource _scoreGet;

	public void Awake()
	{
		if(instance != null)
		{
			Destroy(this);
			return;
		}
		
		SetSingleton(this);
		
        _audioCache = new Define.SimpleCache<AudioSource>(audioBase,delegate{});
		_audioCache.SetParent(transform);
		_audioCache.CreateObject(30);

		DontDestroyOnLoad(this);
	}

	public void Update()
	{
		if(_listCheck)
		{
			foreach(var request in _playList)
			{
				Debug.Log(request.path);
				Play(request.path,false,request.random,request.volumeRatio);
				_requestCache.Enqueue(request);
			}

			_playList.Clear();
			_listCheck = false;
		}

		if(_loop != null)
		{
			_loop.mute = !bgm;
		}

		_audioCache.Loop((audio)=>{
			var scale = Timer.timeScale;
			scale = scale <= 0.1f ? 0.1f : scale;
			audio.pitch = scale;
			if(!audio.isPlaying)
				audio.gameObject.SetActive(false);
		});
	}

	public void StopLoop()
	{
		if(_loop != null)
		{
			_loop.Stop();
			_loop = null;
		}
	}

	public void PlayRequest(string path, int randomPath = -1, float volRatio = 1f)
	{
		if(!se)
			return;
	
		foreach(var play in _playList)
		{
			if(play.path == path)
			{
				if(play.volumeRatio < volRatio)
					play.volumeRatio = volRatio;
					
				return;
			}
		}

		var item = GetRequestCache();
		
		_listCheck = true;
		item.path = path;
		item.random = randomPath;
		item.volumeRatio = volRatio;

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

	public void SetVolume(float value)
	{
		bgmVol = value;
	}

	public void LoadBGM()
	{
		// ResourceManager.GetInstance().GetAudioClip("Game");
		// ResourceManager.GetInstance().GetAudioClip("Boss");
		// ResourceManager.GetInstance().GetAudioClip("Title");

	}

	public AudioSource Play(string path, bool loop, int randomPath = -1, float volRatio = 1f)
	{
		if(!se && !loop)
			return null;

		if(randomPath > 0)
			path += Random.Range(0,randomPath);

		AudioSource audio = _audioCache.ActiveObject();

		volRatio = volRatio >= 1f ? 1f : (volRatio <= 0f ? 0f : volRatio);

		audio.gameObject.SetActive(true);
		audio.clip = ResourceManager.GetInstance().GetAudioClip(path);
		audio.volume = (seVol * masterVol) * volRatio;
		audio.loop = loop;
		audio.mute = loop ? !bgm : !se;
		audio.pitch = Timer.timeScale;
		audio.Stop();
		audio.Play();

		if(audio.clip == null)
			Debug.Log(path);

		if(loop)
		{
			if(_loop != null)
				_loop.gameObject.SetActive(false);
			_loop = audio;
			audio.volume = bgmVol * masterVol;
		}

		return audio;
	}

	public void PlayScoreGagueSE()
	{
		AudioSource audio = _audioCache.ActiveObject();

		audio.gameObject.SetActive(true);
		audio.clip = ResourceManager.GetInstance().GetAudioClip("score_one");
		audio.volume = seVol * masterVol;
		audio.loop = true;
		audio.mute = !se;
		audio.Stop();
		audio.Play();

		_scoreGet = audio;
	}

	public void StopScoreGagueSE()
	{
		if(_scoreGet != null)
		{
			_scoreGet.Stop();
			_scoreGet = null;
		}
	}

}