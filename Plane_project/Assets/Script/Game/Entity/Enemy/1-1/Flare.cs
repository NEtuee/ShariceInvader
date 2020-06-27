using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : PlaneBase
{
    AnimationControllEx _aniCon;

    float _actTimer = 0f;
    bool _hit = false;

    public ObjectBase target;

    public bool right;

    public override void deleteEvent()
	{
		BasicDeleteEvents();

        if(_hit)
        {
            Explode();
        }

        if(_stunEffect != null)
            _stunEffect.SetActive(false);
		//ComboCount.instance.AddComboCount(1);
	}

    public override void Hit(int val,ObjectBase attacker)
    {
        _hit = true;

        base.Hit(val,attacker);
    }

    public override void firstSetting()
    {
        base.firstSetting();

        _aniType = AnimationType.None;
        //SetSpriteSet("Enemy",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        _aniCon = new AnimationControllEx(_sprRenderer);

        _aniCon.AddAnimation("Launch","SpriteSet/Planes/RayDrone/Flare/Launch");
        _aniCon.AddAnimation("Disapear","SpriteSet/Planes/RayDrone/Flare/Disapear");
        _aniCon.AddAnimation("Loop","SpriteSet/Planes/RayDrone/Flare/Loop");
    }

    public override void initialize()
    {
        BasicInitialize();

        _burst = false;

        _speed = 0.2f;
        _bodyAttack = 0;

        RegisteCollisionList();

        _aniCon.ChangeAni("Launch",false);

        _actTimer = Random.Range(1.8f,2.3f);

        SetSortingOrder(2);
        //_maxSpeed = 0.1f;
    }

    public override void progress(float deltaTime)
    {
        if(target.deleted && _aniCon.currAni != "Disapear")
            _aniCon.ChangeAni("Disapear",false);

        _aniCon.AnimationProgress(deltaTime);
        if(_aniCon.currAni == "Launch")
        {
            if(_aniCon.isEnd)
            {
                _aniCon.ChangeAni("Loop",true);
            }
        }
        else if(_aniCon.currAni == "Disapear")
        {
            if(_aniCon.isEnd)
                Delete();
        }

        if(_actTimer < 0f)
        {
            _actTimer = 0f;
            _aniCon.ChangeAni("Disapear",false);
        }
        else if(_actTimer > 0f)
        {
            _actTimer -= deltaTime;
        }

        AddForce(target.direction);
        Vector3 dir = Vector3.Cross(_direction,new Vector3(0f,0f,-1f)) * (right ? 1f : -1f);
        //_direction = (target.position - _position).normalized;
        AddForce(dir * 0.15f);
        _eulerAngle = MathEx.directionToAngle(_velocity.normalized);


        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }

    public void Explode()
    {
        EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Planes/RayDrone/Flare/Explode",false,null);
        for(int i = 0; i < 8; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-0.18f,0.18f),Random.Range(-0.02f,0.18f));
            EffectManager.GetInstance().AddEffect(pos + _position,"SpriteSet/Planes/RayDrone/Flare/Exparticle/" + Random.Range(0,2).ToString(),false,null)
                                        .DelayApear(Random.Range(0f,0.1f));
        }

        for(int i = 0; i < 4; ++i)
        {
            EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Planes/RayDrone/Flare/Spark/" + Random.Range(0,2).ToString(),false,null)
                                        .DelayApear(Random.Range(0f,0.05f))
                                        .SetAngle(Random.Range(0f,360f));
        }


    }
}
