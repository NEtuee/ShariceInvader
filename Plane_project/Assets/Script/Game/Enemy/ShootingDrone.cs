using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingDrone : PlaneBase
{
    Vector3 standPos;

    float timer;
    float shotTimer = 0f;
    float flash = 0f;
    float standTimer = 0f;
    int flashCount = 0;
    int count = 0;

    bool act = false;

    public ObjectBase standBase = null;

    private Transform gunPoint;
    private Transform shotPoint;
    private MeshRenderer leftArm;
    private MeshRenderer rightArm;
    

	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("ShootingDrone",AnimationType.Vertical);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		_maxSpeed = 1.5f;
		_speed = .035f;
        _gravityScale = 0.5f;

        //standPos = ObjectManager.GetInstance()._place.mainPlace.leftBottom;
        
        _trail.gameObject.transform.position = new Vector2(0f,-0.1f);

        GunPointSetup();
        
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);
        _rotateLock = true;
        _directionAngle = true;
		_hp = 5;
        timer = Random.Range(0f,.4f);
        standPos = position;

        _trail.time = 0.2f;

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
        GunPointUpdate();

        if(!_fall)
            AirStand();

        if(act)
        {
            timer -= deltaTime;
            if(timer <= 0f)
            {
                timer = Random.Range(3f,4f);

                float d = Vector2.Distance(standPos,position);

                if(d <= 1f)
                {
                    d = Vector2.Distance(position,GameManager.instance.player.position);

                    if(d <= 5f)
                    {
                        flash = 0.2f;
                        flashCount = 4;
                    }
                    else
                    {
                        act = false;

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

            float d = Vector2.Distance(position,GameManager.instance.player.position);
            if(d <= 2.5f)
            {
                act = true;
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
                    standPos = GameManager.instance.player.position + new Vector3(Random.Range(-.2f,.2f),Random.Range(-.2f,.2f));
                }
            }
        }

        if(count != 0)
        {
            shotTimer -= deltaTime;
            if(shotTimer <= 0f)
            {
                --count;
                shotTimer = .3f;
                Vector3 dir = (GameManager.instance.player.position - position).normalized;
                BulletManager.GetInstance().Active(BulletType.enemy,shotPoint.position,dir,3.5f,0,5f).SetAngle(MathEx.directionToAngle(dir));
                EffectManager.GetInstance().AddEffect(shotPoint.position + dir * 0.1f,"Fire").SetAngle(MathEx.directionToAngle(dir));
            }
        }

        if(act || count != 0f)
        {
            Vector3 dir = (GameManager.instance.player.position - position).normalized;
            Vector3 ang = gunPoint.transform.localEulerAngles;
            ang.z = MathEx.directionToAngle(dir);
            gunPoint.transform.localEulerAngles = ang;
        }

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);
	}

    public void AirStand()
    {
        _direction = (standPos - position).normalized;
    }

    public override void afterUpdateTransform()
    {
        GunPointUpdate();

        if(!act)
            return;

        if(_direction.x < 0)
        {
            gunPoint.localScale = new Vector3(-1f,1f,1f);
            Vector3 ang = gunPoint.transform.localEulerAngles;
            gunPoint.transform.localEulerAngles = -ang;
        }
        else
        {
            gunPoint.localScale = new Vector3(1f,1f,1f);
        }
    }

    public void GunPointSetup()
    {
        gunPoint = Instantiate(ResourceManager.GetInstance().GetPrefab("GunPoint")).transform;
        gunPoint.SetParent(transform);
        gunPoint.transform.position = new Vector3(0f,-0.0186f,0f);

        leftArm = gunPoint.Find("LeftArm").GetComponent<MeshRenderer>();
        rightArm = gunPoint.Find("RightArm").GetComponent<MeshRenderer>();
        shotPoint = gunPoint.Find("ShotPoint");
    }

    public void GunPointUpdate()
    {
        Vector3 ang = gunPoint.transform.localEulerAngles;
        ang.y = (float)_spritePoint * _spriteAngle;
        gunPoint.transform.localEulerAngles = ang;

        int sortingOrder = _sprRenderer.sortingOrder;
        int add = _direction.x < 0 ? -1 : 1;


        if(ang.y < 90f)
        {
            rightArm.sortingOrder = sortingOrder -1;// (1 * add);
            leftArm.sortingOrder = sortingOrder + 1;//(1 * add);
        }
        else if(ang.y < 270f)
        {
            rightArm.sortingOrder = sortingOrder + 1;//(1 * add);
            leftArm.sortingOrder = sortingOrder - 1;//(1 * add);
        }
        else if(ang.y < 360f)
        {
            rightArm.sortingOrder = sortingOrder - (1 * add);
            leftArm.sortingOrder = sortingOrder + (1 * add);
        }
    }
}
