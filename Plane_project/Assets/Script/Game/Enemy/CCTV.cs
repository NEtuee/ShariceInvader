using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : PlaneBase
{

    LineRenderer line;

    Vector2 point0;
    Vector2 point1;

    float recogDist = 4f;
    public float mainAngle = 0f;
    float plusAngle = 15f;

    float timer = 0f;
    float alertTimer = 0f;

    float xRand = 0f;

    Color maxColor;
    bool act = false;
    bool burstCall = false;

    float targetDist = 0f;

    Vector3 _camPos;
    Vector3 standPos;
    Vector3 targetPos;
    Vector3 lastPos;

	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("CCTVDrone",AnimationType.Vertical_Velocity);
	
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));

		_maxSpeed = 3f;
		_speed = .2f;
        _gravityScale = 0.2f;

        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 3;

        line.SetPosition(0,new Vector2());
        line.SetPosition(1,new Vector2());
        line.SetPosition(2,new Vector2());
        
        line.startWidth = 0.025f;
        line.endWidth = 0.025f;

        line.material = ResourceManager.GetInstance().GetMaterial("SpriteDefault");

        Color c = new Color(1f,1f,1f,.3f);

        line.startColor = c;
        line.endColor = c;

        maxColor = new Color(1f,0f,0f,0.3f);

        _camPos = new Vector3(0f,-0.1f,0f);

        miniMapIcon.gameObject.SetActive(false);
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);
        _rotateLock = true;
        _directionAngle = true;
		_hp = 15;

        xRand = Random.Range(-1f,1f);

        RegisteCollisionList();
	}

    public override ObjectBase SetPosition(Vector3 pos) 
    {
        base.SetPosition(pos);
        standPos = position;
        return this;
    }

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

	public override void progress(float deltaTime)
	{
        PointUpdate();

        if(!_fall)
            AirStand();

        targetPos = GameManager.instance.player.position;

        bool inArea = InAreaCheck();
        act = act ? true : inArea;

        if(act)
        {
            float ang = 0f;
            _maxSpeed = 3f;

            if(!inArea)
            {
                standPos = lastPos;

                ang = MathEx.directionToAngle((lastPos - position).normalized);
            }
            else
            {
                timer += 90f * deltaTime;
                timer = MathEx.clamp360Degree(timer);
                float t = timer * Mathf.Deg2Rad;

                Vector3 circlePos = new Vector3(Mathf.Sin(t),Mathf.Cos(t)) * .1f;
                Vector3 dir = new Vector3();

                
                if(targetDist < 1f)
                    dir = (_position - targetPos).normalized * 1f;
                else
                    dir = GameManager.instance.player.velocity.normalized;

                if(position.y - targetPos.y < 1f)
                {
                    dir.y = 3f;
                    dir = dir.normalized;
                }
                    
                standPos = targetPos + dir + circlePos;
                standPos.x += xRand;
                standPos.y += 0.8f;

                ang = MathEx.directionToAngle((targetPos - position).normalized);

                lastPos = targetPos;

                alertTimer += deltaTime;

                if(alertTimer >= 4f)
                {
                    Delete();

                    EnemyCreator.BoomDrone(4,ObjectManager.GetInstance()._place.WorldPosToMapPos(_position));
                }
            }

            mainAngle = Mathf.LerpAngle(mainAngle,ang,deltaTime * 20f);

        }
        else
        {
            alertTimer = 0f;

            // standPos = _position + Vector3.right;
            // standPos.y = 6f;

            // _maxSpeed = 1.5f;

            mainAngle = MathEx.directionToAngle(_direction);
        }

        line.startColor = Color.Lerp(Color.white,maxColor,alertTimer / 3f);
        line.endColor = line.startColor;

        // if(!burstCall)
        // {
        //     float a = 0f;
        //     Vector3 pos = GameManager.instance.player.position;

        //     if(!act)
        //     {
        //         timer += 0.25f * deltaTime;
        //         a = 130f * Mathf.Sin(timer) + 90f;

        //         float d = Vector2.Distance(position,pos);

        //         float dot = Mathf.Cos(Mathf.Deg2Rad * plusAngle);
        //         Vector3 dir = (pos - position).normalized;

        //         mainAngle = Mathf.Lerp(mainAngle,a,0.05f);

        //         if(d <= recogDist)
        //         {
        //             if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * mainAngle)) > dot)
        //             {
        //                 act = true;
        //                 maxColor = Color.red;
        //                 alertTimer = 0f;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         a = MathEx.directionToAngle((pos - position).normalized);

        //         float d = Vector2.Distance(position,pos);

        //         alertTimer += deltaTime;

        //         if(alertTimer >= 2f)
        //         {
        //             burstCall = true;
        //             line.enabled = false;
        //             var p = position;
        //             p.y = 3f;
        //             EnemyCreator.BoomDrone(13,ObjectManager.GetInstance()._place.WorldPosToMapPos(p));
        //         }

        //         mainAngle = Mathf.LerpAngle(mainAngle,a,0.2f);

        //         if(d > recogDist)
        //         {
        //             act = false;
        //             mainAngle += -(float)((int)mainAngle / 360) * 360;

        //             alertTimer = 0.2f;

        //             maxColor = Color.white;
        //         }
        //     }

        //     line.startColor = Color.Lerp(Color.white,maxColor,alertTimer / 2f);
        //     line.endColor = line.startColor;
        // }

        //GameManager.instance.player.Collision(this);
        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);
	}

    public void AirStand()
    {
        _direction = (standPos - position).normalized;
    }

    public bool InAreaCheck()
    {
        targetDist = Vector2.Distance(_camPos + _position,targetPos);

        float dot = Mathf.Cos(Mathf.Deg2Rad * plusAngle);
        Vector3 dir = (targetPos - (_camPos + _position)).normalized;

        if(targetDist <= recogDist)
        {
            if(Vector3.Dot(dir,MathEx.angleToDirection(Mathf.Deg2Rad * mainAngle)) > dot)
            {
                //Debug.Log("in Area");
                return true;
            }
        }

        return false;
    }

    public void PointUpdate()
    {
        Vector3 pos = _camPos + _position;
        point0 = pos + MathEx.angleToDirection(Mathf.Deg2Rad * (mainAngle - plusAngle)) * recogDist;
        point1 = pos + MathEx.angleToDirection(Mathf.Deg2Rad * (mainAngle + plusAngle)) * recogDist;
        line.SetPosition(0,point0);
        line.SetPosition(1,pos);// + MathEx.angleToDirection(Mathf.Deg2Rad * mainAngle)* 0.3f);
        line.SetPosition(2,point1);
    }

}