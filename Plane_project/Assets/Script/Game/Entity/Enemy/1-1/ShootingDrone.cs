using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingDrone : PlaneBase
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
    AnimationControllEx _shotPoint;

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

        _base.ChangeAni("Active",false);

        RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

    public override ObjectBase SetPosition(Vector3 pos) 
    {
        base.SetPosition(pos);
        standPos = position;
        return this;
    }

	public override void progress(float deltaTime)
	{
        if(!_fall)
            AirStand();

        _deco.DecoAniProgress(deltaTime);
        if(_shotPoint.isEnd)
        { 
            _shotPoint.Stop();
            _shotPoint._sprRenderer.enabled = false;
        }

        if(act)
        {
            timer -= deltaTime;
            if(timer <= 0f)
            {
                timer = Random.Range(3f,4f);

                float d = Vector2.Distance(standPos,position);

                if(d <= 2f)
                {
                    d = Vector2.Distance(position,Player.instance.position);

                    if(d <= 5f)
                    {
                        flash = 0.2f;
                        flashCount = 4;
                    }
                    else
                    {
                        act = false;
                        
                        _base.Stop();
                        _base._sprRenderer.sprite = _baseSprite;

                        if(standBase == null || standBase.deleted)
                        {
                            standPos = _position;
                        }
                        else
                        {
                            standPos = standBase.position + new Vector3(Random.Range(-.5f,.5f),0f);
                        }
                    }
                }
            }
        }
        else
        {
            standTimer += 90f * deltaTime;
            standTimer = standTimer >= 360f ? standTimer - 360f : standTimer;
            float sin = Mathf.Sin(standTimer * Mathf.Deg2Rad);
            float cos = Mathf.Cos(standTimer * Mathf.Deg2Rad);

            _direction = (standPos + new Vector3(sin * 0.5f,cos * 0.1f,0f) - position).normalized;

            float d = Vector2.Distance(position,Player.instance.position);
            if(d <= 2.5f)
            {
                act = true;

                _base.ChangeAni("Active",false);
            }
        }


        if(flash != 0f)
        {
            flash -= deltaTime;
            if(flash <= 0f)
            {
                if(flashCount-- != 0)
                {
                    _sprRenderer.color = _sprRenderer.color.g == 1f ? new Color(1f,0f,0f) : new Color(1f,1f,1f);
                    flash = 0.1f;
                }
                else
                {
                    flash = 0f;
                    flashCount = 0;
                    count = 4;
                    standPos = Player.instance.position + new Vector3(Random.Range(-.2f,.2f),Random.Range(-.2f,.2f));
                }
            }
        }

        Vector3 dir = (Player.instance.position - position).normalized;

        if(count != 0)
        {
            shotTimer -= deltaTime;
            if(shotTimer <= 0f)
            {
                --count;
                if(!controllLock)
                {
                    shotTimer = .3f;
                    BulletManager.GetInstance().Active(BulletType.enemy,_position + dir * 0.2f,dir,3.5f,0,5f).SetAngle(MathEx.directionToAngle(dir));
                    //EffectManager.GetInstance().AddEffect(_position + dir * 0.2f,"ShootingDrone/Fire",false,null,2).SetAngle(MathEx.directionToAngle(dir));
                    _shotPoint.ChangeAni("Fire",false);
                    _shotPoint._sprRenderer.enabled = true;
                }
                
            }
        }

        _base._sprRenderer.transform.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(dir));

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

        _base.AddAnimation("Active","SpriteSet/Planes/ShootingDrone/Open");
        _base.Stop();

        _base._sprRenderer.sprite = _baseSprite = ResourceManager.GetInstance().GetSprite("SpriteSet/Planes/ShootingDrone/Base");
        _base._sprRenderer.sortingOrder = -1;
        
        SpriteRenderer spr = new GameObject("Edge").AddComponent<SpriteRenderer>();
        spr.sprite = ResourceManager.GetInstance().GetSprite("SpriteSet/Planes/ShootingDrone/Edge");
        spr.sortingOrder = 1;
        spr.transform.SetParent(_base._sprRenderer.transform);

        _shotPoint = _deco.AddDeco(new Vector2(0.2f,0f));

        _shotPoint.AddAnimation("Fire","SpriteSet/Planes/ShootingDrone/Fire");
        _shotPoint.Stop();
        _shotPoint._sprRenderer.transform.SetParent(_base._sprRenderer.transform);
    }
}
