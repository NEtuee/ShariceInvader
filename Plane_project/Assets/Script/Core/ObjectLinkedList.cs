using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LinkBase<T> where T : ObjectBase
{
	public T target;
	public LinkBase<T> next = null;
	public LinkBase<T> prev = null;

	public void Link(LinkBase<T> link)
	{
		link.next = this;

		this.prev = link;
	}

	public void Link(LinkBase<T> p, LinkBase<T> n)
	{
		p.next = this;
		n.prev = this;

		this.next = n;
		this.prev = p;
	}

	public LinkBase(){}
	public LinkBase(T t){target = t;}
}

public class LinkedList<T> where T : ObjectBase
{
	public int count{get{return _count;}}
	public LinkBase<T> front{get{return _front;}}

	private int _count = 0;

	private Action<T> _destroy = (T obj) => {GameObject.Destroy(obj.gameObject);};
	private Define.BoolObjectDelegate _deleteCondition = delegate{return false;};

	private LinkBase<T> _front = null;
	private LinkBase<T> _back = null;

	private Queue<LinkBase<T>> _linkCache = new Queue<LinkBase<T>>();

	public void Progress(float deltaTime)
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			if(link.target.active)
				link.target.progress(deltaTime);

			link = link.next;
		}

	}

	public void AfterProgress(float deltaTime)
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			if(link.target.active)
				link.target.afterProgress(deltaTime);

			link = link.next;
		}
	}

	public void DeleteProgress()
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			if(_deleteCondition(link.target))
			{
				_destroy(DisconnectLink(link));
			}

			link = link.next;
		}
	}

	public void UpdateTransform()
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			if(link.target.active)
			{
				link.target.UpdateTransform();
				link.target.afterUpdateTransform();
			}
			link = link.next;
		}
	}

	public void RemoveAll()
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			_destroy(DisconnectLink(link));

			link = link.next;
		}
	}

	public void SetDeleteCondition(Define.BoolObjectDelegate del) {_deleteCondition = del;}
	public void SetDeleteAction(Action<T> action) {_destroy = action;}

	public void Loop(Define.VoidObjectDelegate del)
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			del(link.target);

			link = link.next;
		}
	}
		
	public LinkBase<T> Add(T target)
	{
		LinkBase<T> link;
		if(_linkCache.Count == 0)
		{
			link = new LinkBase<T>(target);
		}
		else
		{
			link = _linkCache.Dequeue();
			link.target = target;
			link.next = link.prev = null;
		}

		if(_front == null)
		{
			_front = link;
			_back = _front;
		}
		else
		{
			link.Link(_back);
			_back = link;
		}

		++_count;
		
		return link;
	}

	public T DisconnectLink(LinkBase<T> link)
	{
		if(link == null)
		{
			Debug.Log("link is null");
			return null;
		}

		if(link == _front)
		{
			_front = link.next;
			if(_front != null)
				_front.prev = null;
		}
		else if(link == _back)
		{
			_back = link.prev;
			if(_back != null)
				_back.next = null;
		}
		else
		{
			link.prev.next = link.next;
			link.next.prev = link.prev;
		}

		_linkCache.Enqueue(link);

		--_count;
		return link.target;
	}

	public T RemoveTarget(T target)
	{
		LinkBase<T> link = Find(target);
		if(link != null)
			return DisconnectLink(link);
		return null;
	}

	public LinkBase<T> Find(string target)
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			if(link.target.name == target)
			{
				return link;
			}
			link = link.next;
		}
		return null;
	}

	public LinkBase<T> Find(T target)
	{
		LinkBase<T> link = _front;
		while(link != null)
		{
			if(link.target == target)
			{
				return link;
			}
			link = link.next;
		}
		return null;
	}
}