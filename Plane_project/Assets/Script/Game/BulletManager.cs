using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Singleton<BulletManager>, Define.IManager {

	private Dictionary<int, Define.SimpleCache<BulletBase>> _cache = 
		new Dictionary<int, Define.SimpleCache<BulletBase>>();
	private Sprite[] _sprites;

	private Action<BulletBase> loop;
	private Action<BulletBase> collisionLoop;
	private PlaneBase _collisionTarget;

	private Vector4 bulletLimit;

	private float deltaTime = 0f;
	private int[] count = {1,1};
	public void firstSetting()
	{
		var res = ResourceManager.GetInstance();
		GameObject obj = res.GetPrefab("BulletBase");
		_sprites = res.GetSpriteSet("SpriteSet/Bullets/Single/");

		loop = new Action<BulletBase>(Loop);
		collisionLoop = new Action<BulletBase>(CollisionLoop);
		
		int end = (int)BulletType.end;
		for(int i = 0; i < end; ++i)
		{
			Define.SimpleCache<BulletBase> cache = new Define.SimpleCache<BulletBase>(obj,(BulletBase e)=>{
				e.SetNecessary();
				e.firstSetting();

				e.SetSprite(_sprites[0]);
			});
			cache.CreateObject(count[i]);

			_cache.Add(i, cache);
		}
	}

	public BulletBase Active(BulletType type, Vector3 pos, Vector3 dir, float speed, int sprite,float timer = 1f)
	{
		BulletBase bullet = _cache[(int)type].ActiveObject();
		bullet.Active(pos,dir,speed,timer);
		bullet.SetSprite(_sprites[sprite]);

		SoundManager.instance.Play("SE/GunShot_",false,3);

		return bullet;
	}

	public BulletBase Active(BulletType type, Vector3 pos, Vector3 dir, float speed, string ani, bool loop,float timer = 1f)
	{
		var bullet = _cache[((int)type)].ActiveObject();
		bullet.Active(pos,dir,speed,timer);
		bullet.SetAnimation(ani,loop);

		SoundManager.instance.Play("SE/GunShot_",false,3);

		return bullet;
	}

	public void Loop(BulletBase bullet)
	{
		bullet.progress(deltaTime);


		if(!bullet.LimitCheck(bulletLimit))
			bullet.UpdateTransform();

		bullet.afterProgress(deltaTime);
	}

	public void CollisionLoop(BulletBase bullet)
	{
		if(bullet.canCollision)
		{
			if(_collisionTarget.CollisionBullet(bullet))
			{
				if(!bullet.penetrate)
					bullet.SetActive(false);
				
				bullet.AddCollisionList(_collisionTarget);
			}
		}
	}

	public void Sync(BulletBase bullet)
	{
	}

	public void initialize()
	{

	}

	public void CollisionCheck(PlaneBase target, BulletType type)
	{
		_collisionTarget = target;

		Define.SimpleCache<BulletBase> c;
		_cache.TryGetValue((int)type,out c);

		c.Loop(collisionLoop);
	}

	public void progress(float del)
	{
		bulletLimit = CameraControll.instance.GetCamBounds();
		deltaTime = del;

		foreach(var item in _cache)
		{
			item.Value.Loop(loop);
		}
	}

	public void lateProgress(float deltaTime)
	{

	}
}
