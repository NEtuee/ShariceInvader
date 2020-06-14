using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>, Define.IManager {
	public delegate void DelayObjectCreateEventDelegate(ObjectBase obj);

	public class DelayObjectCreateItem
	{
		public string name;
		public float timer;
		public GameObject target;
		public Define.ObjectType type;
		public DelayObjectCreateEventDelegate eventDelegate;

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

		public void Set(string n, float t, DelayObjectCreateEventDelegate d,GameObject tar, Define.ObjectType ty)
		{
			name = n;
			timer = t;
			eventDelegate = d;
			target = tar;
			type = ty;
		}
	}

	private Dictionary<int, LinkedList<ObjectBase>> _objectDic =
			new Dictionary<int, LinkedList<ObjectBase>>();

	private ObjectCache _cache;
	private Dictionary<string, ObjectCache> _entityCache = new Dictionary<string, ObjectCache>();
	public PlaceMapper _place;

	private float _stopTimer = 0f;

	private List<ObjectBase> _created = new List<ObjectBase>();


	private List<DelayObjectCreateItem> _createRequests = new List<DelayObjectCreateItem>();
	private Queue<DelayObjectCreateItem> _createPool = new Queue<DelayObjectCreateItem>();
	
	private Define.VoidObjectDelegate placeUpdate;

	public void firstSetting()
	{
		_cache = new ObjectCache();
		_place = new PlaceMapper();

		_place.InitPlace(50,20,6);

		int end = (int)Define.ObjectType.AutoProgressEnd;
		for(int i = 0; i < end; ++i)
		{
			LinkedList<ObjectBase> link = new LinkedList<ObjectBase>();
			link.SetDeleteCondition((obj)=>{return obj.deleted;});
			link.SetDeleteAction((obj)=>
			{
				if(obj.cacheObject)
					obj.SetActive(false);
				else
					GameObject.Destroy(obj.obj);
			});

			_objectDic.Add(i, link);
		}

		placeUpdate = new Define.VoidObjectDelegate(PlaceUpdateLoop);
	}

	public LinkBase<ObjectBase> GetFirstLink(Define.ObjectType type) {return _objectDic[(int)type].front;}

	public void progress(float deltaTime)
	{
		if(_created.Count != 0)
		{
			for(int i = 0; i < _created.Count; ++i)
			{
				_created[i].BeforeCreated();
			}

			_created.Clear();
		}

		if(_stopTimer != 0f)
		{
			_stopTimer -= deltaTime;
			if(_stopTimer <= 0f)
				_stopTimer = 0f;

			return;
		}

		foreach(var dic in _objectDic)
		{
			if(dic.Value.count > 0)
			{
				dic.Value.Progress(deltaTime);
			}
		}



		foreach(var dic in _objectDic)
		{
			if(dic.Value.count > 0)
			{
				dic.Value.AfterProgress(deltaTime);
			}
		}

		// _physcis.BasicUpdate();
		// _physcis.Process(deltaTime);
		//=====

		_place.UpdatePlaceOrder();

		foreach(var dic in _objectDic)
		{
			if(dic.Value.count > 0)
			{
				dic.Value.Loop(placeUpdate);
			}
		}

		DelayedRequestsProgress(deltaTime);

		_cache.UpdateCache();
		foreach(var item in _entityCache.Values)
		{
			item.UpdateCache();
		}
	}
	
	public void PlaceUpdateLoop(ObjectBase obj)
	{
		if(obj.place != _place.GetPlace(obj))
		{
			if(obj.place != null && !obj.deleted)
				obj.place.ExitPlace(obj);
			obj.place = _place.GetPlace(obj);
			if(obj.place != null)
				obj.place.EnterPlace(obj);
		}

		if(obj.position.x > _place._right.leftBottom.x + _place._placeWidth)
		{
			Vector2 pos = obj.position;
			pos.x = pos.x - _place._mapWidth;
			obj.SetPosition(pos);
			obj.beforeUpdateTransform();
		}
		else if(obj.position.x < _place._left.leftBottom.x)
		{
			Vector2 pos = obj.position;
			pos.x = pos.x + _place._mapWidth;
			obj.SetPosition(pos);
			obj.beforeUpdateTransform();
		}
	}

	public void lateProgress(float deltaTime)
	{

	}

	public void UpdateTransform()
	{
		foreach(var dic in _objectDic)
		{
			if(dic.Value.count > 0)
			{
				dic.Value.UpdateTransform();
			}
		}
	}

	public void DeleteProgress()
	{
		foreach(var dic in _objectDic)
		{
			if(dic.Value.count > 0)
			{
				dic.Value.DeleteProgress();
			}
		}
	}

	public void DelayedRequestsProgress(float deltaTime)
	{
		for(int i = 0; i < _createRequests.Count;)
		{
			if(_createRequests[i].progress(deltaTime))
			{
				var obj = AddObject(_createRequests[i].type,_createRequests[i].target);
				obj.name = _createRequests[i].name;
				_createRequests[i].eventDelegate(obj);

				_createPool.Enqueue(_createRequests[i]);
				_createRequests.RemoveAt(i);
			}
			else
			{
				++i;
			}
		}
	}

	public void AddObjectDelayed(float time, string name, GameObject origin, Define.ObjectType type, DelayObjectCreateEventDelegate del)
	{
		DelayObjectCreateItem item = null;

		if(_createPool.Count != 0)
		{
			item = _createPool.Dequeue();
		}
		else
		{
			item = new DelayObjectCreateItem();
		}

		item.Set(name,time,del,origin,type);

		_createRequests.Add(item);
	}

	public void UpdateStop(float time)
	{
		_stopTimer = time;
	}

	public ObjectBase FindObject(Define.ObjectType type, string name)
	{
		int t = (int)type;
		if(_objectDic.ContainsKey(t))
		{
			var item = _objectDic[t].Find(name);
			if(item != null)
				return item.target;
		}

		return null;
	}

	public ObjectBase AddObject(Define.ObjectType type, string name)
	{
		ObjectBase obj = _cache.GetCacheObject(type,name);
		_objectDic[(int)type].Add(obj);
		
		_created.Add(obj);

		return obj;
		
	}

	public ObjectBase AddObject(Define.ObjectType type, GameObject origin)
	{
		ObjectBase obj = CreateObject(type,origin,true,false);
		_objectDic[(int)type].Add(obj);

		_created.Add(obj);

		return obj;
	}

	public T AddObject<T> (Define.ObjectType type, string name) where T : ObjectBase
	{
		T obj = CreateObject<T>(type,name,true,false);
		_objectDic[(int)type].Add(obj);

		_created.Add(obj);

		return obj;
	}

	public void CreateCacheObjects(Define.ObjectType type, GameObject origin, string name, int count)
	{
		_cache.CreateObjects(type,origin,name,count);
	}

	public T CreateObject<T>(Define.ObjectType type, string name, bool active = false, bool cache = true) where T : ObjectBase
	{
		GameObject obj = new GameObject(name);
		T target = obj.AddComponent<T>();
		
		target.cacheObject = cache;
		target.type = type;

		target.SetNecessary();
		target.firstSetting();
		if(!cache)
			target.initialize();
		target.SetActive(active);

		return target;
	}

	public ObjectBase CreateObject(Define.ObjectType type, GameObject origin, bool active = false, bool cache = true)
	{
		ObjectBase obj = GameObject.Instantiate(origin).GetComponent<ObjectBase>();

		obj.cacheObject = cache;
		obj.type = type;

		obj.SetNecessary();
		obj.firstSetting();
		if(!cache)
			obj.initialize();
		obj.SetActive(active);

		return obj;
	}

	public ObjectCache GetCache() {return _cache;}
}
