using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationKeyEvent
{
    public enum EventType
    {
        Translate,
        SetActive
    };

    public class KeyEventBase
    {
        public EventType type;
        public Transform target;
        public Vector3 item;

        public KeyEventBase(EventType t, Transform ta, Vector3 v)
        {
            type = t;
            target = ta;
            item = v;
        }
    }

    private Dictionary<string, Dictionary<int ,List<KeyEventBase>>> _events = 
            new Dictionary<string, Dictionary<int ,List<KeyEventBase>>>();

    public void AddEvent(string key, int frame, EventType type, Transform target, Vector3 value)
    {
        if(!_events.ContainsKey(key))
        {
            _events.Add(key, new Dictionary<int ,List<KeyEventBase>>());
        }

        if(!_events[key].ContainsKey(frame))
        {
            _events[key][frame] = new List<KeyEventBase>();
        }

        _events[key][frame].Add(new KeyEventBase(type,target,value));
    }

    public void EventEntry(string key, int frame)
    {
        if(_events.ContainsKey(key))
        {
            if(_events[key].ContainsKey(frame))
            {
                EventProgress(_events[key][frame]);
            }
        }
    }

    public void EventProgress(List<KeyEventBase> list)
    {
        foreach(var e in list)
        {
            if(e.type == EventType.Translate)
            {
                e.target.localPosition = e.item;
            }
            else if(e.type == EventType.SetActive)
            {
                e.target.gameObject.SetActive(e.item.x == 0);
            }
        }
        
    }

    public void AddTranslateEvent(string key, int frame, Transform target, Vector2 value)
    {
        AddEvent(key,frame,EventType.Translate,target,value);
    }

    public void AddActiveEvent(string key, int frame, Transform target, bool value)
    {
        AddEvent(key,frame,EventType.SetActive,target,value ? Vector3.zero : Vector3.left);
    }
}  
