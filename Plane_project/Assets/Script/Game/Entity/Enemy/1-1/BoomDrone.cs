using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomDrone : PlaneBase
{
    public ObjectBase standPos;

    bool act = false;
    bool master = false;

    bool empStun = false;
    bool stunCharge = false;

    float timer = 0f;

    EffectBase rangeCircle = null;
    EffectBase charge = null;
    Color32 startColor = new Color32(77,136,235,255);

	public override void firstSetting()
	{
		base.firstSetting();
        SetVerticalAngledCount(64,4);

		SetSpriteSet("SpriteSet/Planes/BoomDrone",AnimationType.Vertical_Angled);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));



		_maxSpeed = 3.4f;
		_speed = .2f;
        _gravityScale = 0.3f;

        //_trail.gameObject.SetActive(false);

        _sprRenderer.flipX = true;
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);
        _velocityFlip = false;
        _directionAngle = true;
		maxHp = _hp = 20;

        standPos = this;
        master = false;
        stunCharge = false;
        
        _boostAniProgress = false;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);

        if(rangeCircle != null)
            rangeCircle.SetActive(false);
	}

    public override void WhenDecreaseHP(int i)
    {
        if(empStun && (_fall || deleted))
        {
            float d = Vector2.Distance(position,standPos.position);

            if(d <= 1.5f)
            {
                Player.instance.ControllLock(timer * .5f);
                Player.instance.AddForce(-Player.instance.velocity / 1.5f);
            }

            EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Effects/ElectricBoom/explosion",false);

            charge.SetActive(false);
            empStun = false;
            act = false;
        }
    }

	public override void progress(float deltaTime)
	{            
        if((standPos == null || standPos.deleted || standPos == this) && !master && !deleted)
        {
            standPos = Player.instance;
            master = true;
            _maxSpeed = Random.Range(3f,3.5f);
		    _speed = Random.Range(.2f,.25f);

            rangeCircle = EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Effects/CircleRange",true,this);
            rangeCircle.sprRenderer.color = startColor;
        }

        if(act && !_fall)
        {
            float d = Vector2.Distance(position,Player.instance.position);

            if(d <= 1.4f && !empStun && master)
            {
                timer += deltaTime;


                if(timer >= 1f)
                {
                    timer = 0f;
                    empStun = true;

                    charge = EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Effects/ElectricBoom/charge",false,this);
                }
            }

            if(d >= 10f)
            {
                if(standPos == null || standPos.deleted)
                {
                    standPos = this;
                }
                act = false;
            }
        }
        else
        {
            float d = Vector2.Distance(position,standPos.position);
            if(d <= 10f)
            {
                act = true;
                timer = 0f;
                empStun = false;
            }
        }

        if(empStun && act && !_fall && !deleted)
        {
            timer += deltaTime;

            if(master)
            {
                rangeCircle.sprRenderer.color = Color32.Lerp(startColor,new Color32(255,255,0,255),timer * 0.5f);
            }
            if(timer >= 2f)
            {
                stunCharge = true;
                timer = 2f;
                DecreaseHP(_hp);
                //EffectManager.GetInstance().AddEffect(_position,"ElectricBoom/explosion",false);

                if(rangeCircle != null)
                    rangeCircle.SetActive(false);
            }

        }

        if(!_fall)
            AirStand();

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);
	}

    public void AirStand()
    {
        _direction = (standPos.position + new Vector3(Random.Range(-0.25f,0.25f),Random.Range(-0.25f,0.25f)) - position).normalized;
    }
}
