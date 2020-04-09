using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControll : SingletonMono<CameraControll>, Define.IProgress {

	public ScreenGlitch screenGlitch;

	public Camera mainCam{get{return _main;}}
	private Transform _followTarget = null;
	private float _followSpeed = 10f;

	private Transform _tp;
	private Camera _main;
	private Vector3 _position;
	private float _zDist = -10f;
	private float _shakeTimer = 0f;
	private float _shakeTurm = .1f;
	private Vector2 _shakeFactor = new Vector2(1f,1f);

	private float _mainCamSize = 0f;
	private float _zoomScale;
	private float _glitchTimer = 0f;
	private float _timeSaver = 0f;
	private float _delayTimer = 0f;

	private float camWidth;
	private float camHeight;

	public void firstSetting()
	{
		SetSingleton(this);
		_tp = transform;
		_position = _tp.position;
		_main = GetComponent<Camera>();

		camHeight = _main.orthographicSize;
		camWidth = camHeight * ((float)Screen.width / (float)Screen.height);

		_mainCamSize = _main.orthographicSize;
	}

	public Vector4 GetCamBounds() //x min x max y min y max
	{
		return new Vector4(_position.x - camWidth, _position.x + camWidth,
							_position.y - camHeight, _position.y + camHeight);
	}

	public void SetTarget(Transform target)
	{
		_followTarget = target;
	}

	public void SetSpeed(float value)
	{
		_followSpeed = value;
	}


	public void initialize()
	{

	}

	public Vector3 ScreenToWorldMouse()
	{
		Vector3 pos = _main.ScreenToWorldPoint(Input.mousePosition);
		pos.z = 0f;
		return pos;
	}

	public void progress(float deltaTime)
	{
		if(_followTarget != null)
		{
			_followSpeed = Mathf.Lerp(_followSpeed,10f,0.05f);
			_position = Vector2.Lerp(_position,_followTarget.position,Timer.GetInstance().noneScaledDeltaTime * _followSpeed);
			_position.z = _zDist;
		}

		if(_followTarget != null)
		{
			if(_delayTimer != 0f)
			{
				_delayTimer -= Time.deltaTime;
				if(_delayTimer <= 0f)
				{
					Timer.GetInstance().SetTimeScaleTimer(1f,0.3f,true);
					_delayTimer = 0f;
				}
			}
		}

		if(_shakeTimer > 0f)
		{
			_shakeTimer -= deltaTime;
			_shakeTurm -= deltaTime;

			if(_shakeTurm <= 0)
			{
				float x = _shakeFactor.x;
				float y = _shakeFactor.y;
			
				_position += new Vector3(Random.Range(-x, x), Random.Range(-y, y));

				_shakeTurm = 0.01f;
			}
		}

		if(_glitchTimer != 0f)
		{
			_glitchTimer -= deltaTime;

			float percentage = (_timeSaver - _glitchTimer) / _timeSaver;

			screenGlitch._colorDrift = Mathf.Lerp(0.4f,0f,percentage);
			screenGlitch._scanLineJitter = Mathf.Lerp(0.5f,0f,percentage);

			// if(percentage > 0.5f)
			// {
			// 	digitalGlitch.digitalIntensity = .2f;
			// }
			// else
			// {
			// 	digitalGlitch.digitalIntensity = .1f;
			// }

			if(_glitchTimer <= 0f)
			{
				screenGlitch._colorDrift = 0f;
				screenGlitch._scanLineJitter = 0f;

				screenGlitch.progress = false;
				_glitchTimer = 0f;
			}
		}

		_main.orthographicSize = Mathf.Lerp(_main.orthographicSize,_mainCamSize,4f * deltaTime);
	}

	public void Glitch(float time)
	{
		_timeSaver = _glitchTimer = time;

		screenGlitch.progress = true;

		screenGlitch._colorDrift = .4f;
		screenGlitch._scanLineJitter = .4f;
	}

	public void Shake(float time, Vector2 factor)
	{
		_shakeTimer = time;
		_shakeFactor = factor;
		_shakeTurm = 0f;
	}

	public void Zoom(float scale)
	{
		if(_main.orthographicSize > scale)
			_main.orthographicSize = scale;
	}

	public void FollowDelay(float delay)
	{
		_delayTimer = delay;

		SetSpeed(1f);
	}

	public void release()
	{

	}

	public void SyncPosition()
	{
		_tp.SetPositionAndRotation(_position, _tp.rotation);
	}
}
