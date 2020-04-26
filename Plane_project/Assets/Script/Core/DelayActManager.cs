using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayActManager : Singleton<DelayActManager> , Define.IManager
{
    class ActionItem
    {
        public Action action;
        public float timer;

        public bool progress(float deltaTime) 
        {
            timer -= deltaTime;
            if(timer <= 0f)
            {
                timer = 0f;
                return true;
            }

            return false;
        }

        public void Set(Action act, float t) {action = act; timer = t;}
        public ActionItem(Action act, float t) {Set(act,t);}
    }

    Queue<ActionItem> _cahce = new Queue<ActionItem>();
    List<ActionItem> _list = new List<ActionItem>();

    public void firstSetting()
    {
        for(int i = 0; i < 10; ++i)
            _cahce.Enqueue(new ActionItem(null,0f));
    }

    public void progress(float deltaTime)
    {
        for(int i = 0; i < _list.Count;)
        {
            if(_list[i].progress(deltaTime))
            {
                _list[i].action();
                _cahce.Enqueue(_list[i]);
                _list.RemoveAt(i);
            }
            else
                ++i;
        }
    }

    public void lateProgress(float deltaTime)
    {

    }

    public void RequestAction(Action act, float timer)
    {
        var item = GetCachedItem();
        item.Set(act,timer);
        _list.Add(item);
    }

    private ActionItem GetCachedItem()
    {
        if(_cahce.Count == 0)
            return new ActionItem(null,0f);
        else
            return _cahce.Dequeue();
    }
}
