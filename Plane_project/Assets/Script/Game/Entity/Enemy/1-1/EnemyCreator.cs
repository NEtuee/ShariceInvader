using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyCreator
{
    public static void BoomDrone(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();
        int r = 0;
        BoomDrone next = null;
        BoomDrone first = null;
        for(int i = 0; i < count; ++i)
        {
			BoomDrone boom = _objManager.AddObject<BoomDrone>(Define.ObjectType.enemy,"BoomDrone" + i);
			boom.SetPositionEm(worldPos);
            boom.SetMaxSpeed(2.5f + i * 0.025f);
            boom.standPos = next;

            r = Random.Range(0,5);

            if(first == null)
                first = boom;

            //if(i <= 10)
            next = next == null ? boom : r == 2 ? next : r == 1 ? first : boom;
        }
    }

    public static void BoostDrone(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();
        int r = 0;
        BoostDrone next = null;
        BoostDrone first = null;
        for(int i = 0; i < count; ++i)
        {
			BoostDrone boost = _objManager.AddObject<BoostDrone>(Define.ObjectType.enemy,"BoostDrone" + i);
			boost.SetPositionEm(worldPos);
            boost.SetMaxSpeed(2.5f + i * 0.025f);
            boost.TargetPosUpdate();
            boost.target = next;

            r = Random.Range(0,5);

            if(first == null)
                first = boost;

            //if(i <= 10)
            next = next == null ? boost : r == 2 ? next : r == 1 ? first : boost;
        }
    }

    public static void RayDrone(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			RayDrone ray = _objManager.AddObject<RayDrone>(Define.ObjectType.enemy,"RayDrone" + i);
			ray.SetPositionEm(worldPos);
        }
    }

    public static void MissileDrone(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			MissileDrone boom = _objManager.AddObject<MissileDrone>(Define.ObjectType.enemy,"MissileDrone" + i);
			boom.SetPositionEm(worldPos);
        }
    }

    public static void Missile(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			NPC boom = _objManager.AddObject<NPC>(Define.ObjectType.enemy,"Missile" + i);
			boom.SetPositionEm(worldPos);
        }
    }

    public static void LaserDrone(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			LaserDrone boom = _objManager.AddObject<LaserDrone>(Define.ObjectType.enemy,"LaserDrone" + i);
			boom.SetPositionEm(worldPos);
        }
    }

    public static void ShootingDrone(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			ShootingDrone boom = _objManager.AddObject<ShootingDrone>(Define.ObjectType.enemy,"ShootingDrone" + i);
			boom.SetPositionEm(worldPos);
            boom.standPos = Player.instance.position + MathEx.RandomCircle(3f);//boom.position;
            if(boom.standPos.y <= 2f)
            {
                boom.standPos.y = Random.Range(2.5f,3f);
            }
        }
    }

    public static void ShootingDrone(int count, Vector2 worldPos, Vector2 targetPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			ShootingDrone boom = _objManager.AddObject<ShootingDrone>(Define.ObjectType.enemy,"ShootingDrone" + i);
			boom.SetPositionEm(worldPos);
            boom.standPos = targetPos;
            boom.standPos.y = targetPos.y <= 2f ? Random.Range(2.5f,4f) : targetPos.y;
            boom.act = true;
        }
    }

    public static void Dummy(int count, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			Dummy boom = _objManager.AddObject<Dummy>(Define.ObjectType.enemy,"ShootingDrone" + i);
			boom.SetPositionEm(worldPos);
            boom.standPos = Player.instance.position + MathEx.RandomCircle(5f);//boom.position;
            if(boom.standPos.y <= 2f)
            {
                boom.standPos.y = Random.Range(2.5f,3f);
            }
        }
    }

    public static void CCTV(int shootingDroneCount, Vector2 worldPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        PlaneBase cctv = _objManager.AddObject<CCTV>(Define.ObjectType.enemy,"CCTV");
        cctv.SetPositionEm(worldPos);

        for(int i = 0; i < shootingDroneCount; ++i)
        {
			ShootingDrone boom = _objManager.AddObject<ShootingDrone>(Define.ObjectType.enemy,"ShootingDrone" + i);
			boom.SetPositionEm(worldPos + new Vector2(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f)));
        }
    }

    public static void LaserIndicator(Vector2 worldPos, float maxSpeed, float apearTime)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

		LaserIndicator laser = _objManager.AddObject<LaserIndicator>(Define.ObjectType.enemy,"LaserIndicator");
		laser.SetPositionEm(worldPos);
        laser.SetMaxSpeed(maxSpeed);
        laser.apearTimer = apearTime;
    }
}
