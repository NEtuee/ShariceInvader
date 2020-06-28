using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostDrone : PlaneBase
{
    public PlaneBase target;

    public bool leader = false;

    bool act = false;

    float _actTimer = 0f;
    float _deleteTimer = 0f;
    float _timer = 0f;

    Vector3 _randomPos;
    Vector3 _stickPos;

    Sprite[] _base;

	public override void firstSetting()
	{
		base.firstSetting();
		_aniType = AnimationType.None;
        _base = ResourceManager.GetInstance().GetSpriteSet("SpriteSet/Planes/BoostDrone/Base");

        SetSprite(_base[1]);

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

    public override void Explosion()
    {
		EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Effects/Explosion").SetSortingOrder(2).SetAngle(Random.Range(0f,360f));
		EffectManager.GetInstance().Explosion(_position,15,0.2f,0.15f,0.23f);
    }

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);
	}

    public override void CollisionProgress(Define.ObjectType type, Collisionable target)
    {
        if(type != Define.ObjectType.enemy && type != Define.ObjectType.player)
			return;
		
		var plane = (PlaneBase)target;

		if(plane.weaponInven.mainAttack)
			Hit(plane);
        else
        {
            act = true;
            SetNoClip(true);
            this.target = plane;

            _stickPos = _position - target.position;
            SetSprite(_base[0]);
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
                target.SetAbsoluteForce(MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad) * 10f);
                target.ControllLock(0.1f);

                EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Planes/BoostDrone/Boost",false,this)
                                        .SetAngle(_eulerAngle);

                _deleteTimer = 0.7f;
                _actTimer = -3f;
            }
        }
        else
        {
            if(!leader && (target == null || target.deleted))
            {
                leader = true;
                target = Player.instance;
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

        if(_deleteTimer != 0f)
        {
            _deleteTimer -= deltaTime;
            if(_deleteTimer <= 0f)
            {
                Hit(_hp,null);
                act = false;
                _deleteTimer = 0f;
            }
        }
	}

    public void TargetPosUpdate()
    {
        _randomPos = new Vector3(Random.Range(-0.2f,0.2f),Random.Range(-0.2f,0.2f));
    }
}
