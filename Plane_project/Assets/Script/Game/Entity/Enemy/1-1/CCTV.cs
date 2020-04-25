using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : PlaneBase
{

    LineRenderer line;

    Vector2 point0;
    Vector2 point1;

    float recogDist = 5.5f;
    public float mainAngle = 0f;
    float plusAngle = 30f;

    float timer = 0f;
    float alertTimer = 0f;
    float spawnTimer = 0f;

    float xRand = 0f;

    bool act = false;

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
        
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;

        line.material = ResourceManager.GetInstance().GetPixelSnapMaterial();

        Color c = new Color(1f,1f,1f,.3f);

        line.startColor = c;
        line.endColor = c;

        _camPos = new Vector3(0f,-0.1f,0f);

        miniMapIcon.gameObject.SetActive(false);
        miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sprite = ResourceManager.GetInstance().GetSprite("UI/map_eliteicon");

        //line.enabled = false;

        PhysicsDebugSetup();
	}

	public override void initialize()
	{
		BasicInitialize();
		
		SetNoClip(false);
        _velocityFlip = false;
        _directionAngle = true;
		maxHp = _hp = 45;

        spawnTimer = Random.Range(1.5f,2f);

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

    public override void WhenDecreaseHP(int d)
    {
        base.WhenDecreaseHP(d);
    }

	public override void progress(float deltaTime)
	{
        PointUpdate();

        if(!_fall)
            AirStand();

        targetPos = Player.instance.position;

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

                
                if(targetDist < 2f)
                    dir = (_position - targetPos).normalized * 1f;
                else
                    dir = Player.instance.velocity.normalized;

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

                if(alertTimer >= 15f)
                {
                    Delete();

                    EnemyCreator.BoomDrone(4,ObjectManager.GetInstance()._place.WorldPosToMapPos(_position));
                }
            }

            mainAngle = ang;//Mathf.LerpAngle(mainAngle,ang,deltaTime * 20f);

            spawnTimer -= deltaTime;
            if(spawnTimer <= 0f)
            {
                float x = Random.Range(-3f,3f);
                x += x > 0 ? -7.2f : 7.2f;

                float y = Random.Range(-3f,3f);
                y += y > 0 ? -3.6f : 3.6f;
                y += y < 1f ? 3.6f : 0f;
                
                int enemy = MathEx.RandomInt(0,2);

                switch(enemy)
                {
                case 0:
                    EnemyCreator.ShootingDrone(1,GetSpawnPos() + targetPos + MathEx.RandomVector3(-0.3f,0.3f),targetPos);
                    EnemyCreator.ShootingDrone(1,GetSpawnPos() + targetPos + MathEx.RandomVector3(-0.3f,0.3f),targetPos);
                    EnemyCreator.ShootingDrone(1,GetSpawnPos() + targetPos + MathEx.RandomVector3(-0.3f,0.3f),targetPos);
                    break;
                case 1:
                    Vector3 pos = GetSpawnPos() + targetPos;
                    EnemyCreator.RayDrone(1, pos + MathEx.RandomVector3(-0.4f,0.4f));
                    EnemyCreator.RayDrone(1, pos + MathEx.RandomVector3(-0.1f,0.1f));
                    break;
                }

                spawnTimer = Random.Range(2f,2.8f);
            }

        }
        else
        {
            alertTimer = 0f;

            // standPos = _position + Vector3.right;
            // standPos.y = 6f;

            // _maxSpeed = 1.5f;

            mainAngle = MathEx.directionToAngle(_direction);
        }

        // line.startColor = Color.Lerp(Color.white,maxColor,alertTimer / 15f);
        // line.endColor = line.startColor;

        BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
        
		BasicUpdate(deltaTime);

        PhysicsDebugUpdate();
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

    public Vector3 GetSpawnPos()
    {
        float x = Random.Range(-3f,3f);
        x += x > 0 ? -7.2f : 7.2f;

        float y = Random.Range(-3f,3f);
        y += y > 0 ? -3.6f : 3.6f;
        y += y < 1f ? 3.6f : 0f;
        
        return new Vector3(x,y);
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