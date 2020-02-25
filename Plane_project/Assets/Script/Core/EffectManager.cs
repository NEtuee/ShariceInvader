using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>, Define.IManager  {

	private ResourceManager _resManager;
	private Define.SimpleCache<EffectBase> _cache;

	private const int count = 100;
	private float timer = 0f;
	private Action<EffectBase> _effectProgress;

	private Dictionary<string,ParticleSystem> _particles = new Dictionary<string, ParticleSystem>();
	private ParticleSystem.EmitParams _param = new ParticleSystem.EmitParams();
	private ParticleSystem.EmitParams _scaledParam = new ParticleSystem.EmitParams();

	public void firstSetting()
	{
		_resManager = ResourceManager.GetInstance();

		GameObject obj = _resManager.GetPrefab("EffectBase");
		//_resManager.Load("Prefab/EffectBase",typeof(GameObject)) as GameObject;

		_cache = new Define.SimpleCache<EffectBase>(obj,(EffectBase e)=>{
			e.SetNecessary();
			e.firstSetting();
		});
		_cache.CreateObject(count);

		_effectProgress = new Action<EffectBase>(Loop);

		GetParticleSystems();
	}

	public EffectBase AddEffect(Vector2 pos, string ef, bool loop = false, ObjectBase target = null)
	{

		return _cache.ActiveObject().Active(pos,_resManager.GetSpriteSet(ef,1),loop,target);
	}

	public EffectBase AddEffect(Vector2 pos, Sprite sprite, float timer, ObjectBase target = null)
	{
		return _cache.ActiveObject().Active(pos,sprite,timer,target);
	}

	public void GetParticleSystems()
	{
		GameObject obj = GameObject.FindGameObjectWithTag("ParticleSystems");
		int count = obj.transform.childCount;

		for(int i = 0; i < count; ++i)
		{
			Transform tp = obj.transform.GetChild(i);
			_particles.Add(tp.name, tp.GetComponent<ParticleSystem>());
		}
	}

	public void progress(float deltaTIme)
	{
		timer = deltaTIme;
		_cache.Loop(_effectProgress);
	}

	public void Loop(EffectBase e)
	{
		e.progress(timer);
		e.UpdateTransform();
	}

	public void EmitParticles(string name, Vector3 pos,int count)
	{
		ParticleSystem sys;
		if(_particles.TryGetValue(name,out sys))
		{
			_param.position = pos;
			
			sys.Emit(_param,count);
		}
	}

	public void EmitParticles(string name, Vector3 pos,float lifeTime, float size, int count)
	{
		ParticleSystem sys;
		if(_particles.TryGetValue(name,out sys))
		{
			_scaledParam.position = pos;
			_scaledParam.startLifetime = lifeTime;
			_scaledParam.startSize = size;
			sys.Emit(_scaledParam,count);
		}
	}

	public void EmitParticles(string name, Vector3 pos, float angle, int count)
	{
		ParticleSystem sys;
		if(_particles.TryGetValue(name,out sys))
		{
			_param.position = pos;
			_param.rotation = angle;
			sys.Emit(_param,count);
		}
	}

	public void ExplosionSmoke(Vector3 start, Vector3 end,float startSize,float endSize, int count)
	{
		ParticleSystem sys;
		if(_particles.TryGetValue("ExplosionSmoke",out sys))
		{
			Color sc = Color.black;
			Color ec = new Color(0.7f,0.7f,0.7f);
			Vector3 dir = (end - start).normalized;
			for(int i = 0; i < count; ++i)
			{
				float timer = (float)i / (float)count;
				Vector3 pos = Vector3.Lerp(start,end,timer);
				_scaledParam.position = pos + new Vector3(UnityEngine.Random.Range(-0.05f,0.05f),UnityEngine.Random.Range(-0.05f,0.05f));
				_scaledParam.startLifetime = Mathf.Lerp(1f,1.8f,timer);
				_scaledParam.startColor = Color.Lerp(sc,ec,timer);
				_scaledParam.startSize = Mathf.Lerp(startSize,endSize,timer);//0.44f - (float)i * 0.01f;
				_scaledParam.velocity = dir * (0.04f + ((float)i * 0.005f));

				sys.Emit(_scaledParam,1);
			}
		}
	}

	public void Explosion(Vector3 pos,int count,float randFactor = 0.2f,float startSize = 0.09f, float endSize = 0.18f)
	{
		ParticleSystem sys;
		if(_particles.TryGetValue("ExplosionCircle",out sys))
		{
			for(int i = 0; i < count; ++i)
			{
				_param.position = pos + new Vector3(UnityEngine.Random.Range(-randFactor,randFactor),
													UnityEngine.Random.Range(-randFactor,randFactor));
				
				sys.Emit(_param,1);
			}
		}
	}

	public void lateProgress(float delatTime)
	{

	}
}
