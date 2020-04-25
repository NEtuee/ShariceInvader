using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : Singleton<CollisionManager>, Define.IManager
{
    private Dictionary<int, List<Collisionable>> _collisionList = 
                                                new Dictionary<int, List<Collisionable>>();
    private bool[,] _canCollision = new bool[(int)Define.ObjectType.AutoProgressEnd,
                                            (int)Define.ObjectType.AutoProgressEnd];

    public void firstSetting()
    {
        InitCollisionMap(false);

        CanCollisionSet(Define.ObjectType.player,Define.ObjectType.enemy,true);
        CanCollisionSet(Define.ObjectType.player,Define.ObjectType.objects,true);
        CanCollisionSet(Define.ObjectType.enemy,Define.ObjectType.objects,true);
    }

    public void progress(float deltaTime)
    {
        
    }

    public void lateProgress(float deltaTime)
    {

    }

    public void RegisteCollisionList(Collisionable coll)
    {
        int i = (int)coll.type;
        if(!_collisionList.ContainsKey(i))
        {
            _collisionList.Add(i,new List<Collisionable>());
        }

        _collisionList[i].Add(coll);
    }

    public void UpdateCollisionList()
    {
        foreach(var one in _collisionList)
        {
            foreach(var two in _collisionList)
            {
                if(one.Key > two.Key)
                    continue;
                
                if(!CanCollision(one.Key,two.Key))
                {
                    continue;
                }

                var oneList = one.Value;
                var twoList = two.Value;

                int oneCount = oneList.Count;
                int twoCount = twoList.Count;

                for(int i = 0; i < oneCount; ++i)
                {
                    int j = one.Key == two.Key ? i : 0;
                    for(; j < twoCount; ++j)
                    {
                        var oneObj = oneList[i];
                        var twoObj = twoList[j];

                        if((oneObj != null && twoObj != null) && (!oneObj.deleted && !twoObj.deleted))
                        {
                            oneObj.UpdateCollider();
                            twoObj.UpdateCollider();

                            if(oneObj.CollisionCheck(twoObj))
                            {
                                oneObj.CollisionProgress(twoObj.type,twoObj);
                                twoObj.CollisionProgress(oneObj.type,oneObj);
                            }
                        }
                    }
                }
            }
        }
    }

    public void SyncCollisionList()
    {
        foreach(var value in _collisionList)
        {
            int count = value.Value.Count;
            var list = value.Value;

            for(int i = 0; i < count;)
            {
                if(list[i] == null || list[i].deleted)
                {
                    list.RemoveAt(i);
                    count--;
                }
                else
                    ++i;
            }
        }
    }

    public List<Collisionable> GetCollisionList(Define.ObjectType type)
    {
        int t = (int)type;
        if(_collisionList.ContainsKey(t))
        {
            return _collisionList[t];
        }
        else
            return null;
    }

    public void CanCollisionSet(Define.ObjectType one, Define.ObjectType two, bool value)
    {
        _canCollision[(int)one,(int)two] = value;
        _canCollision[(int)two,(int)one] = value;
    }

    public bool CanCollision(Define.ObjectType one, Define.ObjectType two)
    {
        return _canCollision[(int)one,(int)two];
    }

    public bool CanCollision(int one, int two)
    {
        return _canCollision[one,two];
    }

    public void InitCollisionMap(bool value)
    {
        int c = (int)Define.ObjectType.AutoProgressEnd;

        for(int i = 0; i < c; ++i)
        {
            for(int j = 0; j < c; ++j)
            {
                _canCollision[i,j] = false;
            }
        }

    }
    
}
