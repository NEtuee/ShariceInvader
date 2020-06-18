using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Defender : WandsBase
{
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

        maxHp = _hp = 50;
    }
}
