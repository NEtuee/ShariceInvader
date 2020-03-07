using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDrone : PlaneBase
{
    bool act = false;

    float shotTimer = 0f;
	float explosiveTimer = 0f;
	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("drone1C",AnimationType.None);
		SetCollider(new Define.SimpleBoxCollider(1.15f,.2f,_position));

		SetSortingOrder(1);

		_maxSpeed = .9f;
		_speed = .2f;
        _gravityScale = 0f;
        _mass = 5f;

        _trailEmmit = false;
		_sprRenderer.flipX = true;
		_bounceOff = true;
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);

		_hp = 50;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

	public override void progress(float deltaTime)
	{            
        _direction = Vector3.left;

		float dist = Vector3.Distance(GameManager.instance.player.position,_position);
		act = dist < 8f ? true : false;

		if(act)
		{
			shotTimer += deltaTime;
			if(shotTimer >= 5f)
			{
				shotTimer = 0f;

				for(int i = 0; i < 5; ++i)
				{
					ObjectManager.GetInstance().AddObject<NPC>(Define.ObjectType.enemy,"Missile").
												SetPosition(_position + new Vector3(Random.Range(-0.4f,0.4f),0f));
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
			}
		}

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
	}   
}
