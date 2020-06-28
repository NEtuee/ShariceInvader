using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intercepter : PlaneBase
{
    private Vector3 randFactor;

    public override void firstSetting()
    {
        base.firstSetting();


        SetSpriteSet("SpriteSet/Planes/Enemy",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));
    }

    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);
	}

    public override void initialize()
    {
        BasicInitialize();

        _direction = Vector3.left;
        _speed = 0.2f;
        _maxSpeed = 3.2f; 

        RegisteCollisionList();
    }

    public override void progress(float deltaTime)
    {


        _direction = (Player.instance.position - _position).normalized;


        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }
}
