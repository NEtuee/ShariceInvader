using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>, Define.IManager {

	private Dictionary<Define.ObjectType, LinkedList<ObjectBase>> _objectDic =
			new Dictionary<Define.ObjectType, LinkedList<ObjectBase>>();

	private ObjectCache _cache;
	public PlaceMapper _place;

	private float _stopTimer = 0f;

	public void firstSetting()
	{
		_cache = new ObjectCache();
		_place = new PlaceMapper();

		_place.InitPlace(30,10,5);

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

			_objectDic.Add((Define.ObjectType)i, link);
		}
	}

	public LinkBase<ObjectBase> GetFirstLink(Define.ObjectType type) {return _objectDic[type].front;}
	// public LinkBase<ObjectBase> GetPlaceObjectLink(ObjectBase obj, Define.ObjectType type)
	// {
	// 	if(obj.place == null)
	// 		return null;
		
	// 	return obj.place.GetLinkToType(obj.type);
	// }
	public void progress(float deltaTime)
	{
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
				dic.Value.Loop((ObjectBase obj)=>{
					if(obj.place != _place.GetPlace(obj))
					{
						if(obj.place != null)
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
					
				});
			}
		}

		foreach(var dic in _objectDic)
		{
			if(dic.Value.count > 0)
			{
				dic.Value.UpdateTransform();
			}
		}


//충돌

//====
		_cache.UpdateCache();
	}
	
	public void lateProgress(float deltaTime)
	{

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

	public void UpdateStop(float time)
	{
		_stopTimer = time;
	}

	public ObjectBase FindObject(Define.ObjectType type, string name)
	{
		if(_objectDic.ContainsKey(type))
		{
			var item = _objectDic[type].Find(name);
			if(item != null)
				return item.target;
		}

		return null;
	}

	public ObjectBase AddObject(Define.ObjectType type, string name)
	{
		ObjectBase obj = _cache.GetCacheObject(type,name);
		_objectDic[type].Add(obj);
		
		return obj;
		
	}

	public ObjectBase AddObject(Define.ObjectType type, GameObject origin)
	{
		ObjectBase obj = CreateObject(type,origin,true,false);
		_objectDic[type].Add(obj);

		return obj;
	}

	public T AddObject<T> (Define.ObjectType type, string name) where T : ObjectBase
	{
		T obj = CreateObject<T>(type,name,true,false);
		_objectDic[type].Add(obj);

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
