using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMono<StageManager>, Define.IManager
{

    public enum EventType
    {
        PlayerSpawn,
        EnemySpawn,
        SetTimer,
        ChangeTimer,
        Loop,
        Dialog,
        WaitForAnnihilation,
        WaitForSeconds,
        StageClear,
    }

    [System.Serializable]
    public class EventBase
    {
        public EventType type;
        public string data;
        public virtual bool Progress(float delatTime){return true;}
        public virtual void DataSet(){}

        public void Set(EventType t, string d) {type = t; data = d;}
    }

    public class Event_PlayerSpawn : EventBase
    {
        Vector3 pos;
        public override bool Progress(float delatTime)
        {
            PlaneBase obj = ObjectManager.GetInstance().AddObject<Player>(Define.ObjectType.player,"Player");//_objManager.AddObject(Define.ObjectType.one,player);
		    obj.SetPositionEm(pos);
		    CameraControll.instance.SetTarget(obj);

		    ObjectManager.GetInstance()._place.SetMainObject(obj);

		    MainHud.instance.Initiailize();

            return true;
        }

        public override void DataSet()
        {
            var s = data.Split(',');
            pos = new Vector3(float.Parse(s[0]),float.Parse(s[1]));
        }
    }

    public class Event_EnemySpawn : EventBase
    {
        private int pos;
        private int count;
        public override bool Progress(float delatTime)
        {
            switch(pos)
            {
                case 0:
                EnemyCreator.BoomDrone(count,GetSpawnPos(3f));
                break;
                case 1:
                for(int i = 0; i < count; ++i)
                    EnemyCreator.CCTV(0,GetSpawnPos(3f));
                break;
                case 2:
                EnemyCreator.BoostDrone(count,GetSpawnPos(3f));
                break;
                case 3:
                EnemyCreator.LaserDrone(count,GetSpawnPos(3f));
                break;
                case 4:
                EnemyCreator.MissileDrone(count,GetSpawnPos(2f));
                break;
                case 5:
                EnemyCreator.RayDrone(count,GetSpawnPos(3f));
                break;
                case 6:
                EnemyCreator.ShootingDrone(count,GetSpawnPos(3f));
                break;
                case 7:
                EnemyCreator.Missile(count,GetSpawnPos(3f));
                break;
                case 8:
                    //obj.AddObject<TheMarker>(Define.ObjectType.enemy,"Marker").SetPositionEm(MouseWorldPos());
                break;
            }

            return true;
        }

        public Vector3 GetSpawnPos(float yDist)
        {
            var vec = new Vector3(0f,Random.Range(-4f,4f),0f);
            vec.x = (Random.Range(0,2) == 0 ? 1f : -1f) * Random.Range(3.3f,6f);

            return vec;
        }

        public override void DataSet()
        {
            var s = data.Split(',');
            pos = int.Parse(s[0]);
            count = int.Parse(s[1]);
        }

    }

    public class Event_WaitForSeconds : EventBase
    {
        private float timer = 0f;
        private float origin = 0f;
        public override bool Progress(float delatTime)
        {
            timer -= delatTime;
            if(timer <= 0f)
            {
                timer = origin;
                return true;
            }
            
            return false;
        }

        public override void DataSet()
        {
            origin = timer = float.Parse(data);
        }
    }

    public class Event_Loop : EventBase
    {
        int loopCount;
        int point;
        public override bool Progress(float delatTime)
        {
            if(--loopCount >= 0)
            {
                StageManager.instance.SetEventPos(point);
                return false;
            }
            else
                return true;
        }

        public override void DataSet()
        {
            var s = data.Split(',');
            loopCount = int.Parse(s[0]);
            point = int.Parse(s[1]);
        }
    }

    public class Event_Dialog : EventBase
    {
        public override bool Progress(float delatTime)
        {
            DialogManager.instance.ShowDialog(data);
            return true;
        }

        public override void DataSet()
        {
            
        }
    }

    public EventBase[] events;
    public int eventPos = 0;
    public int eventEndPos;

    private bool _eventEnd;

    public void firstSetting()
    {
        SetSingleton(this);

        _eventEnd = events.Length == 0;
    
        for(int i = 0; i < events.Length; ++i)
        {
            string s = events[i].data;

            switch(events[i].type)
            {
                case EventType.PlayerSpawn:
                events[i] = new Event_PlayerSpawn();
                events[i].Set(EventType.PlayerSpawn,s);
                break;
                case EventType.ChangeTimer:
                break;
                case EventType.Dialog:
                events[i] = new Event_Dialog();
                events[i].Set(EventType.Dialog,s);
                break;
                case EventType.EnemySpawn:
                events[i] = new Event_EnemySpawn();
                events[i].Set(EventType.EnemySpawn,s);
                break;
                case EventType.Loop:
                events[i] = new Event_Loop();
                events[i].Set(EventType.Loop,s);
                break;
                case EventType.SetTimer:
                break;
                case EventType.StageClear:
                break;
                case EventType.WaitForAnnihilation:
                break;
                case EventType.WaitForSeconds:
                events[i] = new Event_WaitForSeconds();
                events[i].Set(EventType.WaitForSeconds,s);
                break;
            }
            events[i].DataSet();
        }

        eventEndPos = events.Length;
    }

    public void progress(float deltaTime)
    {
        if(!_eventEnd)
        {
            if(events[eventPos].Progress(deltaTime))
            {
                if(++eventPos >= eventEndPos)
                {
                    _eventEnd = true;
                }
            }
        }
    }

    public void lateProgress(float deltaTime)
    {

    }

    public void SetEventPos(int pos)
    {
        eventPos = pos;
    }
}
