using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Test : WeaponBase
{
    private Define.SimpleCollider _collider;

    public override void Initialize()
    {
        base.Initialize();
        _collider = new Define.SimpleCircleCollider(0.76f,0.76f,Vector2.zero);
        _plane.SetImmortal(false);

        _icon = ResourceManager.GetInstance().GetSprite("UI/icon_nova");
        _ui = ResourceManager.GetInstance().GetSprite("UI/ui_nova");

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
			_plane.SetBodyAttack(5);
        }
    }
    public override void MainAttack()
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
		    		EffectManager.GetInstance().AddEffect(list[i].position,"AttackHit_0").SetAngle(Random.Range(0f,360f));
		    		EffectManager.GetInstance().AddEffect(list[i].position,"AttackHit_1").SetAngle(Random.Range(0f,360f));
    
                    CameraControll.instance.Shake(0.2f, _plane.direction / 15f);
    
                    ((PlaneBase)list[i]).Hit(_plane);

                    atk = true;
                }
            }
        }

        if(atk)
        {
            mainAttack = true;
            EffectManager.GetInstance().AddEffect(_plane.position,"Weapon/EnergyBurst",false)
                                .SetSortingOrder(1);
            //Timer.GetInstance().SetTimeScaleTimer(0.3f,0.5f,true);
        }
        else
        {
            //_plane.BurstActive();
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
                    obj.ControllLock(1f);
    
                    EffectManager.GetInstance().AddEffect(obj.position,"Electric",false,obj).SetSortingOrder(1).SetAngle(Random.Range(0f,360f));
                    EffectManager.GetInstance().AddEffect(obj.position,"Burst",false)
                                            .SetAngle(MathEx.directionToAngle(d));
                }
            }

            link = link.next;
        }

        specAttack = true;

        Timer.GetInstance().SetTimeScaleTimer(0.1f,0.3f);

        return false;
    }
    public override void WhenChanged()
    {
        base.WhenChanged();

        _plane.SetImmortal(false);
		_plane.SetBodyAttack(5);
    }
    public override bool CollisionCheck(PlaneBase target)
    {
        return false;
    }

    public Weapon_Test(PlaneBase plane) : base(plane)
    {
        
    }
}
