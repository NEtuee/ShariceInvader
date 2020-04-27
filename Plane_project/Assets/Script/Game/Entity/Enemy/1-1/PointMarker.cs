using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMarker : PlaneBase
{
    public override void firstSetting()
    {
        base.firstSetting();


        SetSpriteSet("SupplyShip",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));
    }

    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

    public override void initialize()
    {
        BasicInitialize();

        _direction = Vector3.left;
        _speed = 1f;
        _maxSpeed = 1f;

        //SetAdditionalSpeed(3,1f,true);

        RegisteCollisionList();
    }

    public override void progress(float deltaTime)
    {
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);

		BasicUpdate(deltaTime);
    }

}
