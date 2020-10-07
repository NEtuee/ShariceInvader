using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Test : WeaponBase
{
    private Define.SimpleCollider _collider;

    public override void Initialize()
    {
        base.Initialize();
        _collider = new Define.SimpleCircleCollider(1f,1.1f,Vector2.zero);
        _plane.SetImmortal(false);

        _icon = ResourceManager.GetInstance().GetSprite("UI/icon_nova");
        _ui = ResourceManager.GetInstance().GetSprite("UI/ui_nova");

        GagueSetup(1f,10f,8f,3f);

    }
    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        specAttack = false;

        if(mainAttack)
        {
			_mainTimer = mainCoolTime;
			mainAttack = false;
            //_plane.SetImmortal(true);
			//_plane.SetBodyAttack(0);
        }
        else if (CoolDownCheck(ref _mainTimer,deltaTime))
        {
            _plane.SetImmortal(false);
        }
    }
    public override bool MainAttack()
    {
        // _plane.BurstActive();

        // _attackTime = .1f;
        // _plane.SetMaxSpeed(20f);

        bool atk = false;

        Define.ObjectType t = _plane.type == Define.ObjectType.enemy ? Define.ObjectType.player : Define.ObjectType.enemy;
        var list = CollisionManager.GetInstance().GetCollisionList(t);

        if(list != null)
        {
            int count = list.Count;
        

            for(int i = 0 ; i < count; ++i)
            {
                _collider.UpdateBound(_plane.position);
                list[i].UpdateCollider();
    
                if(_collider.CollisionCheck(list[i].coll))
                {
                    ObjectManager.GetInstance().UpdateStop(0.1f);
                    var target = ((PlaneBase)list[i]);
		    		HitEffect(target);
                    AddHitEffect(target.position);
    
                    CameraControll.instance.Shake(0.2f, _plane.direction / 11f);
    
                    target.Hit(_plane);

                    atk = true;
                }
            }
        }

        if(atk)
        {
            mainAttack = true;
            EffectManager.GetInstance().AddEffect(_plane.position,"SpriteSet/Effects/Weapon/Pulse/Attack",false)
                                .SetSortingOrder(1);
            
            SoundManager.instance.Play("SE/Pulse_",false);
            SoundManager.instance.Play("SE/MainHit_",false,2,.5f,false);
            //Timer.SetTimeScaleTimer(0.3f,0.5f,true);
            return true;
        }
        else
        {
            _plane.BurstActive();
            return false;
        }
		//_plane.SetControll(true);
    }
    public override bool SpecialAttack(Vector3 dir)
    {
        var link = ObjectManager.GetInstance().GetFirstLink(Define.ObjectType.enemy);

        while(link != null)
        {
            float dist = Vector3.Distance(_plane.position,link.target.position);

            if(dist <= 5f)
            {
                var obj = (PlaneBase)link.target;
                if(obj._mass < 5f)
                {
                    Vector3 d = (obj.position - _plane.position).normalized;

                    obj.SetAbsoluteForce(d * 3f);
                    obj.ControllLock(2f);
    
                    EffectManager.GetInstance().AddEffect(obj.position,"SpriteSet/Effects/Electric",false,obj).SetSortingOrder(1).SetAngle(Random.Range(0f,360f));
                    EffectManager.GetInstance().AddEffect(obj.position,"SpriteSet/Effects/Burst",false)
                                            .SetAngle(MathEx.directionToAngle(d));
                }
            }

            link = link.next;
        }

        CameraControll.instance.Zoom(2.3f);

        specAttack = true;

        EffectManager.GetInstance().AddEffect(_plane.position,"SpriteSet/Effects/Weapon/Pulse/Drive",false)
                                .RealTimeProgress();
        
        SoundManager.instance.Play("SE/Pulse_Drive_",false,2,1f,false);

        Timer.SetViTimeScaleTimer(3,0.1f,0.3f);

        CloseDriveText();

        return false;
    }


    public void AddHitEffect(Vector3 pos)
    {
        float d = Vector3.Distance(pos,_plane.position);
        float rand = 0f;
        string s = "";

        if(d <= 0.33f)
        {
            s = "SpriteSet/Effects/Weapon/Pulse/Hit/0";
            rand = 0.39f;
        }
        else if(d <= 0.66f)
        {
            s = "SpriteSet/Effects/Weapon/Pulse/Hit/1";
            rand = 0.27f;
        }
        else
        {
            s = "SpriteSet/Effects/Weapon/Pulse/Hit/2";
            rand = 0.19f;
        }

        EffectManager.GetInstance().AddEffect(pos,s,false).SetSortingOrder(3);

        for(int i = 0; i < 7; ++i)
        {
            EffectManager.GetInstance().AddEffect(pos + MathEx.RandomCircle(rand),"SpriteSet/Effects/Weapon/Pulse/Particle",false)
                                    .SetSortingOrder(2);
        }
        
    }

    public override void WhenChanged()
    {
        base.WhenChanged();

        _plane.SetImmortal(false);

        foreach(var ani in _plane._boostAni)
        {
            ani.CopyAnimation("Burst",ani.aniOriginPath["Burst"]);
            ani.CopyAnimation("Loop",ani.aniOriginPath["Loop"]);

            if(ani.currAni == "Loop")
                ani.ChangeAni("Loop",true,false);
        }
    }

    public override void Change()
    {
        foreach(var ani in _plane._boostAni)
        {
            ani.CopyAnimation("Burst","SpriteSet/Effects/Weapon/Pulse/Burst");
            ani.CopyAnimation("Loop","SpriteSet/Effects/Weapon/Pulse/Loop");

            if(ani.currAni == "Loop")
                ani.ChangeAni("Loop",true,false);
        }

        MainHud.instance.MainUIAniSwap("Change","UI/Weapon/Pulse/Attack");
        MainHud.instance.MainUIAniSwap("MainAttack","UI/Weapon/Pulse/Attack");
        MainHud.instance.MainUIAniSwap("Boost","UI/Weapon/Pulse/Boost");
        MainHud.instance.MainUIAniSwap("DriveOn","UI/Weapon/Pulse/DriveOn");
        MainHud.instance.MainUIAniSwap("DriveAttack","UI/Weapon/Pulse/DriveEnd");
    }

    public override void DriveOn()
    {
        OpenDriveText();
    }

    public override bool CollisionCheck(PlaneBase target)
    {
        return false;
    }

    public Weapon_Test(PlaneBase plane) : base(plane)
    {
        
    }
}
