using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collisionable : Drawable {

	public Define.SimpleCollider coll{get{return _collider;}}
	public int collisionCount{get{return _collisions.Count;}}
	public bool noClip{get{return _noclip;}}
	public bool allowMultiCollision = false;

	
	protected Define.SimpleCollider _collider;

	protected List<Collisionable> _collisions = new List<Collisionable>();

	protected bool _noclip = false;

	public void SetCollider(Define.SimpleCollider col)
	{
		_collider = col;
	}

	public void SetNoClip(bool val) {_noclip = val;}

	public void UpdateCollider()
	{
		_collider.UpdateBound(_position);
	}

	public virtual bool CollisionCheck(Collisionable target)
	{
		if(_collisions.Contains(target))
			return false;
			
		target.UpdateCollider();
		UpdateCollider();

		bool c = _collider.CollisionCheck(target.coll);

		if(c)
		{
			if(!target.allowMultiCollision)
				_collisions.Add(target);
			target.CollisionSync(this);
		}
		
		return c;
	}

	public virtual void CollisionProgress(Define.ObjectType type, Collisionable target)
	{
		
	}

	public void RegisteCollisionList()
	{
		CollisionManager.GetInstance().RegisteCollisionList(this);
	}

	public void CollisionSync(Collisionable target)
	{
		if(!_collisions.Contains(target) && !target.allowMultiCollision)
			_collisions.Add(target);
	}

	public override void afterProgress(float deltaTime)
	{
		CollisionListUpdate();
	}

	public void AddCollisionList(Collisionable target) {_collisions.Add(target);}

	public void CollisionListUpdate()
	{
		for(int i = 0; i < _collisions.Count;)
		{
			if(_collisions[i] == null || !_collider.CollisionCheck(_collisions[i].coll))
			{
				_collisions.RemoveAt(i);
			}
			else
				++i;
		}
	}

	public bool IsInCollisionList(Collisionable target) {return _collisions.Contains(target);}
}
