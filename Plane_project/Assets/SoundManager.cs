using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager> {

	public bool bgm = true;
	public bool se = true;

	public float masterVol = 1f;
	public float seVol = 1f;
	public float bgmVol = 1f;

	public GameObject audioBase;
	private Define.SimpleCache<AudioSource> _audioCache;
	private List<string> _playList = new List<string>();
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
				Play(request,false);
			}
			_playList.Clear();
			_listCheck = false;
		}

		if(_loop != null)
		{
			_loop.mute = !bgm;
		}

		_audioCache.Loop((audio)=>{
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

	public void PlayRequest(string path)
	{
		if(!se)
			return;
	
		if(!_playList.Contains(path))
		{
			_playList.Add(path);
			_listCheck = true;
		}
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

	public void Play(string path, bool loop)
	{
		if(!se && !loop)
			return;

		AudioSource audio = _audioCache.ActiveObject();

		audio.gameObject.SetActive(true);
		audio.clip = ResourceManager.GetInstance().GetAudioClip(path);
		audio.volume = seVol * masterVol;
		audio.loop = loop;
		audio.mute = loop ? !bgm : !se;
		audio.Stop();
		audio.Play();

		if(audio.clip == null)
			Debug.Log("Checkit");

		if(loop)
		{
			if(_loop != null)
				_loop.gameObject.SetActive(false);
			_loop = audio;
			audio.volume = bgmVol * masterVol;
		}
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