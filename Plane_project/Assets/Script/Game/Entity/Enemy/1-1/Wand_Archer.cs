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

        LoadPlaneData("StarFish/Archer");
        //SetSpriteSet("StarFish/Marker",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        _dirSprites = ResourceManager.GetInstance().GetSpriteSet("SpriteSet/Planes/StarFish/Archer");
        _spriteAngle = 360f / _dirSprites.Length;

        UpdateSprite();

        _missile = ResourceManager.GetInstance().GetPrefab("Wand_Missile");
        del = ObjectCreateEvent;

        _mass = 5f;

        _minimapIcons[0] = ResourceManager.GetInstance().GetSprite("UI/MinimapIcon/map_eliteicon");
        _minimapIcons[1] = ResourceManager.GetInstance().GetSprite("UI/MinimapIcon/map_eliteicondown");
        _minimapIcons[2] = ResourceManager.GetInstance().GetSprite("UI/MinimapIcon/map_eliteiconup");
        miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sprite = _minimapIcons[0];
    }

    public override void deleteEvent()
    {
        base.deleteEvent();

        EffectManager.GetInstance().AddFakeLight(_position,Random.Range(5f,6f),.3f,new Color(1f,.1f,0f,.2f));
    }

    public override void progress(float deltaTime)
    {
        timer += deltaTime;
        if(timer >= 10f)
        {
            MissileActive();
            timer = 0f;
        }

        maxHp = _hp = 250;

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
        m.SetPositionEm(_position);
        m.SetDirection(dir);
        //m.SetAdditionalSpeed(1f,m.actTime,true);
        m.AddForce(dir * m.actTime * 6f);
    }
}
