using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDrone : PlaneBase
{
    bool act = false;
	bool shot = false;

	int shotCount = 6;

    float shotTimer = 5f;
	float explosiveTimer = 0f;

	float close = 1f;
	float shotTurm = 0f;

	Vector3 shotPoint = new Vector3(-0.284f,0.3f,1f);

	AnimationControllEx spine;

	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("SpriteSet/Planes/MissileDrone/drone1C_body",AnimationType.None);
		SetCollider(new Define.SimpleBoxCollider(1.15f,.2f,_position));

		SetSortingOrder(1);

		CreateDecos();

		_maxSpeed = .4f;
		_speed = .2f;
        _gravityScale = 0f;
        _mass = 5f;

        _trailEmmit = false;
		_bounceOff = true;
		
		_minimapIcons[0] = ResourceManager.GetInstance().GetSprite("UI/map_eliteicon");
        _minimapIcons[1] = ResourceManager.GetInstance().GetSprite("UI/map_eliteiconarrow");
        miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sprite = _minimapIcons[0];
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);

		maxHp = _hp = 500;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);

		coll.UpdateBound(_position);
		for(int i = 0; i < 5; ++i)
		{
			var pos = _position;
			var con = coll.bound.right - coll.bound.left;
			con = con * .2f;
			pos.x = coll.bound.left;
			pos.x += con * i;

			Vector3 randPos = new Vector3(Random.Range(-.05f,.05f),Random.Range(-.05f,.05f));

			EffectManager.GetInstance().Explosion(pos + randPos,5,0.2f,0.25f,0.3f);
			EffectManager.GetInstance().AddEffect(pos + randPos,"SpriteSet/Effects/Explosion")
										.SetTarget(this)
										.SetAddPoint(randPos)
										.SetSortingOrder(2).SetAngle(Random.Range(0f,360f));

			EffectManager.GetInstance().EmitParticles("ExplosionSmoke",pos + randPos,4);
		}
	}

	public override void progress(float deltaTime)
	{            
        _direction = Vector3.left;

		float dist = Vector3.Distance(Player.instance.position,_position);
		act = dist < 8f ? true : false;

		if(close != 0f)
		{
			close -= deltaTime;
			if(close <= 0f)
			{
				if(spine.currAni == "open")
					if(spine.isEnd)
						spine.ChangeAni("close",false);
			}
		}

		if(act)
		{
			shotTimer += deltaTime;
			if(shotTimer >= 10f)
			{
				shotTimer = 0f;
				close = 1.5f;

				spine.ChangeAni("open",false);
				EffectManager.GetInstance().AddEffect(_position + new Vector3(0.01f * _scale.x,0.495f),"SpriteSet/Planes/MissileDrone/launch",false,this)
											.SetAddPoint(new Vector3(-0.01f * _scale.x,0.495f))
											.SetScale(-_scale.x,1f,1f);

				shotCount = 6;
				shot = true;
				shotTurm = 0.1f;
			}
		}

		if(shot)
		{
			shotTurm -= deltaTime;
			if(shotTurm <= 0f)
			{
				shotTurm = 0.1f;

				Vector3 pos = _position + shotPoint;
				pos.x -= (float)(6 - shotCount) * (0.109f * -_scale.x);

				var obj = ObjectManager.GetInstance().AddObject<NPC>(Define.ObjectType.enemy,"commonMissile");
				obj.SetPositionEm(pos).SetDirection(new Vector3(0f,1f));
				obj.SetAngle(90f);
				obj.SetAbsoluteForce(new Vector3(0f,100f));
				obj._gravityScale = 5f;

				if(--shotCount == 0)
				{
					shot = false;
				}
			}
		}
        
		if(_hp < 15)
		{
			explosiveTimer -= deltaTime;

			if(explosiveTimer <= 0f)
			{
				explosiveTimer = Random.Range(0.1f,0.5f);

				Vector3 randPos = new Vector3(Random.Range(-1.15f,1.15f),Random.Range(-.2f,.2f));

				EffectManager.GetInstance().Explosion(_position + randPos,5,0.2f,0.2f,0.3f);
				EffectManager.GetInstance().AddEffect(_position + randPos,"SpriteSet/Effects/Explosion")
											.SetTarget(this)
											.SetAddPoint(randPos)
											.SetSortingOrder(2).SetAngle(Random.Range(0f,360f));
			
				EffectManager.GetInstance().EmitParticles("ExplosionSmoke",_position + randPos,4);
				//EffectManager.GetInstance().ExplosionSmoke(_position + randPos,_position + randPos + new Vector3(Random.Range(-0.2f,0.2f),Random.Range(-0.2f,0.2f)),0.15f,0.01f,4);
			}
		}

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
	}  

	public void CreateDecos()
	{
		spine = _deco.AddDeco(new Vector2(-0.09f,0.195f));
		var fin = _deco.AddDeco(new Vector2(0.59f,0.005f));
		var boost = _deco.AddDeco(new Vector2(-0.76f,0.22f));
		var sideLight = _deco.AddDeco(new Vector2(0.23f,0.11f));

		spine.AddAnimation("close","SpriteSet/Planes/MissileDrone/spineClosing");
		spine.AddAnimation("open","SpriteSet/Planes/MissileDrone/spineOpening");

		fin.AddAnimation("loop","SpriteSet/Planes/MissileDrone/fin");

		boost.AddAnimation("loop","SpriteSet/Planes/MissileDrone/booster");

		sideLight.AddAnimation("loop","SpriteSet/Planes/MissileDrone/sideLight");

		spine.ChangeAni("close",false);
		fin.ChangeAni("loop",true);
		boost.ChangeAni("loop",true);
		sideLight.ChangeAni("loop",true);
	}
}
