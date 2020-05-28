using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    ObjectManager obj;
    PlaceMapper place;
    public TextMesh text;

    Dictionary<int,string> objects = new Dictionary<int, string>();
    int pos = 0;
    int count = 1;

    void Start()
    {
        obj = ObjectManager.GetInstance();
        place = obj._place;

        objects.Add(0,"BoomDrone");
        objects.Add(1,"CCTV");
        objects.Add(2,"BoostDrone");
        objects.Add(3,"LaserDrone");
        objects.Add(4,"MissileDrone");
        objects.Add(5,"RayDrone");
        objects.Add(6,"ShootingDrone");
        objects.Add(7,"Missile");
        objects.Add(8,"Marker");

        text.text = objects[pos];
    }

    void Update()
    {
        Progress();
    }

    public void Progress()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            pos = pos - 1 < 0 ? objects.Count - 1 : pos - 1;
            text.text = objects[pos];
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            pos = pos + 1 >= objects.Count ? 0 : pos + 1;
            text.text = objects[pos];
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
            Create(pos);
    }

    public void Create(int pos)
    {
        switch(pos)
        {
            case 0:
            EnemyCreator.BoomDrone(count,MouseMapPos());
            break;
            case 1:
            for(int i = 0; i < count; ++i)
                EnemyCreator.CCTV(0,MouseMapPos());
            break;
            case 2:
            EnemyCreator.BoostDrone(count,MouseMapPos());
            break;
            case 3:
            for(int i = 0; i < count; ++i)
                obj.AddObject<LaserDrone>(Define.ObjectType.enemy,"LaserDrone").SetPositionEm(MouseWorldPos());
            break;
            case 4:
            for(int i = 0; i < count; ++i)
                obj.AddObject<MissileDrone>(Define.ObjectType.enemy,"MissileDrone").SetPositionEm(MouseWorldPos());
            break;
            case 5:
            EnemyCreator.RayDrone(count,MouseWorldPos());
            break;
            case 6:
            EnemyCreator.ShootingDrone(count,MouseMapPos());
            break;
            case 7:
            for(int i = 0; i < count; ++i)
                obj.AddObject<NPC>(Define.ObjectType.enemy,"Missile").SetPositionEm(MouseWorldPos());
            break;
            case 8:
            obj.AddObject<TheMarker>(Define.ObjectType.enemy,"Marker").SetPositionEm(MouseWorldPos());
            break;
        }
    }

    public Vector2 MouseMapPos()
    {
        return place.WorldPosToMapPos(MouseWorldPos());
    }

    public Vector2 MouseWorldPos()
    {
        return CameraControll.instance.ScreenToWorldMouse();
    }
}
