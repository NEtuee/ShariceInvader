using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Lancer : WeaponBase
{
    private float _attackTime;
    private float _mainSpeed;
    private Vector3 _dodgeStartPos;

    private EffectBase _uiArrow;
    private EffectBase _backUi;
    private EffectBase _driveArrow;

    private EffectBase _attackRange;

    public override void Initialize()
    {
        base.Initialize();

        mainCoolTime = 0.1f;
        specCoolTime = 0.5f;
        _mainTimer = 0f;
        _specTimer = 0f;

        _attackTime = 0f;
        _mainSpeed = _plane.maxSpeed;
        
        _icon = ResourceManager.GetInstance().GetSprite("UI/icon_lancer");
        _ui = ResourceManager.GetInstance().GetSprite("UI/ui_lancer");

        _hideAimObject = true;

        GagueSetup(1f,5f,15f,5f);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);
        UpdateAimTarget(4.5f,30f);

        if(_attackTime != 0f)
        {
            _plane.SetAbsoluteForce(_plane.direction * 13f);

            if(CoolDownCheck(ref _attackTime,deltaTime))
            {
                _attackTime = 0f;
			    _mainTimer = mainCoolTime;
			    _plane.SetMaxSpeed(_mainSpeed);
                _plane.coll.bound.SetRect(.05f,.05f);
			    mainAttack = false;
                _plane.SetImmortal(false);
			    if(_plane.controllLockTimer == 0f)
                {
                    _plane.SetControll(false);
                }

            }
        }
        else if (CoolDownCheck(ref _mainTimer,deltaTime))
        {
            _plane.SetImmortal(false);
			_plane.SetBodyAttack(5);
        }

        UiUpdate();

    }
    public override bool MainAttack()
    {
        if(mainAttack)
            return true;

        if(_aimTarget != null)
        {
            _plane.SetDirection((_aimTarget.position - _plane.position).normalized);
		    _plane.SetAngle(MathEx.directionToAngle(_plane.direction));
        }

        _plane.coll.bound.SetRect(.1f,.1f);

        _plane.BurstActive();

        _uiArrow.Play(false);
        _backUi.Play(false);



        _attackTime = .18f;
        _plane.SetMaxSpeed(_mainSpeed + 5f);
		mainAttack = true;
		_plane.SetControll(true);
        _plane.SetImmortal(true);

        return true;
    }
    public override bool SpecialAttack(Vector3 dir)
    {
        _backUi.sprRenderer.enabled = true;

        float da = _driveArrow.angle;
        _driveArrow.SetActive(false);
        _driveArrow = null;

        _dodgeStartPos = _plane.position;

        Vector3 pos = _plane.position;
		
		_plane.SetPosition(pos + dir * (_plane._dodgeDist - .2f));
        EffectManager.GetInstance().AddEffect(_plane.position - dir * 0.3f,"Weapon/Lancer/Drive")
                                    .RealTimeProgress()
                                    .SetDisableEvent(ApearArrow)
									.SetAngle(MathEx.directionToAngle(dir));

        Define.ObjectType t = _plane.type == Define.ObjectType.enemy ? Define.ObjectType.player : Define.ObjectType.enemy;
        var list = CollisionManager.GetInstance().GetCollisionList(t);

        if(list != null)
        {
            int count = list.Count;


            for(int i = 0 ; i < count; ++i)
            {
                _plane.UpdateCollider();
                list[i].UpdateCollider();
    
                if(Define.SimpleCollider.CircleLineCircle(list[i].position,_dodgeStartPos,_plane.position,
							_plane.coll.bound.box.x * 6f, list[i].coll.bound.box.x))
		        {
                    ((PlaneBase)list[i]).Hit(15,_plane);
		        }
            }
        }

        _plane.SetAdditionalSpeed(12f,0.2f,true);
        _plane.AddForce(dir * 100f);
        
        _plane.SpriteDisapear(0.1f);
        CameraControll.instance.FollowDelay(0.5f);

        EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/DriveArrow/End",false,_plane,1)
                        .RealTimeProgress()
                        .SetAddPoint(_plane.direction * 0.25f)
                        .SetAngle(da);

        _attackRange.SetActive(false);
        _attackRange = EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/MainRange",false,null,3);
        _attackRange.PassiveDeactive();
        _attackRange.RealTimeProgress();


        return false;
    }

    public void ApearArrow()
    {
        _uiArrow.sprRenderer.enabled = true;
    }

    public void UiUpdate()
    {
        _uiArrow.SetAngle(_plane.angle);
        _uiArrow.SetPosition(_plane.position + _plane.direction * 0.35f);

        _backUi.SetAngle(_plane.angle);
        _backUi.SetPosition(_plane.position - _plane.direction * 0.25f);

        _attackRange.SetAngle(_plane.angle);
        _attackRange.SetPosition(_plane.position + _plane.direction * 0.57f);

        if(_driveArrow != null)
        {
            _driveArrow.SetAngle(_plane.angle);
            _driveArrow.SetPosition(_plane.position + _plane.direction * 0.25f);
        }
    }

    public override void DriveOn()
    {
        _driveArrow = EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/DriveArrow/Start",false,null,1);
        _driveArrow.PassiveDeactive();
        _driveArrow.RealTimeProgress();

        _uiArrow.sprRenderer.enabled = false;
        _backUi.sprRenderer.enabled = false;

        _attackRange.SetActive(false);
        _attackRange = EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/DriveRange",false,null,3);
        _attackRange.PassiveDeactive();
        _attackRange.RealTimeProgress();
    }

    public override bool CollisionCheck(PlaneBase target)
    {
        if(mainAttack)
		{
			ObjectManager.GetInstance().UpdateStop(0.1f);
			EffectManager.GetInstance().AddEffect(target.position,"AttackHit_0").SetAngle(Random.Range(0f,360f));
			EffectManager.GetInstance().AddEffect(target.position,"AttackHit_1").SetAngle(Random.Range(0f,360f));
            _plane.Hit(target);

            CameraControll.instance.Shake(0.2f, _plane.direction / 15f);

            return true;
		}


        return false;
    }

    public override void Change()
    {
        foreach(var ani in _plane._boostAni)
        {
            ani.CopyAnimation("Burst","Effects/Weapon/Lancer/Burst");
            ani.CopyAnimation("Loop","Effects/Weapon/Lancer/Loop");

            if(ani.currAni == "Loop")
                ani.ChangeAni("Loop",true,false);
        }

        _uiArrow = EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/DirectionArrow",false,null,1,false);
        _uiArrow.PassiveDeactive();
        _uiArrow.RealTimeProgress();

        _backUi = EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/Back",false,null,3,false);
        _backUi.PassiveDeactive();
        _backUi.RealTimeProgress();

        _attackRange = EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/Lancer/MainRange",false,null,3);
        _attackRange.PassiveDeactive();
        _attackRange.RealTimeProgress();

        MainHud.instance.MainUIAniSwap("Change",null);
        MainHud.instance.MainUIAniSwap("MainAttack",null);
        MainHud.instance.MainUIAniSwap("Boost",null);
        MainHud.instance.MainUIAniSwap("DriveOn",null);
        MainHud.instance.MainUIAniSwap("DriveAttack",null);
    }

    public override void WhenChanged()
    {
        base.WhenChanged();

        _attackTime = 0f;
		_mainTimer = mainCoolTime;
		_plane.SetMaxSpeed(_mainSpeed);
		mainAttack = false;
		_plane.SetControll(false);
		_plane.SetImmortal(false);
		_plane.SetBodyAttack(5);
        _plane.coll.bound.SetRect(.05f,.05f);
        
        _uiArrow.SetActive(false);
        _uiArrow.sprRenderer.enabled = true;
        _backUi.SetActive(false);
        _backUi.sprRenderer.enabled = true;

        _attackRange.SetActive(false);

        if(_driveArrow != null)
            _driveArrow.SetActive(false);

        foreach(var ani in _plane._boostAni)
        {
            ani.CopyAnimation("Burst",ani.aniOriginPath["Burst"]);
            ani.CopyAnimation("Loop",ani.aniOriginPath["Loop"]);

            if(ani.currAni == "Loop")
                ani.ChangeAni("Loop",true,false);
        }
    }

    public Weapon_Lancer(PlaneBase plane) : base(plane)
    {

    }

}
