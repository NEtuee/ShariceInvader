using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PhantomStinger : WeaponBase
{

    

    public override void Initialize()
    {
        base.Initialize();
        MainHud.instance.wpIcon.sprite = ResourceManager.GetInstance().GetSprite("UI/icon_phantomstinger");
        InitAimObject();
        
    }
    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(mainAttack)
        {
            _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));
            
            if(Input.GetKeyUp(KeyCode.W))
            {
                _mainTimer = 0.1f;
                mainAttack = false;
                //_plane._rotateLock = true;
                Vector3 pos = _plane.position + _plane.direction * 0.25f;
                EffectManager.GetInstance().AddEffect(pos,"Fire").SetAngle(_plane.angle);
                EffectManager.GetInstance().AddEffect(_aimTarget.position,"AttackHit_0").SetAngle(Random.Range(0f,360f));
				EffectManager.GetInstance().AddEffect(_aimTarget.position,"AttackHit_1").SetAngle(Random.Range(0f,360f));

                CameraControll.instance.Shake(0.2f, _plane.direction / 15f);

                _plane.SetDirection((CameraControll.instance.ScreenToWorldMouse() - _plane.position).normalized);
                _plane.SetAdditionalSpeed(3f,0.1f,true);
                _plane.SetAbsoluteForce(_plane.direction * 50f);
                EffectManager.GetInstance().AddEffect(_plane.position,"Burst")
									.SetAngle(MathEx.directionToAngle(_plane.direction));

                Timer.GetInstance().SetTimeScale(1f);
                _aimTarget.DecreaseHP(5);
            }
        }
        else
        {
            UpdateAimTarget(1.5f,80f);
        }

        if(CoolDownCheck(ref _mainTimer,deltaTime))
        {
            _plane._rotateLock = true;
            _plane._rotateLock = true;
        }
    }
    public override void MainAttack()
    {
        if(_aim)
        {
            _plane.SetSpeed(0f);
            _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));

            Timer.GetInstance().SetTimeScale(0.2f);
            mainAttack = true;
        }
        else
        {
            _plane.BurstActive();
        }
    }
    public override bool SpecialAttack(Vector3 dir)
    {
        return true;
    }
    public override bool CollisionCheck(PlaneBase target)
    {
        return false;
    }
    public override void WhenChanged()
    {
        base.WhenChanged();
    }

    public Weapon_PhantomStinger(PlaneBase plane) : base(plane)
    {
    
    }
}
