using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletType
{
	player = 0,
	enemy,
	end
}

public class BulletBase : Collisionable {

	public int attack{get{return _attack;}}
	public bool penetrate{get{return _penetrate;}}
	public bool canCollision{get{return _canCollision;}}

	private float _timer = 1f;

	private int _attack = 1;
	private bool _penetrate = false;
	private bool _canCollision = false;
	private bool _noneDelete = false;

	public override void firstSetting()
	{
		base.firstSetting();

		SetCollider(new Define.SimpleCircleCollider(.0725f,.0725f,_position));
		//SetSprite("Bullet");
	}

	public void Active(Vector3 pos, Vector3 dir, float speed, float timer = 1f)
	{
		_position = pos;
		_direction = dir;
		_speed = speed;
		_timer = timer;

		_attack = 1;
		_penetrate = false;
		_canCollision = true;
		_noneDelete = false;

		SetActive(true);
	}

	public override void initialize()
	{

	}

	public override void SetActive(bool value)
	{
		base.SetActive(value);
		if(!value)
		{
			EffectManager.GetInstance().AddEffect(_position,"Bullet_0_Destroy");
		}
	}

	public override void progress(float deltaTime)
	{
		Move(deltaTime);

		if(!_noneDelete)
		{
			_timer -= deltaTime;
			if(_timer <= 0f)
				SetActive(false);
		}
	}

	public void NoneDelete() {_noneDelete = true;}
	public void Penetrate() {_penetrate = true;}
	public void CanCollision(bool value) {_canCollision = value;}

	public bool LimitCheck(Vector4 limit)
	{
		if(_position.x < limit.x || _position.x > limit.y || 
			_position.y < limit.z || _position.y > limit.w)
		{
			if(_noneDelete)
			{
				return false;
			}
			
			SetActive(false);
//			Debug.Log("false");
			return true;
		}

		return false;
	}
}
