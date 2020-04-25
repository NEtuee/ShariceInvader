using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Defender : WandsBase
{
    public override void firstSetting()
    {
        base.firstSetting();

        LoadPlaneData("Boss_1-1/Defender");
        //SetSpriteSet("Boss_1-1/Marker",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        _dirSprites = ResourceManager.GetInstance().GetSpriteSet("Boss_1-1/Defender",2);
        _spriteAngle = 360f / _dirSprites.Length;

        UpdateSprite();
    }
}
