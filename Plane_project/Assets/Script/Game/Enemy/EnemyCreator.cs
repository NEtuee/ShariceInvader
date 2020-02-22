using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyCreator
{
    public static void BoomDrone(int count, Vector2 mapPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();
        int r = 0;
        BoomDrone next = null;
        BoomDrone first = null;
        for(int i = 0; i < count; ++i)
        {
			BoomDrone boom = _objManager.AddObject<BoomDrone>(Define.ObjectType.enemy,"BoomDrone" + i);
			boom.SetPosition(_objManager._place.MapPosToWorldPos(mapPos));
            boom.SetMaxSpeed(2.5f + i * 0.025f);
            boom.standPos = next;

            r = Random.Range(0,5);

            if(first == null)
                first = boom;

            //if(i <= 10)
            next = next == null ? boom : r == 2 ? next : r == 1 ? first : boom;
        }
    }

    public static void ShootingDrone(int count, Vector2 mapPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        for(int i = 0; i < count; ++i)
        {
			ShootingDrone boom = _objManager.AddObject<ShootingDrone>(Define.ObjectType.enemy,"ShootingDrone" + i);
			boom.SetPosition(_objManager._place.MapPosToWorldPos(mapPos));
        }
    }

    public static void CCTV(int shootingDroneCount, Vector2 mapPos)
    {
        ObjectManager _objManager = ObjectManager.GetInstance();

        PlaneBase cctv = _objManager.AddObject<CCTV>(Define.ObjectType.enemy,"CCTV");
        cctv.SetPosition(_objManager._place.MapPosToWorldPos(mapPos));

        for(int i = 0; i < shootingDroneCount; ++i)
        {
			ShootingDrone boom = _objManager.AddObject<ShootingDrone>(Define.ObjectType.enemy,"ShootingDrone" + i);
			boom.SetPosition(_objManager._place.MapPosToWorldPos(mapPos + new Vector2(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f))));
        }
    }
}
