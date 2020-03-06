using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_PhantomStinger : WeaponBase
{
    private SpriteRenderer _aimObj;
    private PlaneBase.DelayItem.hitEventDelegate delayEvent;

    private List<EffectBase> _aimEffects = new List<EffectBase>();

    int _maxTarget = 5;

    public override void Initialize()
    {
        base.Initialize();
        _icon = ResourceManager.GetInstance().GetSprite("UI/icon_phantomstinger");
        _ui = ResourceManager.GetInstance().GetSprite("UI/ui_phantomstinger");

        InitAimObject();

        immedyActiveSpecAttack = true;

        delayEvent = SpecialTargetHitEvent;
    }
    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        if(mainAttack)
        {
            _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));
            
            if(Input.GetKeyUp(KeyCode.W))
            {
                MainAttackProgress();
            }
        }
        else if(specAttack)
        {
            if(_multiTarget.Count > 0)
                _plane.SetAngle(MathEx.directionToAngle(GetAimTargetDirection()));

            if(Input.GetKeyUp(KeyCode.Mouse1))
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

        UpdateAim();
    }
    public override void MainAttack()
    {
        _aimObj.gameObject.SetActive(true);

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

    public void MainAttackProgress()
    {
        if(_aimTarget == null || _aimTarget.deleted)
        {
            Timer.GetInstance().SetTimeScale(1f);
            mainAttack = false;
            return;
        }
        _mainTimer = 0.3f;
        mainAttack = false;
        //_plane._rotateLock = true;
        Vector3 pos = _plane.position + _plane.direction * 0.25f;
        EffectManager.GetInstance().AddEffect(pos,"Fire").SetAngle(_plane.angle);

        HitEffects(_aimTarget.position);

        CameraControll.instance.Shake(0.2f, _plane.direction / 15f);

        BurstAimDirection(10f,0.08f);

        Timer.GetInstance().SetTimeScale(1f);
        _aimTarget.DecreaseHP(5);

        float dist = Vector2.Distance(_plane.position,_aimTarget.position);
        Vector2 one = Vector2.Lerp(_plane.position,_aimTarget.position,0.333f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        Vector2 two = Vector2.Lerp(_plane.position,_aimTarget.position,0.666f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        EffectManager.GetInstance().DrawBezierLine(_plane.position,_aimTarget.position,one,two,0.05f);
    }

    public override bool SpecialAttack(Vector3 dir)
    {
        _aimObj.gameObject.SetActive(true);

        FindMultipleTarget(1.5f,120f);

        int count = 0;

        if(_multiTarget.Count != 0)
        {
            for(int i = 0; i < _maxTarget; ++i)
            {
                count = i >= _multiTarget.Count ? 0 : i;
                Vector3 pos = _multiTarget[count].position;
                var effect = EffectManager.GetInstance().AddEffect(pos,"PhantomString_Aim/Appear",false,_multiTarget[count])
                            .RealTimeProgress()
                            .PassiveDeactive()
                            .DelayApear(i * 0.07f);
                effect.SetSortingOrder(10);
                _aimEffects.Add(effect);

                ++count;
            }
        }

        _aimObject.gameObject.SetActive(false);

        specAttack = true;
        _plane.SetControll(true);
        _plane.SetSpeed(0f);

        return false;
    }

    public void SpecialAttackProgress()
    {
        Timer.GetInstance().SetTimeScale(1f);
        specAttack = false;

        int count = 0;

        if(_multiTarget.Count != 0)
        {
            for(int i = 0; i < _maxTarget; ++i)
            {
                count = i >= _multiTarget.Count ? 0 : i;
                ((PlaneBase)(_multiTarget[count])).AddDelayAttackList(5,i * 0.07f,delayEvent);
                ++count;
            }
        }

        foreach(var effect in _aimEffects)
        {
            effect.SetActive(false);
            EffectManager.GetInstance().AddEffect(effect.position,"PhantomString_Aim/Disappear").SetSortingOrder(10);
        }

        _aimEffects.Clear();
        _plane.SetControll(false);

        BurstAimDirection(20f,0.1f);
    }

    public void SpecialTargetHitEvent(PlaneBase target)
    {
        HitEffects(target.position);

        float dist = Vector2.Distance(_plane.position,target.position);
        Vector2 one = Vector2.Lerp(_plane.position,target.position,0.333f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        Vector2 two = Vector2.Lerp(_plane.position,target.position,0.666f) + new Vector2(Random.Range(-dist,dist),Random.Range(-dist,dist));
        EffectManager.GetInstance().DrawBezierLine(_plane.position,target.position,one,two,0.05f);
    }

    public void BurstAimDirection(float addForce, float time)
    {
        _plane.SetDirection((CameraControll.instance.ScreenToWorldMouse() - _plane.position).normalized);
        _plane.SetAdditionalSpeed(addForce,time,true);
        _plane.SetAbsoluteForce(_plane.direction * 1000f);
        EffectManager.GetInstance().AddEffect(_plane.position,"Burst")
							.SetAngle(MathEx.directionToAngle(_plane.direction));
        
        _aimObj.gameObject.SetActive(false);
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

        // if(_aimObj != null)
        UnityEngine.GameObject.Destroy(_aimObj.gameObject);
    }

    public void UpdateAim()
    {
        Vector3 pos = _plane.position + _plane.direction * 0.4f;
        _aimObj.transform.position = pos;
        _aimObj.transform.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(_plane.direction));
    }

    public Weapon_PhantomStinger(PlaneBase plane) : base(plane)
    {
        _aimObj = new GameObject("PhantomAim").AddComponent<SpriteRenderer>();
        _aimObj.sprite = ResourceManager.GetInstance().GetSprite("center_phantomstepaim");

        UpdateAim();
    }
}
