using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : PlaneBase
{
    public Vector3 standPos;

    float timer;
    float shotTimer = 0f;
    float flash = 0f;
    float standTimer = 0f;
    int flashCount = 0;
    int count = 0;

    public bool act = false;

    public ObjectBase standBase = null;

    private Sprite _baseSprite;

    AnimationControllEx _base;

	public override void firstSetting()
	{
		base.firstSetting();
		//SetSpriteSet("ShootingDrone",AnimationType.Vertical);
        _aniType = AnimationType.None;
        SetSprite("SpriteSet/Planes/ShootingDrone/Shadow");
        
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		_maxSpeed = 1.5f;
		_speed = .035f;
        _gravityScale = 0.5f;

        //standPos = ObjectManager.GetInstance()._place.mainPlace.leftBottom;
        
        SetDeco();
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);
        _velocityFlip = false;
        _directionAngle = false;
        _rotateLock = true;
		maxHp = _hp = 50;
        timer = Random.Range(0f,.4f);
        standPos = position;

        _bodyAttack = 1;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);
	}

    public override ObjectBase SetPositionEm(Vector3 pos) 
    {
        base.SetPositionEm(pos);
        standPos = position;
        return this;
    }

	public override void progress(float deltaTime)
	{
        if(!_fall)
            AirStand();

        standTimer += 90f * deltaTime;
        standTimer = standTimer >= 360f ? standTimer - 360f : standTimer;
        float sin = Mathf.Sin(standTimer * Mathf.Deg2Rad);
        float cos = Mathf.Cos(standTimer * Mathf.Deg2Rad);

        _direction = (standPos + new Vector3(sin * 0.5f,cos * 0.1f,0f) - position).normalized;


        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);
	}

    public void AirStand()
    {
        _direction = (standPos - position).normalized;
    }

    public void SetDeco()
    {
        _base = _deco.AddDeco(Vector2.zero);


        _base._sprRenderer.sprite = _baseSprite = ResourceManager.GetInstance().GetSprite("SpriteSet/Planes/ShootingDrone/Base");
        _base._sprRenderer.sortingOrder = -1;
        
        SpriteRenderer spr = new GameObject("Edge").AddComponent<SpriteRenderer>();
        spr.sprite = ResourceManager.GetInstance().GetSprite("SpriteSet/Planes/ShootingDrone/Edge");
        spr.sortingOrder = 1;
        spr.transform.SetParent(_base._sprRenderer.transform);
    }
}
