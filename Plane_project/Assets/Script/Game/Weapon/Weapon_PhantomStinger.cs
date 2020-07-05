using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PhantomStinger : WeaponBase
{
    private PlaneBase.hitEventDelegate delayEvent;

    private List<EffectBase> _aimEffects = new List<EffectBase>();

    private EffectBase _aimRange;
    private EffectBase _dashAim;

    int _maxTarget = 7;

    public override void Initialize()
    {
        base.Initialize();

        mainCoolTime = 0.15f;


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

        _aimRange.SetAngle(_plane.angle);
        _aimRange.SetPositionEm(_plane.position);

        if(mainAttack)
        {
            _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));

            _dashAim.SetPositionEm(_plane.position + (Vector3)ControllerEx.GetInstance().centerAxis);
            _dashAim.SetAngle(MathEx.directionToAngle(ControllerEx.GetInstance().centerAxis));
            if(ControllerEx.GetInstance().KeyUp("MainAttack"))
            {
                _dashAim.SetActive(false);
                MainAttackProgress();
            }
        }
        else if(specAttack)
        {
            if(_multiTarget.Count > 0)
                _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));

            _dashAim.SetPositionEm(_plane.position + (Vector3)ControllerEx.GetInstance().centerAxis);
            _dashAim.SetAngle(MathEx.directionToAngle(ControllerEx.GetInstance().centerAxis));
            if(ControllerEx.GetInstance().KeyUp("DriveAttack"))
            {
                _dashAim.SetActive(false);
                SpecialAttackProgress();
            }
        }
        else
        {
            UpdateAimTarget(1.5f,40f);
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
            SoundManager.instance.Play("SE/PSTarget",false,-1,1f,false);

            _dashAim = EffectManager.GetInstance().AddEffect(_plane.position,"UI/Weapon/PS/DashAim",false);
            _dashAim.PassiveDeactive();
            _dashAim.RealTimeProgress();

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

        _aimRange.Play(false);

        EffectManager.GetInstance().AddEffect(_aimTarget.position,"SpriteSet/Effects/PhantomString_Aim/Shot").SetSortingOrder(10);

        _mainTimer = mainCoolTime;
        mainAttack = false;
        //_plane._rotateLock = true;
        Vector3 pos = _plane.position + _plane.direction * 0.25f;
        EffectManager.GetInstance().AddEffect(pos,"SpriteSet/Effects/Fire").SetAngle(_plane.angle);

        HitEffect(_aimTarget);

        CameraControll.instance.Shake(0.2f, _plane.direction / 15f);

        BurstAimDirection(15f,0.15f);

        Timer.SetTimeScale(1f);
        _aimTarget.Hit(50,_plane);

        float dist = Vector2.Distance(_plane.position,_aimTarget.position);
        Vector2 one = Vector2.Lerp(_plane.position,_aimTarget.position,0.111f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        Vector2 two = Vector2.Lerp(_plane.position,_aimTarget.position,0.666f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        EffectManager.GetInstance().DrawBezierLine(_plane.position,_aimTarget.position,one,two,0.05f);

        SoundManager.instance.Play("SE/PS_",false,4);
        SoundManager.instance.Play("SE/MainHit_",false,2,.5f,false);
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
                var effect = EffectManager.GetInstance().AddEffect(pos,"SpriteSet/Effects/PhantomString_Aim/LockOn",false,_multiTarget[count])
                            .RealTimeProgress()
                            .PassiveDeactive()
                            .DelayApear((float)i * 0.06f);
                effect.SetSortingOrder(10);
                _aimEffects.Add(effect);

                ++count;
            }
        }

        for(int i = 0; i < _multiTarget.Count; ++i)
        {
            SoundManager.instance.PlayRequest("SE/PSTarget",-1,1f,false,(float)i * 0.06f);
        }

        _dashAim = EffectManager.GetInstance().AddEffect(_plane.position,"UI/Weapon/PS/DashAim",false);
        _dashAim.PassiveDeactive();
        _dashAim.RealTimeProgress();

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
                ((PlaneBase)(_multiTarget[count])).AddDelayAttackList(50,i * 0.07f,_plane,delayEvent);
                ++count;
            }
        }

        foreach(var effect in _aimEffects)
        {
            effect.SetActive(false);
            EffectManager.GetInstance().AddEffect(effect.position,"SpriteSet/Effects/PhantomString_Aim/Shot").SetSortingOrder(10);
        }

        _aimEffects.Clear();
        _plane.SetControll(false);

        BurstAimDirection(24f,0.15f);
    }

    public void SpecialTargetHitEvent(PlaneBase target)
    {
        HitEffect(target);

        float dist = Vector2.Distance(_plane.position,target.position);
        Vector2 one = Vector2.Lerp(_plane.position,target.position,0.111f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        Vector2 two = Vector2.Lerp(_plane.position,target.position,0.666f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        EffectManager.GetInstance().DrawBezierLine(_plane.position,target.position,one,two,0.05f);

        SoundManager.instance.Play("SE/PS_",false,4);
        SoundManager.instance.Play("SE/MainHit_",false,2,.5f,false);

        CameraControll.instance.Zoom(2.8f);
    }

    public void BurstAimDirection(float addForce, float time)
    {
        _plane.SetDirection(ControllerEx.GetInstance().centerAxis);
        _plane.SetAdditionalSpeed(addForce,time,true);
        _plane.SetAbsoluteForce(_plane.direction * 1000f);
        var ang = MathEx.directionToAngle(_plane.direction);
        // EffectManager.GetInstance().AddEffect(_plane.position,"Burst")
		// 					.SetAngle(ang);
        
        EffectManager.GetInstance().AddEffect(_plane.position - _plane.direction * 0.05f,"SpriteSet/Effects/Weapon/PS/Boost1")
							.SetAngle(ang)
                            .SetDirection(-_plane.direction)
                            .SetSpeed(1f);

        for(int i = 0; i < 3; ++i)
            EffectManager.GetInstance().AddEffect(_plane.position - _plane.direction * 0.05f + MathEx.RandomCircle(0.1f),"SpriteSet/Effects/Weapon/PS/Boost2")
                            .DelayApear(i * 0.03f)
                            .SetDirection(_plane.direction)
                            .SetSpeed(Random.Range(0.4f,1.3f));

        SoundManager.instance.Play("SE/BurstActive_",false,2);

    }

    public override bool CollisionCheck(PlaneBase target)
    {
        return false;
    }
    public override void WhenChanged()
    {
        base.WhenChanged();

        _aimObject.gameObject.SetActive(false);

        _aimRange.gameObject.SetActive(false);
        // if(_aimObj != null)
        //UnityEngine.GameObject.Destroy(_aimObj.gameObject);

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
            ani.CopyAnimation("Burst","SpriteSet/Effects/Weapon/PS/Burst");
            ani.CopyAnimation("Loop","SpriteSet/Effects/Weapon/PS/Loop");

            if(ani.currAni == "Loop")
                ani.ChangeAni("Loop",true,false);
        }

        InitAimObject();

        _aimRange = EffectManager.GetInstance().AddEffect(_plane.position,"UI/Weapon/PS/Change",false,null);
        _aimRange.PassiveDeactive();
        _aimRange.SetAngle(_plane.angle);

        MainHud.instance.MainUIAniSwap("Change","");
        MainHud.instance.MainUIAniSwap("MainAttack","");
        MainHud.instance.MainUIAniSwap("Boost","");
        MainHud.instance.MainUIAniSwap("DriveOn","");
        MainHud.instance.MainUIAniSwap("DriveAttack","");
    }


    public Weapon_PhantomStinger(PlaneBase plane) : base(plane)
    {

    }
}
