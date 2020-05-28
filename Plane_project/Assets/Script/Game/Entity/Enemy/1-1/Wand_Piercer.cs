using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Piercer : WandsBase
{
    public override void firstSetting()
    {
        base.firstSetting();

        LoadPlaneData("StarFish/Defender");
        //SetSpriteSet("StarFish/Marker",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));
        SetSpriteSet("SpriteSet/Planes/StarFish/starfish_piercer",AnimationType.None);

        // _dirSprites = ResourceManager.GetInstance().GetSpriteSet("StarFish/Defender",2);
        // _spriteAngle = 360f / _dirSprites.Length;
    }

    public override void initialize()
    {
        base.initialize();

        _mass = 5f;
        _gravityScale =.7f;
    }
}
