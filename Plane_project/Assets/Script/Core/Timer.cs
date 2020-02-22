using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : Singleton<Timer> {

	public float deltaTime{get{return _deltaTime * _timeScale;}}
	public float timeScale{get{return _timeScale;}}
	public float noneScaledDeltaTime{get{return _deltaTime;}}

	private float _deltaTime = 0f;
	private float _timeScale = 1f;

	private float _scaleTimerValue = 0f;
	private float _scaleTimer = 0f;
	private float _scaledTimeValue = 0f;
	private bool _lerp = false;

	public float SetDeltaTime(float value) {_deltaTime = value; return deltaTime;}
	public float TimeScaling(float time) {return time * (1f / _timeScale);}
	public void SetTimeScale(float value) {_timeScale = value;}

	public void TimeScaleUpdate()
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

	public void SetTimeScaleTimer(float timeScale, float time, bool lerp = false)
	{
		_timeScale = timeScale;
		_scaledTimeValue = timeScale;

		_scaleTimer = time;
		_scaleTimerValue = 0f;

		_lerp = lerp;
	}

}
