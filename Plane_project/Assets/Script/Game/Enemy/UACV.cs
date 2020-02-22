using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UACV : PlaneBase
{
    float _altitude = 0f;

    bool act = false;
    Vector3 targetDirection;
    float shotTimer = 0f;
	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("Enemy",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		_maxSpeed = Random.Range(2f,2.25f);
		_speed = .2f;
        _gravityScale = 0.3f;
        _mass = 2f;

        //_trail.gameObject.SetActive(false);
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);

        _eulerAngle = 0f;

        _altitude = ObjectManager.GetInstance()._place._mapHeight * 0.3f;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

	public override void progress(float deltaTime)
	{            
        if(!act)
        {
            float d = Vector2.Distance(position,GameManager.instance.player.position);
            
            if(position.y < _altitude)
                targetDirection = new Vector3(MathEx.normalize(_direction.x),0.5f).normalized;
            else
                targetDirection = Vector3.right * MathEx.normalize(_direction.x);
            
            if(d < 4f)
            {
                act = true;
                shotTimer = 3f;
            }
        }
        else
        {
            targetDirection = (GameManager.instance.player.position - position).normalized;
            if(position.y < _altitude)
                targetDirection = new Vector3(MathEx.normalize(targetDirection.x),0.5f).normalized;

            float d = Vector2.Distance(position,GameManager.instance.player.position);
            if(d > 4f)
            {
                act = false;
            }

            if(shotTimer != 0f)
            {
                shotTimer -= deltaTime;
                if(shotTimer <= 0f)
                {
                    ObjectManager.GetInstance().AddObject<NPC>(Define.ObjectType.enemy,"Missile").SetPosition(_position);
                    shotTimer = 5f;
                }
            }
        }

        _direction = Vector3.Lerp(_direction,targetDirection,2f * deltaTime);
        
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);
	}
}
