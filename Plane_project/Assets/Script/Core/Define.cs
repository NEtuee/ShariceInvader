using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Define
{

public static class PhysicsSetting
{
	public static float gravity = -2.45f;
}

public interface IProgress
{
	void firstSetting();
	void initialize();
	void progress(float deltaTime);
	void release();
}


public interface IManager
{
	void firstSetting();
	void progress(float deltaTIme);
	void lateProgress(float delatTime);
	//void release();
}

public enum TouchState
{
	Began,
	Pushed,
	Moved,
	Drag,
	End,
	None,
}

public enum ObjectType
{
	player = 0,
	enemy,
	objects,
	effect,
	AutoProgressEnd,
	//여기부터 수동
}

class SimpleSortedThings<T>
{
	public class Link
	{
		public int level;
		public T item;

		public Link prev;
		public Link next;

		public int Compare(int target)
		{
			return level >= target ? 0 : 1;
		}

		public void Set(int l, T i)
		{
			level = l;
			item = i;
		}

		public Link(int l, T i)
		{
			Set(l,i);
		}
	}

	public int Count{get{return _count;}}

	private int _count;
	private Link _head;
	private Queue<Link> _cache = new Queue<Link>();

	private Link GetItem(int level, T item)
	{
		Link link;
		if(_cache.Count == 0)
			link = new Link(level,item);
		else
		{
			link = _cache.Dequeue();
			link.Set(level,item);
		}

		return link;
	}

	public Link GetHead() {return _head;}

	public void CutLink(Link link)
	{
		if(link == _head)
		{
			if(link.next != null)
				link.next.prev = null;
			_head = link.next;
		}
		else
		{
			link.prev.next = link.next;

			if(link.next != null)
				link.next.prev = link.prev;
		}

		--_count;
		_cache.Enqueue(link);
	}

	public void Insert(int level, T item)
	{
		var i = GetItem(level,item);

		if(_count == 0)
		{
			_head = i;
			_head.prev = null;
			_head.next = null;
		}
		else
		{
			var link = _head;
			Link prev = link;

			while(link != null)
			{
				if(link.Compare(i.level) == 0)
				{
					if(_head == link)
					{
						i.next = _head;
						i.prev = null;

						_head.prev = i;
						_head = i;
					}
					else
					{
						prev.next = i;
						i.prev = prev;
						i.next = link;
						link.prev = i;
					}
					break;
				}

				if(link.next == null)
				{
					link.next = i;
					break;
				}

				prev = link;
				link = link.next;
			}

		}

		++_count;
	}

}

public class SimpleRect
{
	public Vector2 center{get{return _pos;}}
	public Vector2 box{get{return new Vector2(_x,_y);}}
	public float left{get{return _left;}}
	public float right{get{return _right;}}
	public float up{get{return _up;}}
	public float down{get{return _down;}}


	private Vector2 _pos;

	private float _left;
	private float _right;
	private float _up;
	private float _down;

	private float _x;
	private float _y;

	public bool Collapse(SimpleRect target)
	{
		if(_left < target._right &&
			_up > target._down &&
			_right > target.left &&
			_down < target.up)
		{
			return true;
		}
		
		return false;
	}

	public void SetRect(float x, float y)
	{
		_x = x;
		_y = y;
	}

	public void UpdateRect(Vector2 pos)
	{
		_pos = pos;
		_left = pos.x - _x;
		_right = pos.x + _x;
		_up = pos.y + _y;
		_down = pos.y - _y;
	}

	public SimpleRect(float x, float y){SetRect(x,y);}
	public SimpleRect(float x, float y, Vector2 pos)
	{
		SetRect(x,y);
		UpdateRect(pos);
	}
}

public abstract class SimpleCollider
{
	public int type{get{return _type;}}
	public SimpleRect bound{get{return _bound;}}
	protected int _type;
	protected SimpleRect _bound;

	public void UpdateBound(Vector2 pos)
	{
		bound.UpdateRect(pos);
	}

	public bool BoundCheck(SimpleRect target)
	{
		return bound.Collapse(target);
	}

	public abstract bool CollisionCheck(SimpleCollider col);

	public static bool CircleLineCircle(Vector2 point,Vector2 lineStart,Vector2 lineEnd,float radiusOne, float radiusTwo)
	{
		float startDist = Vector2.Distance(point,lineStart);
		float endDist = Vector2.Distance(point,lineEnd);
		float lineLen = Vector2.Distance(lineStart,lineEnd);
		float collisionDist = radiusOne + radiusTwo;

		if(startDist <= collisionDist)
			return true;
		else if(endDist <= collisionDist)
			return true;
		else
		{
			if(startDist > lineLen || endDist > lineLen)
			{
				return false;
			}
			return MathEx.DistanceFromPointToLine(point,lineStart,lineEnd) <= radiusOne + radiusTwo;
		}
	}

	public static bool PointInCircle(Vector2 point, Vector2 circlePos, float radius)
	{
		return (point.x - circlePos.x) * (point.x - circlePos.x) +
				(point.y - circlePos.y) * (point.y - circlePos.y) < radius * radius;
	}

	public static bool CircleCollapse(Define.SimpleRect one, Define.SimpleRect two)
	{
		float dist = Vector2.Distance(one.center, two.center);

		return one.box.x + two.box.x >= dist;
	}

	public static bool IntersectRectCircle(Define.SimpleRect circle, Define.SimpleRect rect)
	{
		Vector2 circlePos = circle.center;
		float radius = circle.box.x;

		int zone = GetRectZone(circlePos,rect);

		if(zone == 1)
		{
			return rect.down - radius <= circlePos.y;
		}
		else if(zone == 3)
		{
			return rect.left - radius <= circlePos.x;
		}
		else if(zone == 4)
		{
			return true;
		}
		else if(zone == 5)
		{
			return rect.right + radius >= circlePos.x;
		}
		else if(zone == 7)
		{
			return rect.up + radius >= circlePos.y;
		}
		else
		{
			Vector2 vec = new Vector2(zone == 0 || zone == 6 ? rect.left : rect.right,
					zone == 0 || zone == 2 ? rect.down : rect.up);

			return PointInCircle(vec,circlePos,radius);
		}
	}

	public static bool PointInRect(Vector2 point, Define.SimpleRect rect)
	{
		return point.x >= rect.left && point.x <= rect.right &&
				point.y >= rect.down && point.y <= rect.up;
	}

	public static int GetRectZone(Vector2 circlePos, Define.SimpleRect rect)
	{
    	int xZone = ( circlePos.x <  rect.left ) ? 0 : ( circlePos.x >  rect.right ) ? 2 : 1;
    	int yZone = ( circlePos.y <  rect.down ) ? 0 : ( circlePos.y >  rect.up ) ? 2 : 1;
    	int nZone = xZone + 3*yZone;

    	return nZone;
	}
}

public class SimpleBoxCollider : SimpleCollider
{
	public override bool CollisionCheck(SimpleCollider col)
	{	
		if(col.type == _type)
			return bound.Collapse(col.bound);
		if(col.type == 1)
		{
			return IntersectRectCircle(col.bound,_bound);
		}
		
		// if(bound.Collapse(col.bound))
		// {
		// 	if(col.type == _type)
		// 		return true;
		// 	else if(col.type == 1)
		// 	{
		// 		return IntersectRectCircle(col.bound,_bound);
		// 	}
		// }

		return false;
	}

	public SimpleBoxCollider(float x, float y,Vector2 pos)
	{
		_bound = new SimpleRect(x,y,pos);
		_type = 0;
	}
}

public class SimpleCircleCollider : SimpleCollider
{
	public override bool CollisionCheck(SimpleCollider col)
	{	
		if(col.type == _type)
			return CircleCollapse(col.bound,_bound);
		if(col.type == 0)
		{
			return IntersectRectCircle(_bound,col.bound);
		}

		// else if(bound.Collapse(col.bound))
		// {
		// 	if(col.type == 1)
		// 	{
		// 		return IntersectRectCircle(col.bound,_bound);
		// 	}
		// }

		return false;
	}

	public void Setup(float x, float y,Vector2 pos)
	{
		_bound.SetRect(x,y);
		_bound.UpdateRect(pos);
	}

	public SimpleCircleCollider(float x, float y,Vector2 pos)
	{
		_bound = new SimpleRect(x,y,pos);
		_type = 1;
	}
}

public delegate void VoidObjectDelegate(ObjectBase obj);
public delegate bool BoolObjectDelegate(ObjectBase obj);

[System.Serializable]
	public class SimpleCache<T> where T : Behaviour
	{
		private List<T> _mainList = new List<T>();
		private Queue<T> _cacheQueue = new Queue<T>();

		private System.Action<T> _firstSetting;
		private GameObject _baseObj;

		public void CreateObject(int count)
		{
			for(int i = 0; i < count; ++i)
			{
				T target = GameObject.Instantiate(_baseObj).GetComponent<T>();

				_firstSetting(target);
				// text.init();
				target.gameObject.SetActive(false);
				_cacheQueue.Enqueue(target);
			}
		}

		public void Loop(System.Action<T> callBack)
		{
			for(int i = 0; i < _mainList.Count;)
			{
				callBack(_mainList[i]);

				if(!_mainList[i].gameObject.activeSelf)
				{
					_cacheQueue.Enqueue(_mainList[i]);
					_mainList.RemoveAt(i);
				}
				else
					++i;
			}
		}

		public T ActiveObject()
		{
			if(_cacheQueue.Count == 0)
				CreateObject(1);
			T target = _cacheQueue.Dequeue();

			_mainList.Add(target);

			return target;
		}

		public void DisableAllObject()
		{
			for(int i = 0; i < _mainList.Count; ++i)
			{
				_mainList[i].gameObject.SetActive(false);
				_cacheQueue.Enqueue(_mainList[i]);
			}

			_mainList.Clear();
		}

		public SimpleCache(GameObject obj,System.Action<T> firstSetting)
		{
			_baseObj = obj;
			_firstSetting = firstSetting;
		}
	}

	public class GizmoHelper
	{
		private Vector2[] _rectPoint = new Vector2[4];
		public void GetRectPoint(Vector2 center, float width, float height)
		{
			_rectPoint[0] = new Vector2(center.x - width, center.y - height);
			_rectPoint[1] = new Vector2(_rectPoint[0].x, center.y + height);
			_rectPoint[2] = new Vector2(center.x + width, _rectPoint[1].y);
			_rectPoint[3] = new Vector2(_rectPoint[2].x, _rectPoint[0].y);
		}

		public void GetLeftBottomRectPoint(Vector2 leftBottom, float width, float height)
		{
			_rectPoint[0] = new Vector2(leftBottom.x, leftBottom.y);
			_rectPoint[1] = new Vector2(_rectPoint[0].x, leftBottom.y + height);
			_rectPoint[2] = new Vector2(leftBottom.x + width, _rectPoint[1].y);
			_rectPoint[3] = new Vector2(_rectPoint[2].x, _rectPoint[0].y);
		}
		public void DrawRect(Vector2 center, float width, float height)
		{
			GetRectPoint(center,width,height);

			for(int i = 0; i < 4; ++i)
			{
				Gizmos.DrawLine(_rectPoint[i],_rectPoint[i + 1 == 4 ? 0 : i + 1]);
			}
		}

		public void DrawLeftBottomCenterRect(Vector2 leftBottom, float width, float height)
		{
			GetLeftBottomRectPoint(leftBottom,width,height);

			for(int i = 0; i < 4; ++i)
			{
				Gizmos.DrawLine(_rectPoint[i],_rectPoint[i + 1 == 4 ? 0 : i + 1]);
			}
		}
	}

}