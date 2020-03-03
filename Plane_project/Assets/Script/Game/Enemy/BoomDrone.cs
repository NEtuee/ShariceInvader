using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomDrone : PlaneBase
{
    public ObjectBase standPos;

    bool act = false;
    bool master = false;

    bool empStun = false;

    float timer = 0f;

    EffectBase effect;

	public override void firstSetting()
	{
		base.firstSetting();
        SetVerticalAngledCount(32,4);

		SetSpriteSet("BoomDrone",AnimationType.Vertical_Angled);
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
		_hp = 1;

        standPos = this;
        master = false;

        _trail.gameObject.transform.position = new Vector2(0f,-0.05f);
        _trail.time = 0.4f;

        _boostSpr.enabled = false;

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);

	}

    public override void WhenDecreaseHP()
    {
        if(empStun && (_fall || deleted))
        {
            float d = Vector2.Distance(position,standPos.position);

            if(d <= 1.5f)
            {
                GameManager.instance.player.ControllLock(1f);
                GameManager.instance.player.AddForce(-GameManager.instance.player.velocity / 1.5f);
            }

            empStun = false;

            effect.sprRenderer.color = Color.white;
            effect.SetTimer(0.1f);

            act = false;
        }
    }

	public override void progress(float deltaTime)
	{            
        if(standPos == null || standPos.deleted || standPos == this && !master)
        {
            standPos = GameManager.instance.player;
            master = true;
            _maxSpeed = Random.Range(3f,3.5f);
		    _speed = Random.Range(.2f,.25f);
        }

        if(act && !_fall)
        {
            float d = Vector2.Distance(position,GameManager.instance.player.position);

            if(d <= 1.4f && !empStun && master)
            {
                timer += deltaTime;


                if(timer >= 1f)
                {
                    timer = 0f;
                    empStun = true;

                    effect = EffectManager.GetInstance().AddEffect(_position,"CircleEffect",false,this).SetFps(0f);
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

            Color c = effect.sprRenderer.color;

            c.a = Mathf.Sin((timer * 720f) * Mathf.Deg2Rad);

            if(timer >= 0.8f)
            {
                DecreaseHP(_hp);
                c.a = 1f;
                effect.SetTarget(null);
                effect.SetSortingOrder(1);
            }

            effect.sprRenderer.color = c;
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
