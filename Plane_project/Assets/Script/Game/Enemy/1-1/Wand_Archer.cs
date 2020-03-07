using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Archer : WandsBase
{
    public float timer;

    private GameObject _missile;
    private ObjectManager.DelayObjectCreateEventDelegate del;
    private bool _dir = false;
    public override void firstSetting()
    {
        base.firstSetting();

        LoadPlaneData("Boss_1-1/Marker");
        //SetSpriteSet("Boss_1-1/Marker",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        _dirSprites = ResourceManager.GetInstance().GetSpriteSet("Boss_1-1/Marker",2);
        _spriteAngle = 360f / _dirSprites.Length;

        UpdateSprite();

        _missile = ResourceManager.GetInstance().GetPrefab("Wand_Missile");
        del = ObjectCreateEvent;
    }

    public override void progress(float deltaTime)
    {
        timer += deltaTime;
        if(timer >= 10f)
        {
            MissileActive();
            timer = 0f;
        }

        base.progress(deltaTime);
    }

    public void MissileActive()
    {
        for(int i = 0; i < 8; ++i)
        {
            ObjectManager.GetInstance().AddObjectDelayed(i * 0.1f,"missile",_missile,Define.ObjectType.enemy,del);
        }
    }

    public void ObjectCreateEvent(ObjectBase obj)
    {
        var m = (Wand_Missile)obj;


        Vector3 dir = Vector3.Cross(MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad),new Vector3(0f,0f,-1f));

        dir = dir * (_dir ? 1f : -1f);
        _dir = !_dir;

        m.actTime = Random.Range(0.2f,0.7f);
        m.SetPosition(_position);
        m.SetDirection(dir);
        //m.SetAdditionalSpeed(1f,m.actTime,true);
        m.AddForce(dir * m.actTime * 6f);
    }
}
