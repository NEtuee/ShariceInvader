using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase
{
    public enum WeaponList
    {
        Lancer,
        Pulse,
        PhantomStinger
    };

    protected PlaneBase _plane;
    public PlaneBase _aimTarget = null;
    public Sprite _icon = null;
    public Sprite _ui = null;

    public float mainCoolDown{get{return _mainTimer;}}
    public float specCoolDown{get{return _specTimer;}}
    public float gague{get{return _currGague;}}

    public bool mainAttack;
    public bool specAttack;
    public bool immedyActiveSpecAttack = false;


    public float mainCoolTime = 0f;
    public float specCoolTime = 0f;
    protected float _mainTimer = 0f;
    protected float _specTimer = 0f;
    protected bool _aim = false;
    protected bool _canAim = false;
    protected bool _hideAimObject = false;

    protected float _maxGague;
    protected float _currGague;
    protected float _chargeSpeed;

    protected float _mainAttackGague;
    protected float _driveAttackGague;

    protected static SpriteRenderer _aimObject = null;
	protected static AnimationControllEx _aimAni;

    public virtual void Initialize()
    {
        mainCoolTime = 0.5f;
        specCoolTime = 0.5f;
        _mainTimer = 0f;
        _specTimer = 0f;

        mainAttack = false;
        specAttack = false;
    }
    public virtual void Progress(float deltaTime)
    {
        if(_canAim)
        {
            if(_aimTarget != null && !_hideAimObject)
		    {
		    	_aimObject.transform.position = _aimTarget.position;
		    	_aimAni.AnimationProgress(Timer.noneScaledDeltaTime);
		    }
        }
    }
    public abstract bool MainAttack();
    public abstract bool SpecialAttack(Vector3 dir);
    public abstract bool CollisionCheck(PlaneBase target);

    public void GagueSetup(float main, float drive, float max, float speed)
    {
        _mainAttackGague = main;
        _driveAttackGague = drive;
        _maxGague = max;
        _currGague = max;
        _chargeSpeed = speed;;
    }

    public float GetWeaponGaguePercentage() {return _currGague / _maxGague;}

    public bool DecreaseMainGague()
    {
        return DecreaseGague(_mainAttackGague);
    }

    public bool DecreaseDriveGague()
    {
        return DecreaseGague(_driveAttackGague);
    }

    public bool DecreaseGague(float value)
    {
        if(_currGague <= 0f)
            return false;
        _currGague -= value;
        return true;
    }

    public void GagueCharge(float deltaTime)
    {
        _currGague += _chargeSpeed * deltaTime;
        if(_currGague >= _maxGague)
        {
            _currGague = _maxGague;
        }
    }
    public virtual void Change()
    {

    }
    public virtual void WhenChanged()
    {

    }

    public virtual void DriveOn(){}

    public void SetTarget(PlaneBase plane) {_plane = plane;}

    public virtual void PropelRelase()
    {
        
    }

    public Vector3 GetAimTargetDirection()
    {
        return _aimTarget == null ? Vector3.zero : (_aimTarget.position - _plane.position).normalized;
    }

    public void UpdateAimTarget(float aimDist, float aimAngle)
	{
		if(_plane.place == null)
		{
			_aimTarget = null;
			return;
		}

		var target = FindSingleTarget(aimDist,aimAngle);

        if(_aimTarget != target && target != null)
        {
            if(!_hideAimObject)
            {
                _aimObject.transform.position = target.position;
		        _aimAni.ChangeAni("On",false);
		        _aimObject.gameObject.SetActive(true);
            }
        }

		if(target == null && !_hideAimObject)
			_aimObject.gameObject.SetActive(false);

        if(!_hideAimObject && _aimTarget != null && _aimTarget != target)
        {
            EffectManager.GetInstance().AddEffect(_aimTarget.position,"SpriteSet/Effects/PhantomString_Aim/Miss").SetSortingOrder(10);
        }

		if(target != null)
        {
            _aimTarget = (PlaneBase)target;
        }
        else
            _aimTarget = null;
        
        _aim = _aimTarget != null;
	}

    Define.SimpleCircleCollider distColl = new Define.SimpleCircleCollider(0f,0f,Vector2.zero);
    public ObjectBase FindSingleTarget(float aimDist, float aimAngle)
    {
        Define.ObjectType t = _plane.type == Define.ObjectType.enemy ? Define.ObjectType.player : Define.ObjectType.enemy;

		var link = ObjectManager.GetInstance().GetFirstLink(t);//place.GetLinkToType(Define.ObjectType.enemy);
        
		ObjectBase target = null;
		float dist = aimDist + 1f;

        while(link != null)
		{
            float dot = Mathf.Cos(Mathf.Deg2Rad * aimAngle);
            Vector3 dir = (link.target.position - _plane.position).normalized;

            if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * _plane.angle)) > dot)
            {
                float d = Vector2.Distance(link.target.position,_plane.position);
			    var p = (PlaneBase)link.target;

                p.UpdateCollider();
                distColl.Setup(aimDist,aimDist,_plane.position);

                if(p.coll.CollisionCheck(distColl))
                {
                    if(dist > d)
                    {
                        dist = d;
			            target = link.target;
                    }
                }
            }

			link = link.next;
		}

        return target;
    }

    protected List<ObjectBase> _multiTarget = new List<ObjectBase>();
    public void FindMultipleTarget(float aimDist, float aimAngle)
    {
        _multiTarget.Clear();

        Define.ObjectType t = _plane.type == Define.ObjectType.enemy ? Define.ObjectType.player : Define.ObjectType.enemy;

		var link = ObjectManager.GetInstance().GetFirstLink(t);
		float dist = aimDist + 1f;

        while(link != null)
		{
            float dot = Mathf.Cos(Mathf.Deg2Rad * aimAngle);
            Vector3 dir = (link.target.position - _plane.position).normalized;

            if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * _plane.angle)) > dot)
            {
                float d = Vector2.Distance(link.target.position,_plane.position);
			    var p = (PlaneBase)link.target;

                p.UpdateCollider();
                distColl.Setup(aimDist,aimDist,_plane.position);

                if(p.coll.CollisionCheck(distColl))
                {
                    if(dist > d)
                    {
                        _multiTarget.Add(link.target);
                    }
                }
            }

			// float d = Vector2.Distance(link.target.position,_plane.position);
			// if(d <= aimDist)
			// {
            //     float dot = Mathf.Cos(Mathf.Deg2Rad * aimAngle);
            //     Vector3 dir = (link.target.position - _plane.position).normalized;


            //     if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * _plane.angle)) > dot)
            //     {
            //         _multiTarget.Add(link.target);
            //     }
			// }

			link = link.next;
		}

    }

    public void InitAimObject()
    {
        if(_aimObject == null)
        {
            _aimObject = new GameObject("Aim").AddComponent<SpriteRenderer>();;
		    _aimObject.sortingOrder = 10;
            _aimAni = new AnimationControllEx(_aimObject);
		    _aimAni.AddAnimation("On","SpriteSet/Effects/PhantomString_Aim/Appear");
            _aimAni.AddAnimation("Lock","SpriteSet/Effects/PhantomString_Aim/LockOn");
           // _aimObject.transform.SetParent(_plane.transform);
        }
        else
        {
            _aimObject.gameObject.SetActive(false);
        }

        _canAim = true;
    }

    public virtual void HitEffect(PlaneBase target)
    {
        if(!target.immortal)
        {
            EffectManager.GetInstance().AddEffect(target.position,"SpriteSet/Effects/AttackHit_0").SetAngle(Random.Range(0f,360f));
		    EffectManager.GetInstance().AddEffect(target.position,"SpriteSet/Effects/AttackHit_1").SetAngle(Random.Range(0f,360f));
        }
    }

    public virtual bool CoolDownCheck(ref float timer, float deltaTime)
    {
        if(timer > 0f)
        {
            timer -= deltaTime;
            if(timer <= 0f)
            {
                timer = 0f;
                return true;
            }
        }

        return false;
    }

    public WeaponBase(PlaneBase plane) 
    {
        if(plane != null)
        {
            SetTarget(plane);
            Initialize();
        }
    }
}
