using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCache {
	private Dictionary<string, Queue<ObjectBase>> _cacheDic = 
			new Dictionary<string, Queue<ObjectBase>>();

	private Dictionary<string, LinkedList<ObjectBase>> _runDic = 
			new Dictionary<string, LinkedList<ObjectBase>>();

	private Dictionary<string, GameObject> _originDic = 
			new Dictionary<string, GameObject>();

	public ObjectBase GetCacheObject(Define.ObjectType type, string name, bool createIfQueueIsEmpty = true)
	{
		if(_cacheDic.ContainsKey(name))
		{
			var queue = _cacheDic[name];
			if(queue.Count != 0)
			{
				var obj = queue.Dequeue();
				obj.SetActive(true);
				obj.Revive();
				obj.initialize();

				_runDic[name].Add(obj);

				return obj;
			}
			else
			{
				if(createIfQueueIsEmpty)
				{
					ObjectBase obj = ObjectManager.GetInstance().CreateObject(type,_originDic[name],true);
					obj.initialize();

					_runDic[name].Add(obj);

					return obj;
				}
			}
		}

		return null;
	}

	public void CreateObjects(Define.ObjectType type,GameObject origin,string name, int count)
	{
		if(_cacheDic.ContainsKey(name))
		{
			var queue = _cacheDic[name];
			for(int i = 0; i < count; ++i)
			{
				queue.Enqueue(ObjectManager.GetInstance().CreateObject(type,origin));
			}
		}
		else
		{
			Queue<ObjectBase> queue = new Queue<ObjectBase>();
			for(int i = 0; i < count; ++i)
			{
				queue.Enqueue(ObjectManager.GetInstance().CreateObject(type,origin));
			}

			_cacheDic.Add(name,queue);
			_runDic.Add(name,new LinkedList<ObjectBase>());
			_originDic.Add(name,origin);
		}
	}

	public void UpdateCache()
	{
		foreach(var item in _runDic)
		{
			var link = item.Value.front;
			var queue = _cacheDic[item.Key];

			while(link != null)
			{
				if(link.target.deleted)
				{
					item.Value.DisconnectLink(link);
					queue.Enqueue(link.target);
				}

				link = link.next;
			}
		}
	}
}
