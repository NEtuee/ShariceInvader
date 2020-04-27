using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory
{
    struct WeaponInfo
    {
        public WeaponBase weapon;
        public WeaponBase.WeaponList type;

        public WeaponInfo(WeaponBase w, WeaponBase.WeaponList t)
        {
            weapon = w;
            type = t;
        }
    }
    public bool mainAttack{get{return _currWeapon == null ? false : _currWeapon.mainAttack;}}
    public bool drievAttack{get{return _currWeapon == null ? false : _currWeapon.specAttack;}}
    public bool immedyActiveSpecAttack{get{return _currWeapon == null ? false : _currWeapon.immedyActiveSpecAttack;}}

    public float mainCooldown{get{return _currWeapon == null ? -1f : _currWeapon.mainCoolDown;}}

    PlaneBase _plane;

    List<WeaponInfo> _weaponList = new List<WeaponInfo>();
    WeaponBase _currWeapon;

    int pos = -1;

    public void WeaponChange(int p)
    {
        if(_weaponList.Count <= p)
        {
            Debug.Log("Weapon position Error : " + p);
            return;
        }
        else if(_weaponList.Count == 0)
        {
            Debug.Log("weapon does not exists");
            return;
        }

        if(_currWeapon != null)
			_currWeapon.WhenChanged();
		EffectManager.GetInstance().AddEffect(_plane.position,"WeaponChange",false,_plane).SetSortingOrder(1);
		_currWeapon = _weaponList[p].weapon;
		_currWeapon.SetTarget(_plane);
		_currWeapon.Initialize();
		_currWeapon.Change();
    }

    public void WeaponChange()
    {
        ++pos;
        pos = pos >= _weaponList.Count ? 0 : pos;
        WeaponChange(pos);
    }

    public void WeaponProgress(float deltaTime)
    {
        if(_currWeapon == null)
            return;
        
        _currWeapon.Progress(deltaTime);
    }

    public void MainAttack()
    {
        if(_currWeapon != null)
        {
            //if(_currWeapon.DecreaseMainGague())
                _currWeapon.MainAttack();
        }
    }

    public bool DriveAttack(Vector3 dir)
    {
        if(_currWeapon != null)
        {
            //if(_currWeapon.DecreaseDriveGague())
                return _currWeapon.SpecialAttack(dir);
        }
        return false;
    }

    public void GagueProgress(float deltaTime)
    {
        for(int i = 0; i < _weaponList.Count; ++i)
        {
            if(i == pos)
                continue;
            
            _weaponList[i].weapon.GagueCharge(deltaTime);
        }

    }

    public bool CurrWeaponExist() {return _currWeapon != null;}
    public bool CollisionCheck(PlaneBase target) {return _currWeapon.CollisionCheck(target);}

    public bool WeaponExist(WeaponBase.WeaponList type)
    {
        foreach(var item in _weaponList)
        {
            if(item.type == type)
                return true;
        }

        return false;
    }

    public void AddWeapon(WeaponBase.WeaponList type)
    {
        if(WeaponExist(type))
        {
            Debug.Log("exists");
            return;
        }

        switch(type)
        {
            case WeaponBase.WeaponList.Lancer:
            _weaponList.Add(new WeaponInfo(new Weapon_Lancer(_plane),type));
            break;
            case WeaponBase.WeaponList.Pulse:
            _weaponList.Add(new WeaponInfo(new Weapon_Test(_plane),type));
            break;
            case WeaponBase.WeaponList.PhantomStinger:
            _weaponList.Add(new WeaponInfo(new Weapon_PhantomStinger(_plane),type));
            break;
        }
    }

    public WeaponInventory(PlaneBase target) {_plane = target;}
}
