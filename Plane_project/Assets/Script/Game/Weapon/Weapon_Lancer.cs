using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Lancer : WeaponBase
{
    private float _attackTime;
    private float _mainSpeed;
    private Vector3 _dodgeStartPos;

    private SpriteRenderer _aimObj;

    public override void Initialize()
    {
        base.Initialize();
        _attackTime = 0f;
        _mainSpeed = _plane.maxSpeed;
        _plane.SetImmortal(false);
        
        _icon = ResourceManager.GetInstance().GetSprite("UI/icon_lancer");
        _ui = ResourceManager.GetInstance().GetSprite("UI/ui_lancer");

        InitAimObject();
        _hideAimObject = true;
    }
    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);
        UpdateAimTarget(4.5f,30f);

        specAttack = false;

        if(_attackTime != 0f)
        {
            _plane.SetAbsoluteForce(_plane.direction * 10f);

			//Vector3 pos = _plane.position + new Vector3(Mathf.Sin(Random.Range(0,360f)),Mathf.Cos(Random.Range(0,360f)),0f) * 0.055f;
			//EffectManager.GetInstance().EmitParticles("AttackTrail",pos,-_plane.angle,1);

            if(CoolDownCheck(ref _attackTime,deltaTime))
            {
                _attackTime = 0f;
			    _mainTimer = mainCoolTime;
			    _plane.SetMaxSpeed(_mainSpeed);
			    mainAttack = false;
			    if(_plane.controllLockTimer == 0f)
                {
                    _plane.SetControll(false);
                }
                //_plane.SetImmortal(true);
			    //_plane.SetBodyAttack(0);
            }
        }
        else if (CoolDownCheck(ref _mainTimer,deltaTime))
        {
            _plane.SetImmortal(false);
			_plane.SetBodyAttack(5);
        }

        UpdateAim();
    }
    public override void MainAttack()
    {
        if(mainAttack)
            return;

        if(_aimTarget != null)
        {
            _plane.SetDirection((_aimTarget.position - _plane.position).normalized);
		    _plane.SetAngle(MathEx.directionToAngle(_plane.direction));

            // if(!_plane._directionAngle)
            // {

            // }
		    // if(_plane.direction.x < 0f)
		    // 	_plane.SetScale(1f,-1f,1f);
		    // else if(_plane.direction.x > 0f)
		    // 	_plane.SetScale(1f,1f,1f);
        }

        _plane.BurstActive();

        Vector3 pos = _plane.position;
		pos -= _plane.direction.normalized * 0.25f;
		EffectManager.GetInstance().AddEffect(_plane.position + _plane.direction.normalized * 0.2f,"Weapon/Lancer/Trail")
                                        .SetFps(14f)
										.SetAngle(MathEx.directionToAngle(_plane.direction));

        EffectManager.GetInstance().AddEffect(pos,"Weapon/Lancer/Chase",false,_plane)
                                        .SetFps(16f)
                                        .SetAddPoint(_plane.direction.normalized * 0.35f)
										.SetAngle(MathEx.directionToAngle(_plane.direction));

        _attackTime = .15f;
        _plane.SetMaxSpeed(_mainSpeed + 6f);
		mainAttack = true;
		_plane.SetControll(true);

        Timer.GetInstance().SetTimeScaleTimer(0.3f,0.5f,true);
    }
    public override bool SpecialAttack(Vector3 dir)
    {
        _dodgeStartPos = _plane.position;
        specAttack = true;

        Vector3 pos = _dodgeStartPos;

		EffectManager.GetInstance().AddEffect(pos + dir * (_plane._dodgeDist / 2f),"Weapon/Lancer/Drive")
                                    .SetFps(12f)
									.SetAngle(MathEx.directionToAngle(dir));

        return true;
    }
    public override bool CollisionCheck(PlaneBase target)
    {
        if(specAttack)
		{
			_plane.UpdateCollider();
			target.UpdateCollider();

			if(Define.SimpleCollider.CircleLineCircle(target.position,_dodgeStartPos,_plane.position,
								_plane.coll.bound.box.x * 2f, target.coll.bound.box.x))
			{
				_plane.SetImmortal(true);
				//_plane.Hit(target);
                target.DecreaseHP(15);
				_plane.SetImmortal(false);
				return true;
			}
		}
		else if(mainAttack)
		{
			_plane.SetImmortal(true);
			ObjectManager.GetInstance().UpdateStop(0.1f);
			EffectManager.GetInstance().AddEffect(target.position,"AttackHit_0").SetAngle(Random.Range(0f,360f));
			EffectManager.GetInstance().AddEffect(target.position,"AttackHit_1").SetAngle(Random.Range(0f,360f));
            _plane.Hit(target);
            _plane.SetImmortal(false);

            CameraControll.instance.Shake(0.2f, _plane.direction / 15f);

            return true;
		}


        return false;
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

        UnityEngine.GameObject.Destroy(_aimObj.gameObject);
    }

    public void UpdateAim()
    {
        Vector3 pos = _plane.position + _plane.direction * 0.4f;
        _aimObj.transform.position = pos;
        _aimObj.transform.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(_plane.direction));
    }

    public Weapon_Lancer(PlaneBase plane) : base(plane)
    {
        _aimObj = new GameObject("LancerAim").AddComponent<SpriteRenderer>();
        _aimObj.sprite = ResourceManager.GetInstance().GetSprite("center_lanceaim");

        UpdateAim();
    }

}
