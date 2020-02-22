using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInfo
{
	public Define.TouchState state;
	public int dragDir{get{return _dragDir;}}
	public int touchCount{get{return _touchCount;}}
	public bool touchSomething = false;
	public bool draged{get{return _draged;}}
	public Vector2 touchedPos{get{return _touchedPos;}}
    public Vector2 movedPos{get{return _movedPos;}}
	public Vector2 upPos{get{return _upPos;}}
	private Vector2 _touchedPos;
	private Vector2 _movedPos;
	private Vector2 _upPos;
	private int _dragDir = -1;
	private int _touchCount = -1;
	private bool _touched = false;
	private bool _canDrag = false;
	public bool _draged = false;
	private float _dragTimer = 0f;
	private float _TouchTimer = 0f;
	public void FindDirection()
	{
		Vector2 dir = _movedPos - _touchedPos;
		dir = dir.normalized;
		if(MathEx.abs(dir.x) > MathEx.abs(dir.y))
		{
			_dragDir = dir.x > 0f ? 0 : 2;
		}
		else
		{
			_dragDir = dir.y > 0f ? 1 : 3;
		}
	}
	public bool DragCheck(float dragFactor = 0.05f)
	{
		if(!_canDrag)
		{
			return false;
		}

		if(state == Define.TouchState.Moved && _touched)
		{
			float dist = Vector2.Distance(_touchedPos,_movedPos);
			if(dist >= Screen.width * dragFactor)
			{
				FindDirection();
				_draged = true;
//				Debug.Log(_dragDir);
	//			_touched = false;
				return true;
			}
			else
			{
				_dragDir = -1;
			}
		}
		else
		{
			_dragDir = -1;
		}

		return false;
	}
	public void TouchStart(int touchCount)
	{
		state = Define.TouchState.Began;
		_touchedPos = Input.mousePosition;
		_touched = true;
		_canDrag = true;
		_dragTimer = 0f;
		_TouchTimer = 0f;
		_touchCount = touchCount;
	}
	public void TouchMoved(Vector3 pos)
	{
		state = Define.TouchState.Moved;
		_movedPos = pos;
	}
	public void TouchUp()
	{
		state = Define.TouchState.End;
		_upPos = Input.mousePosition;
		_touched = false;
		_canDrag = false;
		_draged = false;
		touchSomething = false;
		_touchCount = -1;
	}
	public void BeforeUpdate()
	{
		if(state == Define.TouchState.Began)
		{
			state = Define.TouchState.Pushed;
		}
		else if(state == Define.TouchState.End)
		{
			state = Define.TouchState.None;
		}
	}
	public void LateUpdate()
	{
		if(_canDrag)
		{
			_dragTimer += Time.deltaTime;
			if(_dragTimer >= .5f)
			{
				_canDrag = false;
			}
		}
		if(_touched)
		{
			_TouchTimer += Time.deltaTime;
		}
	}
}