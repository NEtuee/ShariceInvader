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

		_maxSpeed = 1f;
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

        
		if(_hp < 20)
		{
			explosiveTimer -= deltaTime;

			if(explosiveTimer <= 0f)
			{
				explosiveTimer = Random.Range(0.1f,0.5f);

				Vector3 randPos = new Vector3(Random.Range(-1.15f,1.15f),Random.Range(-.2f,.2f));

				EffectManager.GetInstance().Explosion(_position + randPos,7,0.4f);
			}
		}

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
	}   
}
