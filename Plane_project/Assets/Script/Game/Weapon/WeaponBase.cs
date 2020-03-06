using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase
{
    protected PlaneBase _plane;
    protected PlaneBase _aimTarget = null;
    protected Sprite _icon = null;
    protected Sprite _ui = null;

    public float mainCoolDown{get{return _mainTimer;}}
    public float specCoolDown{get{return _specTimer;}}

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

    protected SpriteRenderer _aimObject = null;
	private AnimationControll _aimAni;

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
		    	_aimAni.AnimationProgress(ref _aimObject,deltaTime);
		    }
        }
    }
    public abstract void MainAttack();
    public abstract bool SpecialAttack(Vector3 dir);
    public abstract bool CollisionCheck(PlaneBase target);
    public void Change()
    {
        MainHud.instance.WeaponChange(_icon,_ui);
    }
    public virtual void WhenChanged()
    {
        if(_aimObject != null)
        {
            //_aimObject.gameObject.SetActive(false);
            GameObject.Destroy(_aimObject);
            _aimAni = null;
        }
    }
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

        Define.ObjectType t = _plane.type == Define.ObjectType.enemy ? Define.ObjectType.player : Define.ObjectType.enemy;

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
            EffectManager.GetInstance().AddEffect(_aimTarget.position,"PhantomString_Aim/Disappear").SetSortingOrder(10);
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
        Transform obj = _plane.transform.Find("Aim");
        if(obj == null)
        {
            _aimObject = new GameObject("Aim").AddComponent<SpriteRenderer>();;
		    _aimObject.sortingOrder = 10;
           // _aimObject.transform.SetParent(_plane.transform);
        }
        else
        {
            _aimObject = obj.GetComponent<SpriteRenderer>();
            _aimObject.gameObject.SetActive(true);
        }

        _aimAni = new AnimationControll();
		_aimAni.AddAnimation("On","Effects/PhantomString_Aim/Appear");
		_aimAni.SetFps(18);

        _canAim = true;
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
