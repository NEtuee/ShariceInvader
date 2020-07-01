using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : PlaneBase {

	ObjectBase _target;

	Vector3 randD;
	bool random = false;
	bool act = false;
	float timer = 0f;

	float actTime = 0f;

	AnimationControllEx _ani;

	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("SpriteSet/Planes/commonMissile/missile",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		BoostSetUp("SpriteSet/Effects/commonMissileBoost",new Vector2(-0.1f,0f));

		_sprRenderer.flipX = true;

		_maxSpeed = 4f;
		_speed = .1f;

		_ani = new AnimationControllEx(_sprRenderer);
		_ani.AddAnimation("open","SpriteSet/Planes/commonMissile/launch");
		_ani.AddAnimation("loop","SpriteSet/Planes/commonMissile/spin");

		//_sprRenderer.sprite = _ani.animations["open"][0].sprite;
		_ani.ChangeAni("open",false);
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);

		_maxSpeed = 4f;
		_speed = .1f;
		maxHp = _hp = 10;

		_bodyAttack = 30;

		actTime = .8f;
		_burst = false;
		_rotateLock = true;

		RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);
	}

	public override void progress(float deltaTime)
	{
		if(!act)
		{
			timer += deltaTime;

			if(timer >= actTime)
			{
				timer = 0f;
				act = true;
				_rotateLock = false;

				_ani.ChangeAni("open",false);
				SoundManager.instance.Play("SE/Missile_",false,4);

				_maxSpeed = Random.Range(3.8f,4.5f);
				_gravityScale = .7f;
				_mass = 1f;

				_boostAniProgress = true;
				BurstActive();
				// EffectManager.GetInstance().AddEffect(_position + new Vector3(0f,-0.1f),"commonMissileBoost/Burst")
				// 							.SetAngle(90f);
			}
			else
			{
				BasicUpdate(deltaTime);
				return;
			}
		}

		float dist = Vector3.Distance(_position,Player.instance.position);

		_ani.AnimationProgress(deltaTime);

		if(_ani.currAni == "open" && _ani.isEnd)
		{
			_ani.ChangeAni("loop",true);
		}

		_direction = (Player.instance.position - _position).normalized;

		if(dist <= 5f && !_controllLock)
		{
			Vector3 pos = new Vector3(Random.Range(-0.02f,0.02f),Random.Range(-0.02f,0.02f)) + _position;
			EffectManager.GetInstance().EmitParticles("MissileTrailSmoke",pos,Random.Range(0.8f,2f),Random.Range(0.2f,0.35f),1);
			EffectManager.GetInstance().EmitParticles("MissileTrailSmoke",pos,Random.Range(0.8f,2f),Random.Range(0.2f,0.35f),1);
		}
		// float dist = Vector3.Distance(_position,Player.instance.position);
		// if(dist <= 2f)
		// 	_maxSpeed = Random.Range(10f,11f);
		// if(timer == 0f)
		// {
		// 	if(dist > 3f)
		// 	{
		// 		BurstActive();
		// 		timer = .5f;
		// 		_maxSpeed = 16f;
		// 	}
		// }
		// else
		// {
		// 	timer -= deltaTime;
		// 	if(timer <= 0f)
		// 	{
		// 		timer = 0f;
		// 	}
		// }

		if(!random)
			_direction = (Player.instance.position - _position).normalized +
							randD.normalized * .1f;

		if(dist < 0.5f)
		{
			randD = (Player.instance.position - _position).normalized * Random.Range(1f,1.25f);;
			timer = Random.Range(2f,2.5f);
			random = true;
		}

		timer -= deltaTime;
		if(timer <= 0f)
		{
			randD = (_position - Player.instance.position).normalized;
			random = false;
			timer = Random.Range(0.5f,4f);
			randD = new Vector3(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f));

			if(dist < 1.5f)
			{
				randD = (_position - Player.instance.position).normalized;
				timer = Random.Range(0.5f,2f);
				random = true;
			}
		}

		BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
	}
}
