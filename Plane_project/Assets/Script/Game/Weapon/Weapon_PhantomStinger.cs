using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PhantomStinger : WeaponBase
{
    private PlaneBase.hitEventDelegate delayEvent;

    private List<EffectBase> _aimEffects = new List<EffectBase>();

    int _maxTarget = 7;

    public override void Initialize()
    {
        base.Initialize();

        mainCoolTime = 0.22f;


        _icon = ResourceManager.GetInstance().GetSprite("UI/icon_phantomstinger");
        _ui = ResourceManager.GetInstance().GetSprite("UI/ui_phantomstinger");

        InitAimObject();

        immedyActiveSpecAttack = true;

        delayEvent = SpecialTargetHitEvent;

        GagueSetup(1f,5f,15f,5f);
    }
    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(mainAttack)
        {
            _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));
            
            if(ControllerEx.GetInstance().KeyUp("MainAttack"))
            {
                MainAttackProgress();
            }
        }
        else if(specAttack)
        {
            if(_multiTarget.Count > 0)
                _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));

            if(ControllerEx.GetInstance().KeyUp("DriveAttack"))
            {
                SpecialAttackProgress();
            }
        }
        else
        {
            UpdateAimTarget(1.5f,80f);
        }

        if(CoolDownCheck(ref _mainTimer,deltaTime))
        {
            //_plane._rotateLock = true;
        }

        //UpdateAim();
    }
    public override bool MainAttack()
    {
        if(_aim)
        {
            _plane.SetSpeed(0f);
            _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));

            Timer.SetTimeScale(0.2f);
            mainAttack = true;

            _aimAni.ChangeAni("Lock",false);

            return true;
        }
        else
        {
            _plane.BurstActive();

            return false;
        }
    }

    public void MainAttackProgress()
    {
        if(_aimTarget == null || _aimTarget.deleted)
        {
            Timer.SetTimeScale(1f);
            mainAttack = false;
            return;
        }

        EffectManager.GetInstance().AddEffect(_aimTarget.position,"PhantomString_Aim/Shot").SetSortingOrder(10);

        _mainTimer = mainCoolTime;
        mainAttack = false;
        //_plane._rotateLock = true;
        Vector3 pos = _plane.position + _plane.direction * 0.25f;
        EffectManager.GetInstance().AddEffect(pos,"Fire").SetAngle(_plane.angle);

        HitEffects(_aimTarget.position);

        CameraControll.instance.Shake(0.2f, _plane.direction / 15f);

        BurstAimDirection(15f,0.08f);

        Timer.SetTimeScale(1f);
        _aimTarget.Hit(3,_plane);

        float dist = Vector2.Distance(_plane.position,_aimTarget.position);
        Vector2 one = Vector2.Lerp(_plane.position,_aimTarget.position,0.111f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        Vector2 two = Vector2.Lerp(_plane.position,_aimTarget.position,0.666f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        EffectManager.GetInstance().DrawBezierLine(_plane.position,_aimTarget.position,one,two,0.05f);
    }

    public override bool SpecialAttack(Vector3 dir)
    {

        FindMultipleTarget(1.5f,120f);

        _aimObject.gameObject.SetActive(false);

        int count = 0;

        if(_multiTarget.Count != 0)
        {
            for(int i = 0; i < _maxTarget; ++i)
            {
                count = i >= _multiTarget.Count ? 0 : i;
                Vector3 pos = _multiTarget[count].position;
                var effect = EffectManager.GetInstance().AddEffect(pos,"PhantomString_Aim/LockOn",false,_multiTarget[count])
                            .RealTimeProgress()
                            .PassiveDeactive()
                            .DelayApear(i * 0.06f);
                effect.SetSortingOrder(10);
                _aimEffects.Add(effect);

                ++count;
            }
        }

        specAttack = true;
        _plane.SetControll(true);
        _plane.SetSpeed(0f);

        return false;
    }

    public void SpecialAttackProgress()
    {
        Timer.SetTimeScale(1f);
        specAttack = false;

        int count = 0;

        if(_multiTarget.Count != 0)
        {
            for(int i = 0; i < _maxTarget; ++i)
            {
                count = i >= _multiTarget.Count ? 0 : i;
                ((PlaneBase)(_multiTarget[count])).AddDelayAttackList(3,i * 0.07f,_plane,delayEvent);
                ++count;
            }
        }

        foreach(var effect in _aimEffects)
        {
            effect.SetActive(false);
            EffectManager.GetInstance().AddEffect(effect.position,"PhantomString_Aim/Shot").SetSortingOrder(10);
        }

        _aimEffects.Clear();
        _plane.SetControll(false);

        BurstAimDirection(24f,0.15f);
    }

    public void SpecialTargetHitEvent(PlaneBase target)
    {
        HitEffects(target.position);

        float dist = Vector2.Distance(_plane.position,target.position);
        Vector2 one = Vector2.Lerp(_plane.position,target.position,0.111f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        Vector2 two = Vector2.Lerp(_plane.position,target.position,0.666f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        EffectManager.GetInstance().DrawBezierLine(_plane.position,target.position,one,two,0.05f);
    }

    public void BurstAimDirection(float addForce, float time)
    {
        _plane.SetDirection(ControllerEx.GetInstance().centerAxis);
        _plane.SetAdditionalSpeed(addForce,time,true);
        _plane.SetAbsoluteForce(_plane.direction * 1000f);
        var ang = MathEx.directionToAngle(_plane.direction);
        // EffectManager.GetInstance().AddEffect(_plane.position,"Burst")
		// 					.SetAngle(ang);
        
        EffectManager.GetInstance().AddEffect(_plane.position - _plane.direction * 0.05f,"Weapon/PS/Boost1")
							.SetAngle(ang)
                            .SetDirection(-_plane.direction)
                            .SetSpeed(1f);

        for(int i = 0; i < 3; ++i)
            EffectManager.GetInstance().AddEffect(_plane.position - _plane.direction * 0.05f + MathEx.RandomCircle(0.1f),"Weapon/PS/Boost2")
                            .DelayApear(i * 0.03f)
                            .SetDirection(_plane.direction)
                            .SetSpeed(Random.Range(0.4f,1.3f));

    }

    public void HitEffects(Vector3 pos)
    {
        EffectManager.GetInstance().AddEffect(pos,"AttackHit_0").SetAngle(Random.Range(0f,360f));
		EffectManager.GetInstance().AddEffect(pos,"AttackHit_1").SetAngle(Random.Range(0f,360f));
    }

    public override bool CollisionCheck(PlaneBase target)
    {
        return false;
    }
    public override void WhenChanged()
    {
        base.WhenChanged();

        _aimObject.gameObject.SetActive(false);

        // if(_aimObj != null)
        //UnityEngine.GameObject.Destroy(_aimObj.gameObject);
    }

    public override void Change()
    {
        InitAimObject();

        MainHud.instance.MainUIAniSwap("Change","Weapon/PS",3);
        MainHud.instance.MainUIAniSwap("MainAttack","Weapon/PS",3);
        MainHud.instance.MainUIAniSwap("Boost","Weapon/PS",3);
        MainHud.instance.MainUIAniSwap("DriveOn","Weapon/PS",3);
        MainHud.instance.MainUIAniSwap("DriveAttack","Weapon/PS",3);
    }


    public Weapon_PhantomStinger(PlaneBase plane) : base(plane)
    {

    }
}
