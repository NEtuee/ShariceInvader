using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intercepter : PlaneBase
{
    private Vector3 randFactor;

    public override void firstSetting()
    {
        base.firstSetting();


        SetSpriteSet("Enemy",AnimationType.Horizontal);
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
        _speed = 0.2f;
        _maxSpeed = 3.2f;

        WeaponChange(new Weapon_Lancer(this));

        RegisteCollisionList();
    }

    public override void progress(float deltaTime)
    {
        mainWeapon.Progress(deltaTime);


        _direction = (GameManager.instance.player.position - _position).normalized;


        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }
}
