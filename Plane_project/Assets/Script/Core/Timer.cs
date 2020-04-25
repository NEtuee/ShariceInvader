using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Timer {

	public static float deltaTime{get{return _deltaTime * _timeScale;}}
	public static float timeScale{get{return _timeScale;}}
	public static float noneScaledDeltaTime{get{return _deltaTime;}}

	private static float _deltaTime = 0f;
	private static float _timeScale = 1f;

	private static float _scaleTimerValue = 0f;
	private static float _scaleTimer = 0f;
	private static float _scaledTimeValue = 0f;
	private static bool _lerp = false;

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
		if(_scaleTimer != 0f)
		{
			_scaleTimerValue += _deltaTime;

			if(_lerp)
			{
				_timeScale = Mathf.Lerp(_scaledTimeValue,1f,_scaleTimerValue / _scaleTimer);
			}

			if(_scaleTimerValue >= _scaleTimer)
			{
				_scaleTimer = 0f;
				_timeScale = 1f;
			}
		}
	}

	public static void SetTimeScaleTimer(float timeScale, float time, bool lerp = false)
	{
		_timeScale = timeScale;
		_scaledTimeValue = timeScale;

		_scaleTimer = time;
		_scaleTimerValue = 0f;

		_lerp = lerp;
	}

}
