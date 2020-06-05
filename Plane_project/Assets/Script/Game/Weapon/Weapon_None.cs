using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_None : WeaponBase
{
    public override void Initialize()
    {
        base.Initialize();
        MainHud.instance.SetNull();
    }
    public override void Progress(float deltaTime)
    {
        
    }
    public override bool MainAttack()
    {
        return true;
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

    public Weapon_None(PlaneBase plane) : base(plane)
    {
    
    }
}
