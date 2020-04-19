using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor_PlaneInfoBase
{
    public PlaneBase.AnimationType animationType = PlaneBase.AnimationType.None;
    public Define.ObjectType objectType = Define.ObjectType.enemy;
    
    public struct TrailInfo
    {
        public string trailMaterial;
        public float time;
        public float startWidth;
        public float endWidth;

        public TrailInfo(string mat, float t, float s, float e)
        {
            trailMaterial = mat;
            time = t;
            startWidth = s;
            endWidth = e;
        }
    };

    public string   planeName = "None";
    public string   spriteSet = "";

    public string   boostAni = "Effects/Boost";
    public float    mass = 1f;
    public float    frictionFactor = 0.01f;
    public float    gravityScale = 0.7f;
    public float    maxSpeed = 3f;
    public float    speed = 0.1f;
    public float    dodgeDist = 1f;


    public bool     rotateLock = false;
    public bool     velocityFlip = true;
    public bool     directionAngle = false;
    public bool     trailEmmit = true;
    public bool     boostAniProgress = true;

    public int      hp = 5;
    public int      bodyAttack = 1;

    public int      boostCount = 1;
    public int      trailCount = 1;

    public TrailInfo trailInfo;

    public Dictionary<int,List<Vector2>> trailPoint = new Dictionary<int,List<Vector2>>();
    public Dictionary<int,List<int>> trailSortingOredrs = new Dictionary<int, List<int>>();
    public Dictionary<int,List<Vector2>> boostPoint = new Dictionary<int,List<Vector2>>();

    private string _path;

    public void SetPath(string p) {_path = p;}

    public void ClearDictionary()
    {
        foreach(var item in trailPoint)
        {
            item.Value.Clear();
        }
        trailPoint.Clear();

        foreach(var item in trailSortingOredrs)
        {
            item.Value.Clear();
        }
        trailSortingOredrs.Clear();

        foreach(var item in boostPoint)
        {
            item.Value.Clear();
        }
        boostPoint.Clear();
        
    }

    public void CreateDataFile(string spr)
    {
        Debug.Log(spr.Replace("\\",string.Empty));
        Sprite[] set = ResourceManager.GetInstance().GetSpriteSet(spr,2);

        planeName = "None";
        spriteSet = "";
        boostAni = "Effects/Boost";
        mass = 1f;
        frictionFactor = 0.01f;
        gravityScale = 0.7f;
        maxSpeed = 3f;
        speed = 0.1f;
        dodgeDist = 1f;
        rotateLock = false;
        velocityFlip = true;
        directionAngle = false;
        trailEmmit = true;
        boostAniProgress = true;
        hp = 5;
        bodyAttack = 1;
        boostCount = 1;
        trailCount = 1;

        trailInfo = new TrailInfo("PlaneTrail",0.5f,0.03f,0.005f);

        ClearDictionary();

        for(int i = 0; i < set.Length; ++i)
        {
            List<Vector2> trailList = new List<Vector2>();
            List<Vector2> boostList = new List<Vector2>();
            List<int> sortList = new List<int>();

            trailList.Add(new Vector2(0f,0f));
            boostList.Add(new Vector2(0f,0f));
            sortList.Add(-1);

            trailPoint.Add(i,trailList);
            boostPoint.Add(i,boostList);
            trailSortingOredrs.Add(i,sortList);
        }

        spriteSet = spr;

        SaveData();
    }

    public void SaveData()
    {
        List<string> data = new List<string>();
        data.Add("animationType:" + animationType.ToString());
        data.Add("objectType:" + objectType.ToString());
        data.Add("planeName:" + planeName);
        data.Add("spriteSet:" + spriteSet);
        data.Add("boostAni:" + boostAni);
        data.Add("mass:" + mass.ToString());
        data.Add("frictionFactor:" + frictionFactor.ToString());
        data.Add("gravityScale:" + gravityScale.ToString());
        data.Add("maxSpeed:" + maxSpeed.ToString());
        data.Add("speed:" + speed.ToString());
        data.Add("dodgeDist:" + dodgeDist.ToString());
        data.Add("rotateLock:" + rotateLock.ToString());
        data.Add("velocityFlip:" + velocityFlip.ToString());
        data.Add("directionAngle:" + directionAngle.ToString());
        data.Add("trailEmmit:" + trailEmmit.ToString());
        data.Add("boostAniProgress:" + boostAniProgress.ToString());
        data.Add("hp:" + hp.ToString());
        data.Add("bodyAttack:" + bodyAttack.ToString());
        data.Add("boostCount:" + boostCount.ToString());
        data.Add("trailCount:" + trailCount.ToString());
        data.Add("trailInfo:" + trailInfo.trailMaterial + "," + trailInfo.time + "," + trailInfo.startWidth + "," + trailInfo.endWidth);

        if(trailPoint.Count > 0)
        {
            string s = "trailPoints:";
            foreach(var item in trailPoint)
            {
                s += item.Key.ToString() + "_";
                foreach(var vector in item.Value)
                {
                    s += vector.x + "," + vector.y + "!";
                }
                s += "/";
            }
            data.Add(s);

            s = "trailSortingOrder:";
            foreach(var item in trailSortingOredrs)
            {
                s += item.Key.ToString() + "_";
                foreach(var i in item.Value)
                {
                    s += i + "!";
                }
                s += "/";
            }
            data.Add(s);
        }

        if(boostPoint.Count > 0)
        {
            string s = "boostPoints:";
            foreach(var item in boostPoint)
            {
                s += item.Key.ToString() + "_";
                foreach(var vector in item.Value)
                {
                    s += vector.x + "," + vector.y + "!";
                }
                s += "/";
            }
            data.Add(s);
        }

        IOManager.WriteStringToFile_NoMark(data.ToArray(),_path,false);
    }

    public void ParseDictionaryData(ref Dictionary<int,List<Vector2>> dic, string data)
    {
        data = data.Replace("\r",string.Empty);
        string[] splitData = data.Split('/');
        foreach(var d in splitData)
        {
            if(d == "")
                continue;

            string[] row = d.Split('_');
            string[] vectors = row[1].Split('!');

            List<Vector2> vectorList = new List<Vector2>();
            foreach(var vector in vectors)
            {
                if(vector == "")
                    continue;

                string pureData = vector.Replace(" ",string.Empty);
                pureData = pureData.Replace("(",string.Empty);
                pureData = pureData.Replace(")",string.Empty);

                string[] split = pureData.Split(',');
                vectorList.Add(new Vector2(float.Parse(split[0]),float.Parse(split[1])));
            }

            dic.Add(int.Parse(row[0]),vectorList);
        }
    }

    public void ParseDictionaryDataForSortingOrder(ref Dictionary<int,List<int>> dic, string data)
    {
        data = data.Replace("\r",string.Empty);
        string[] splitData = data.Split('/');
        foreach(var d in splitData)
        {
            if(d == "")
                continue;

            string[] row = d.Split('_');
            string[] ints = row[1].Split('!');

            List<int> intList = new List<int>();
            foreach(var i in ints)
            {
                if(i == "")
                    continue;

                string pureData = i.Replace(" ",string.Empty);

                intList.Add(int.Parse(pureData));
            }
            dic.Add(int.Parse(row[0]),intList);
        }
    }

    public void LoadDataFile(string[] data)
    {
        ClearDictionary();

        foreach(var d in data)
        {
            string[] split = d.Split(':');

            if(split[0] == "animationType")
                animationType = (PlaneBase.AnimationType)Enum.Parse(typeof(PlaneBase.AnimationType),split[1]);
            else if(split[0] == "objectType")
                objectType = (Define.ObjectType)Enum.Parse(typeof(Define.ObjectType),split[1]);
            else if(split[0] == "planeName")
                planeName = split[1];
            else if(split[0] == "spriteSet")
                spriteSet = split[1];
            else if(split[0] == "boostAni")
                boostAni = split[1];
            else if(split[0] == "mass")
                mass = float.Parse(split[1]);
            else if(split[0] == "frictionFactor")
                frictionFactor = float.Parse(split[1]);
            else if(split[0] == "gravityScale")
                gravityScale = float.Parse(split[1]);
            else if(split[0] == "maxSpeed")
                maxSpeed = float.Parse(split[1]);
            else if(split[0] == "speed")
                speed = float.Parse(split[1]);
            else if(split[0] == "dodgeDist")
                 dodgeDist = float.Parse(split[1]);
            else if(split[0] == "rotateLock")
                rotateLock = bool.Parse(split[1]);
            else if(split[0] == "velocityFlip")
                velocityFlip = bool.Parse(split[1]);
            else if(split[0] == "directionAngle")
                directionAngle = bool.Parse(split[1]);
            else if(split[0] == "trailEmmit")
                trailEmmit = bool.Parse(split[1]);
            else if(split[0] == "boostAniProgress")
                boostAniProgress = bool.Parse(split[1]);
            else if(split[0] == "hp")
                hp = int.Parse(split[1]);
            else if(split[0] == "bodyAttack")
                bodyAttack = int.Parse(split[1]);
            else if(split[0] == "boostCount")
                boostCount = int.Parse(split[1]);
            else if(split[0] == "trailCount")
                trailCount = int.Parse(split[1]);
            else if(split[0] == "trailInfo")
            {
                string[] row = split[1].Split(',');
                trailInfo = new TrailInfo(row[0],float.Parse(row[1]),float.Parse(row[2]),float.Parse(row[3]));
            }
            else if(split[0] == "trailPoints")
            {
                ParseDictionaryData(ref trailPoint,split[1]);
            }
            else if(split[0] == "boostPoints")
            {
                ParseDictionaryData(ref boostPoint,split[1]);
            }
            else if(split[0] == "trailSortingOrder")
            {
                ParseDictionaryDataForSortingOrder(ref trailSortingOredrs,split[1]);
            }
        }
    }

    public void LoadDataFile()
    {
        string[] data = IOManager.ReadStringFromFile(_path);

        LoadDataFile(data);
    }
}
