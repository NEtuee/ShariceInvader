using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandsBase : PlaneBase
{
    public override void firstSetting()
    {
        base.firstSetting();


        Debug.Log("one");
        // LoadPlaneData("Boss_1-1/Marker");
        // //SetSpriteSet("Boss_1-1/Marker",AnimationType.None);
		// SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

        // _dirSprites = ResourceManager.GetInstance().GetSpriteSet("Boss_1-1/Marker",2);
        // _spriteAngle = 360f / _dirSprites.Length;

        // UpdateSprite();
    }

    public float mainAngle = 0f;
    public Vector2 targetPos;
    public bool act = false;
    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

    public override void initialize()
    {
        BasicInitialize();

        //_direction = Vector3.left;

        _directionAngle = false;
        _velocityFlip = false;
        _rotateLock = true;

        _speed = 0.2f;
        _maxSpeed = 3.2f; 
        _gravityScale = 0f;

        _hp = 30;


        RegisteCollisionList();

    }

    public void UpdateSprite()
    {
        if(deleted)
            return;
        
        mainAngle = MathEx.clamp360Degree(mainAngle);
        _spritePoint = (int)(mainAngle / _spriteAngle);

        sprRenderer.sortingOrder = mainAngle >= 180f ? 1 : -1;

        SetSprite(_dirSprites[_spritePoint]);
    }

    public override void progress(float deltaTime)
    {
        if(!act)
        {
            _position = targetPos;// Vector3.Lerp(_position,targetPos,0.2f);
        }
        
        UpdateSprite();
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }
}
