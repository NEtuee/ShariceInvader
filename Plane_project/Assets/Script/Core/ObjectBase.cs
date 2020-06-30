using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour, Define.IProgress {

	public static int uniqueNumberGen = 0;
	public int uniqueNumber = 0;

	public bool active{get{return _isActive;}}
	public bool deleted{get{return _isDeleted;}}

	public Vector3 position{get{return _position;}}
	public Vector3 direction{get{return _direction;}}
	public float angle{get{return _eulerAngle;}}
	public float mainSpeed{get{return _speed;}}

	[HideInInspector]
	public bool cacheObject = false;
	public bool noneTarget = false;

	private bool _isActive = true;
	private bool _isDeleted = false;

	[HideInInspector]
	public Transform tp;
	[HideInInspector]
	public GameObject obj;
	[HideInInspector]
	public Define.ObjectType type;


	public PlaceMapper.Place place;

	protected Vector3 _position = new Vector3();
	protected Vector3 _scale = new Vector3(1f,1f,1f);
	protected float _eulerAngle = 0f;

	protected Vector3 _direction;
	protected float _speed;

	public abstract void firstSetting();
	public abstract void initialize();
	public virtual void BeforeCreated(){}
	public abstract void progress(float deltaTime);
	public virtual void afterProgress(float deltaTime){}
	public virtual void deleteEvent(){}
	public virtual void beforeUpdateTransform(){}
	public virtual void afterUpdateTransform(){}
	public abstract void release();

	public void UpdateTransform()
	{
		if(tp.localScale != _scale)
		{
			tp.localScale = _scale;
		}

		if(tp.position != _position || 
			tp.localEulerAngles.z != _eulerAngle)
		{
			tp.SetPositionAndRotation(_position,Quaternion.Euler(0f,0f,_eulerAngle));
			_eulerAngle = tp.localEulerAngles.z;
		}
	}

	public void SetTransform(){tp = GetComponent<Transform>();}
	public void SetGameObject(){obj = gameObject;}
	public ObjectBase SetObjectType(Define.ObjectType t) {type = t; return this;}
	public virtual ObjectBase SetPosition(Vector3 pos) 
	{
		_position = pos;
		return this;
	}
	public virtual ObjectBase SetPositionEm(Vector3 pos) 
	{
		_position = pos;
		UpdateTransform();
		return this;
	}
	public ObjectBase SetAngle(float z) {_eulerAngle = z; return this;}
	public ObjectBase SetScale(float x,float y, float z) {_scale = new Vector3(x,y,z); return this;}
	public ObjectBase SetDirection(Vector3 dir) {_direction = dir; return this;}
	public ObjectBase SetSpeed(float sp) {_speed = sp; return this;}

	public void Move(float deltaTime)
	{
		_position += _direction * _speed * deltaTime;
	}

	public virtual void SetActive(bool value){obj.SetActive((_isActive = value));}
	public void Delete()
	{
		_isDeleted = true;
		if(place != null)
		{
			place.ExitPlace(this);
		}
		deleteEvent();
	}
	public void Revive(){_isDeleted = false;}

	public void SetNecessary()
	{
		SetTransform();
		SetGameObject();

		uniqueNumber = uniqueNumberGen++;
	}
}
