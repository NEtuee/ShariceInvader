using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostDrone : PlaneBase
{
    public PlaneBase target;

    public bool leader = false;

    bool act = false;

    float _actTimer = 0f;

    float _timer = 0f;

    Vector3 _randomPos;
    Vector3 _stickPos;
	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("Enemy",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		_maxSpeed = Random.Range(2f,2.25f);
		_speed = .2f;
        _gravityScale = 0.3f;
        _mass = 2f;

        _bodyAttack = 0;
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);

        RegisteCollisionList();

        _burst = false;
        _bodyAttack = 0;
        _timer = 0f;
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

    public override void CollisionProgress(Define.ObjectType type, Collisionable target)
    {
        if(type != Define.ObjectType.enemy && type != Define.ObjectType.player)
			return;
		
		var plane = (PlaneBase)target;

		if(plane.mainWeapon.mainAttack)
			Hit(plane);
        else
        {
            act = true;
            SetNoClip(true);
            this.target = plane;

            _stickPos = target.position - _position;
        }
		
    }

	public override void progress(float deltaTime)
	{
        if(act)
        {
            _actTimer += deltaTime;

            SetPosition(target.position + _stickPos);

            if(_actTimer >= 1.3f)
            {
                target.SetAdditionalSpeed(10f,0.1f,true);
                target.AddForce(MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad) * 10f);
                target.ControllLock(0.1f);

                Hit(_hp);
                act = false;
            }
        }
        else
        {
            if(!leader && (target == null || target.deleted))
            {
                leader = true;
                target = GameManager.instance.player;
            }

            _direction = ((target.position + _randomPos) - _position).normalized;


            _timer += deltaTime;
            if(_timer >= 3f)
            {
                _timer = 0f;
                TargetPosUpdate();
            }

            BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		    BasicUpdate(deltaTime);
        }
	}

    public void TargetPosUpdate()
    {
        _randomPos = new Vector3(Random.Range(-0.2f,0.2f),Random.Range(-0.2f,0.2f));
    }
}
