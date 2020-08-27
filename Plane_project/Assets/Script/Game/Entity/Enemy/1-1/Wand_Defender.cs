using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Defender : WandsBase
{
    public static int guardFactor = 0;
    public static float guardTimer = 0f;
    public override void firstSetting()
    {
        base.firstSetting();

        LoadPlaneData("StarFish/Defender");
        //SetSpriteSet("StarFish/Marker",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        _dirSprites = ResourceManager.GetInstance().GetSpriteSet("SpriteSet/Planes/StarFish/Defender");
        _spriteAngle = 360f / _dirSprites.Length;

        UpdateSprite();

        _mass = 5f;

        maxHp = _hp = 300;

        hpChangeEvent += HitEvent;
    }

    public override void deleteEvent()
    {
        base.deleteEvent();

        EffectManager.GetInstance().AddFakeLight(_position,Random.Range(5f,6f),.3f,new Color(1f,.1f,0f,.2f));
    }

    public void HitEvent()
    {
        _hp += guardFactor;
        guardFactor += 20;
        guardFactor = guardFactor >= 50 ? 50 : guardFactor;
        guardTimer = .5f;
    }
}
