using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMono<StageManager>, Define.IManager
{

    public enum EventType
    {
        PlayerSpawn = 0,
        EnemySpawn,
        EnemySpawnTimer,
        SetTimer,
        ChangeTimer,
        Loop,
        Jump,
        Dialog,
        SkippedDialog,
        WaitForAnnihilation,
        WaitForSeconds,
        ShowWaveIcon,
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

    public class Event_EnemySpawnTimer : EventBase
    {
        private int pos;
        private int count;
        float timer = 0f;
        float time = 0f;
        public override bool Progress(float delatTime)
        {
            timer += delatTime;
            if(timer >= time)
            {
                switch(pos)
                {
                    case 0:
                    EnemyCreator.BoomDrone(1,GetSpawnPos(3f));
                    break;
                    case 1:
                    EnemyCreator.CCTV(0,GetSpawnPos(3f));
                    break;
                    case 2:
                    EnemyCreator.BoostDrone(1,GetSpawnPos(3f));
                    break;
                    case 3:
                    EnemyCreator.LaserDrone(1,GetSpawnPos(3f));
                    break;
                    case 4:
                    EnemyCreator.MissileDrone(1,GetSpawnPos(2f));
                    break;
                    case 5:
                    EnemyCreator.RayDrone(1,GetSpawnPos(3f));
                    break;
                    case 6:
                    EnemyCreator.ShootingDrone(1,GetSpawnPos(3f));
                    break;
                    case 7:
                    EnemyCreator.Missile(1,GetSpawnPos(3f));
                    break;
                    case 8:
                        //obj.AddObject<TheMarker>(Define.ObjectType.enemy,"Marker").SetPositionEm(MouseWorldPos());
                    break;
                }

                timer = 0f;
                --count;
            }
            

            return count <= 0;
        }

        public Vector3 GetSpawnPos(float yDist)
        {
            var vec = new Vector3(0f,Random.Range(-4f,4f),0f);
            vec.x = (Random.Range(0,2) == 0 ? 1f : -1f) * Random.Range(5f,9f);
            vec = Player.instance.position + vec;

            vec.y = vec.y <= 1f ? Random.Range(2f, 4f) : vec.y;

            return vec;
        }

        public override void DataSet()
        {
            var s = data.Split(',');
            pos = int.Parse(s[0]);
            count = int.Parse(s[1]);
            time = float.Parse(s[2]);
            timer = 0f;
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
            vec.x = (Random.Range(0,2) == 0 ? 1f : -1f) * Random.Range(5f,9f);
            vec = Player.instance.position + vec;

            vec.y = vec.y <= 1f ? Random.Range(2f, 4f) : vec.y;

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

    public class Event_Jump : EventBase
    {
        int point;
        public override bool Progress(float delatTime)
        {
            StageManager.instance.SetEventPos(point);
            return true;

        }

        public override void DataSet()
        {
            point = int.Parse(data);
        }
    }

    public class Event_Dialog : EventBase
    {
        string[] datas;
        public override bool Progress(float delatTime)
        {
            DialogManager.instance.ShowDialog(datas[0],datas[1] == "0",datas[2] == "0");
            return true;
        }

        public override void DataSet()
        {
            datas = data.Split(',');
        }
    }

    public class Event_WaitForAnnihilation : EventBase
    {
        int enemyCount;
        float timer;

        bool trigger = false;
        public override bool Progress(float delatTime)
        {
            if(!trigger)
            {
                if(ObjectManager.GetInstance().GetEnemyCount() <= enemyCount)
                    trigger = true;
            }
            else
            {
                timer -= delatTime;
                if(timer <= 0f)
                    return true;
            }
            

            return false;
        }

        public override void DataSet()
        {
            var s = data.Split(',');
            enemyCount = int.Parse(s[0]);
            timer = float.Parse(s[1]);
        }
    }

    public class Event_SkippedDialog : EventBase
    {
        string[] datas;
        public override bool Progress(float delatTime)
        {
            if(DialogManager.instance.skipped)
                DialogManager.instance.ShowDialog(datas[0],datas[1] == "0",datas[2] == "0");
            return true;
        }

        public override void DataSet()
        {
            datas = data.Split(',');
        }
    }

    public class Event_ShowWaveIcon : EventBase
    {
        float time;
        public override bool Progress(float delatTime)
        {
            MainHud.instance.ShowWaveIcon(time);
            return true;
        }

        public override void DataSet()
        {
            time = float.Parse(data);
        }
    }

    public List<EventBase> events = new List<EventBase>();
    public int eventPos = 0;
    public int eventEndPos;

    public TextAsset mapData;

    private bool _eventEnd;

    public void firstSetting()
    {
        SetSingleton(this);

        ParseMapData();
    }

    public void ParseMapData()
    {
        var data = mapData.text.Replace("\r",string.Empty).Split('\n');
        foreach(var low in data)
        {
            var split = low.Split('/');
            events.Add(ParseEventData(split[0],split[1]));
        }


        eventEndPos = events.Count;
        _eventEnd = events.Count == 0;
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

    public EventBase ParseEventData(string title, string data)
    {
        var head = (EventType)(int.Parse(title));
        EventBase e = null;

        if(head == EventType.ChangeTimer)
        {
            
        }
        else if(head == EventType.Dialog)
        {
            e = new Event_Dialog();
        }
        else if(head == EventType.EnemySpawn)
        {
            e = new Event_EnemySpawn();
        }
        else if(head == EventType.EnemySpawnTimer)
        {
            e = new Event_EnemySpawnTimer();
        }
        else if(head == EventType.Jump)
        {
            e = new Event_Jump();
        }
        else if(head == EventType.Loop)
        {
            e = new Event_Loop();
        }
        else if(head == EventType.PlayerSpawn)
        {
            e = new Event_PlayerSpawn();
        }
        else if(head == EventType.SetTimer)
        {
            
        }
        else if(head == EventType.ShowWaveIcon)
        {
            e = new Event_ShowWaveIcon();
        }
        else if(head == EventType.SkippedDialog)
        {
            e = new Event_SkippedDialog();
        }
        else if(head == EventType.StageClear)
        {
            
        }
        else if(head == EventType.WaitForAnnihilation)
        {
            e = new Event_WaitForAnnihilation();
        }
        else if(head == EventType.WaitForSeconds)
        {
            e = new Event_WaitForSeconds();
        }

        if(e == null)
        {
            Debug.Log("Mistake");
            return null;
        }

        e.Set(head,data);
        e.DataSet();

        return e;
    }

    public void SetEventPos(int pos)
    {
        eventPos = pos;
    }
}
