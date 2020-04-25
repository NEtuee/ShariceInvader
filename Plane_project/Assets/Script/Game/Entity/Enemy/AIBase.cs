using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBase : PlaneBase
{

    protected float _turnFactor;


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
        RegisteCollisionList();
    }

    public abstract void AIProgress(float deltaTime);

    public override void progress(float deltaTime)
    {
        mainWeapon.Progress(deltaTime);

        AIProgress(deltaTime);

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }

    // public void TurnDirection(Vector3 targetDir)
    // {
    //     float dist = Vector3.Distance(_direction, targetDir);

    //     //ㅅㅂ
    // }
}
