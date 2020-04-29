using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Timer {

	struct TimeScaleSet
	{
		public float scaledTime;
		public float time;
		public float timer;
		public bool lerp;

		public float Progress(float deltaTime)
		{
			timer += deltaTime;
			float timeScale = scaledTime;

			if(timer >= time)
			{
				timer = 0f;
				timeScale = 1f;
			}
			else if(lerp)
			{
				timeScale = Mathf.Lerp(scaledTime,1f,timer / time);
			}

			return timeScale;
		}

		public TimeScaleSet(float ts,float t,bool l)
		{
			scaledTime = ts;
			time = t;
			lerp = l;

			timer = 0f;
		}
	}

	public static float deltaTime{get{return _deltaTime * _timeScale;}}
	public static float timeScale{get{return _timeScale;}}
	public static float noneScaledDeltaTime{get{return _deltaTime;}}

	private static float _deltaTime = 0f;
	private static float _timeScale = 1f;

	private static float _scaleTimerValue = 0f;
	private static float _scaleTimer = 0f;
	private static float _scaledTimeValue = 0f;
	private static bool _lerp = false;

	private static Define.SimpleSortedThings<TimeScaleSet> _viTimeScale = new Define.SimpleSortedThings<TimeScaleSet>();

	public static float SetDeltaTime(float value) {_deltaTime = value; return deltaTime;}
	public static float TimeScaling(float time) {return time * (1f / _timeScale);}
	public static void SetTimeScale(float value) 
	{
		if(_scaleTimer != 0f)
			_scaleTimer = 0f;
			
		_timeScale = value;
	}

	public static void TimeScaleUpdate()
	{
		if(_viTimeScale.Count != 0)
		{
			var link = _viTimeScale.GetHead();
			_timeScale = link.item.Progress(_deltaTime);

			if(_timeScale == 1f)
				_viTimeScale.CutLink(link);
			
			link = link.next;
			while(link != null)
			{
				var item = link.item;

				if(item.Progress(_deltaTime) == 1f)
				{
					_viTimeScale.CutLink(link);
				}

				link = link.next;
			}
		}

		if(_scaleTimer != 0f)
		{
			_scaleTimerValue += _deltaTime;

			if(_lerp)
			{
				if(_viTimeScale.Count == 0)
				{
					_timeScale = Mathf.Lerp(_scaledTimeValue,1f,_scaleTimerValue / _scaleTimer);
				}
			}

			if(_scaleTimerValue >= _scaleTimer)
			{
				_scaleTimer = 0f;
				if(_viTimeScale.Count == 0)
				{
					_timeScale = 1f;
				}
			}
		}
	}

	public static void SetTimeScaleTimer(float timeScale, float time, bool lerp = false)
	{
		// if(_timeScale != 1f)
		// 	return;
		_timeScale = timeScale;
		_scaledTimeValue = timeScale;

		_scaleTimer = time;
		_scaleTimerValue = 0f;

		_lerp = lerp;
	}

	public static void SetViTimeScaleTimer(int level, float timeScale, float time, bool lerp = false)
	{
		_timeScale = timeScale;

		_viTimeScale.Insert(level,new TimeScaleSet(timeScale,time,lerp));
	}

}
