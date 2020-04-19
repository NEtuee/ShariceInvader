using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaneBase : Collisionable {

	public class DelayItem
	{
		public delegate void hitEventDelegate(PlaneBase plane);

		public hitEventDelegate hitEvent;
		public int attack;
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

		public void Set(int a, float t)
		{
			attack = a;
			timer = t;
		}

		public DelayItem(int a, float t)
		{
			Set(a,t);
		}
	}


	public enum AnimationType
	{
		Vertical = 0,
		Vertical_Angled,
		Vertical_Velocity,
		Horizontal,
		None,
		End,
	}

	public bool activeBody = true;

	public Vector3 velocity{get{return _velocity;}}
	public int bodyAttack{get{return _bodyAttack;}}
	public bool controllLock{get{return _controllLock;}}
	public float controllLockTimer{get{return _controllLockTimer;}}
	public bool immortal{get{return _immortal;}}
	public float dodgeDist{get{return _dodgeDist;}}
	public float maxSpeed{get{return _maxSpeed;}}


	public WeaponBase mainWeapon;

	protected Sprite[] _dirSprites;
	protected Vector3 _velocity = new Vector2();
	protected Vector3 _friction = new Vector2();
	public float _mass = 1f;
	protected float _frictionFactor = 0.01f;
	public float _gravityScale = .7f;
	protected float _maxSpeed = 5f;
	public float _dodgeDist = 1f;
	private float _additionalSpeed;
	private float _addSpeedTimer = 0f;
	private float _addSpeedTime = 0f;

	public bool _burst = true;
	protected bool _dodge = true;
	public bool _rotateLock = false;
	public bool _directionAngle = false;
	protected bool _controllLock = false;
	protected bool _velocityFlip = true;
	protected bool _immortal = false;
	private bool _lerpAddSpeed = false;
	public bool _bounceOff = false;

	protected int _hp = 1;
	protected int _bodyAttack = 1;
	protected int _spritePoint = 0;

	protected int _Ani_angleCount = 0;
	protected int _Ani_verticalCount = 0;


	protected bool _fall = false;
	protected bool _fallExplosion = false;
	protected bool _trailEmmit = false;
	protected bool _boostAniProgress = false;
	private float _fallTimer = 1f;
	protected float _controllLockTimer = 0f;
	protected float _spriteDisapearTimer = 0f;
	protected float _spriteAngle;

	protected List<TrailRenderer> _trail = new List<TrailRenderer>();
	public List<AnimationControllEx> _boostAni = new List<AnimationControllEx>();
	private Material _trailMat;
	protected Transform miniMapIcon;

	protected AnimationType _aniType;

	protected Editor_PlaneInfoBase _infoBase = null;

	protected DecoAnimeController _deco;


	protected LineRenderer directionViewer;
	protected LineRenderer velocityViewer;


	private List<DelayItem> _delayHitList = new List<DelayItem>();
	private static Queue<DelayItem> _delayPool = new Queue<DelayItem>();

	public override void firstSetting()
	{
		base.firstSetting();

		AddSortingGroup();
		MiniMapIconSetup();

		_deco = new DecoAnimeController(tp);
	}

	public void BasicInitialize()
	{
		_burst = true;
		_rotateLock = false;
		_directionAngle = false;
		_controllLock = false;
		_fall = false;
		_fallExplosion = false;
		_immortal = false;

		_fallTimer = Random.Range(1f,1.5f);

		_controllLockTimer = 0f;
		_spriteDisapearTimer = 0f;

		SetNoClip(false);

		miniMapIcon.gameObject.SetActive(true);
	}

	public void SetBodyAttack(int value) {_bodyAttack = value;}
	public void SetImmortal(bool value) {_immortal = value;}
	public void SetMaxSpeed(float value) {_maxSpeed = value;}
	public void SetControll(bool value) {_controllLock = value;}

	public void AddForce(Vector3 f)
	{
		_velocity += f;
		MathEx.nearZero(ref _velocity);
	}

	public void SetAbsoluteForce(Vector3 f)
	{
		_velocity = f;
		MathEx.nearZero(ref _velocity);
	}

	public void ChangeDirection(Vector3 dir)
	{
		Vector3 vel = _velocity;
		Vector3 normal = vel.normalized;

		vel.x = normal.x != 0 ? dir.x * (vel.x / normal.x) : dir.x;
		vel.y = normal.y != 0 ? dir.y * (vel.y / normal.y) : dir.y;

		SetAbsoluteForce(vel);
	}

	public void ApplyForce(Vector3 f)
	{
		AddForce(f / _mass);
	}

	public void FrictionCheck()
	{
		_friction = -_velocity.normalized * (_frictionFactor);
		if(MathEx.Vector3Compare(_friction,_velocity) == 1)
		{
			_friction = _velocity = Vector2.zero;
		}
		AddForce(_friction);
	}

	public void GravityCheck(float deltaTime)
	{
		//AddForce(new Vector2(0f,(Define.PhysicsSetting.gravity * _mass * _gravityScale)));
		float g = _gravityScale;
		if(_controllLock)
		{
			g *= 3f;
		}
		AddForce(new Vector2(0f,deltaTime * (Define.PhysicsSetting.gravity * _mass * g)));
	}

	public void SetAdditionalSpeed(float factor, float time, bool lerp = false)
	{
		_additionalSpeed = factor;
		_addSpeedTimer = 0f;
		_addSpeedTime = time;
		_lerpAddSpeed = lerp;
	}

	public virtual void WeaponChange(WeaponBase weapon)
	{
		if(mainWeapon != null)
			mainWeapon.WhenChanged();
		EffectManager.GetInstance().AddEffect(position,"WeaponChange",false,this).SetSortingOrder(1);
		mainWeapon = weapon;
		mainWeapon.SetTarget(this);
		mainWeapon.Initialize();
		mainWeapon.Change();
	}

	public void Dodge(Vector3 dir, bool effectActive = true)
	{
		_dodge = false;
		Vector3 pos = _position;
		Vector3 dirPos = dir / 10f;
		
		_position = pos + dir * _dodgeDist;

		if(effectActive)
		{
			EffectManager.GetInstance().AddEffect(pos + dir * (_dodgeDist / 2f),"DodgeSlash")
									.SetAngle(MathEx.directionToAngle(dir));

			int t = (int)_dodgeDist * 10;
			for(int i = 0; i < t; ++i)
			{
				pos = pos + dirPos;	
				EffectManager.GetInstance().EmitParticles("DodgeEffect",pos,1);
			}
		}

		ChangeDirection(dir);
	}

	public virtual void BurstActive(bool effectActive = true)
	{
		SetAdditionalSpeed(2f,0.3f,true);
		SetAbsoluteForce((_direction * 150f));

		if(_boostAniProgress)
		{
			foreach(var ani in _boostAni)
			{
				ani.ChangeAni("Burst",false);
			}
		}
		_burst = false;


		if(effectActive)
		{
			Vector3 pos = _position;
			pos += _direction.normalized * 0.1f;
			EffectManager.GetInstance().AddEffect(pos,"Burst")
										.SetAngle(MathEx.directionToAngle(_direction));
										// .SetDirection(-_direction)
										// .SetSpeed(_velocity.magnitude * .5f);

			EffectBase effect = EffectManager.GetInstance().AddEffect(_position,"Electric");
			effect.SetAngle(MathEx.directionToAngle(_direction));
			effect.SetSortingOrder(1);
		}
	}

	public virtual void Hit(PlaneBase target,bool hit = true)
	{
		if(hit && !target._fall)
			Hit(target.bodyAttack);
	}

	public virtual void Hit(int val)
	{
		DecreaseHP(val);
	}

	public virtual void Hit(BulletBase bullet)
	{
		float ang = MathEx.directionToAngle((-bullet.direction));

		EffectBase effect = EffectManager.GetInstance().AddEffect(_position,"Hit");
		effect.SetAngle(ang);
		effect.SetSortingOrder(1);

		Hit(bullet.attack);
	}

	public void AddDelayAttackList(int attack, float time, DelayItem.hitEventDelegate hitEvent)
	{
		DelayItem item = null;

		if(_delayPool.Count != 0)
		{
			item = _delayPool.Dequeue();
			item.Set(attack,time);
		}
		else
		{
			item = new DelayItem(attack,time);
		}

		item.hitEvent = hitEvent;

		_delayHitList.Add(item);
	}

	public void DelayAttackProgress(float deltaTime)
	{
		for(int i = 0; i < _delayHitList.Count;)
		{
			if(_delayHitList[i].progress(deltaTime))
			{
				_delayHitList[i].hitEvent(this);
				DecreaseHP(_delayHitList[i].attack);

				if(deleted)
					return;

				_delayPool.Enqueue(_delayHitList[i]);

				_delayHitList.RemoveAt(i);
			}
			else
				++i;
		}
	}

	public override bool CollisionCheck(Collisionable target)
	{
		if(_noclip || target.noClip)
			return false;

		bool val = base.CollisionCheck(target);

		return val;
	}

	public bool CollisionBullet(BulletBase target)
	{
		if(CollisionCheck(target))
		{
			Hit(target);
			return true;
		}
		return false;
	}

	public override void CollisionProgress(Define.ObjectType type, Collisionable target)
	{
		if(type != Define.ObjectType.enemy && type != Define.ObjectType.player)
			return;
		
		var plane = (PlaneBase)target;

		bool b = mainWeapon == null ? false : mainWeapon.CollisionCheck(plane);

		if(!b)
		{
			Hit(plane);

			// if(_bounceOff)
			// {
			// 	plane.SetAbsoluteForce(-plane.velocity);
			// }
		}

	}


	public void DecreaseHP(int value)
	{
		if(!_immortal)
		{
			_hp -= value;
			if(_hp <= -2)
				Delete();

			if(_hp <= 0 && !_fall && !deleted)
			{
				_controllLock = true;
				_fall = true;

				EffectManager.GetInstance().Explosion(_position,10);
				_bodyAttack = 0;
				// Delete();
			}

			WhenDecreaseHP();
		}
	}

	public virtual void WhenDecreaseHP(){}

	public void BasicUpdate(float deltaTime)
	{
		DelayAttackProgress(deltaTime);

		_deco.DecoAniProgress(deltaTime);
		
		if(_trailEmmit)
		{
			_trailMat.SetFloat("_random",Random.Range(40000f,50000f));
		}
		foreach(var trail in _trail)
		{
			trail.emitting = _speed != 0f;
			trail.emitting = trail.emitting ? !_controllLock : false;
			trail.emitting = trail.emitting ? _trailEmmit : false;
		}

		foreach(var ani in _boostAni)
		{
			
			if(_boostAniProgress)
			{
				if(ani.AnimationProgress(deltaTime) == -1)
					if(!ani.ChangeAni("Loop",true))
						_boostAniProgress = false;
				ani._sprRenderer.enabled = _speed != 0f;
			}
			else
			{
				ani._sprRenderer.enabled = _boostAniProgress;
			}
		}

		if(_controllLockTimer != 0f)
		{
			_controllLockTimer = _controllLock ? _controllLockTimer - deltaTime : 0f;

			if(_controllLockTimer <= 0f)
			{
				_controllLockTimer = 0f;
				_controllLock = false;
			}
		}

		if(_spriteDisapearTimer != 0f)
		{
			_spriteDisapearTimer -= deltaTime;

			if(_spriteDisapearTimer <= 0f)
			{
				_spriteDisapearTimer = 0f;
				_sprRenderer.enabled = true;

				EffectManager.GetInstance().AddEffect(_position,"Burst")
										.SetAngle(MathEx.directionToAngle(_direction));
			}
		}

		//_trail.time = Timer.GetInstance().TimeScaling(_trail.time);

		if(_direction.magnitude != 0)
		{
			if(_burst)
			{
				BurstActive();
			}
			else if(!_controllLock)
			{
				AddForce(_direction * _speed);
			}
			else if(_fall)
			{
				_fallTimer -= deltaTime;
				EffectManager.GetInstance().EmitParticles("Smoke",_position,1);
				if(_fallTimer <= 0f)
				{
					Delete();
				}
				else if(_fallTimer <= 0.7f && !_fallExplosion)
				{
					_fallExplosion = true;
					EffectManager.GetInstance().AddEffect(_position,"Explosion").SetSortingOrder(2).SetAngle(Random.Range(0f,360f));
					EffectManager.GetInstance().Explosion(_position,15,0.2f);
				}
			}
		}
		else
			_burst = true;

		FrictionCheck();
		GravityCheck(deltaTime);

		if(_immortal)
		{
			Color col = _sprRenderer.color;
			col.a = 0.5f;
			_sprRenderer.color = col;
		}
		else
		{
			Color col = _sprRenderer.color;
			col.a = 1f;
			_sprRenderer.color = col;
		}

		UpdateTrails();
		UpdateBoosts();

		UpdateMiniMapIcon();
		GroundCheck();
	}

	public void SpriteUpdate()
	{
		if(_controllLock)
		{
			return;
		}
		if(_aniType == AnimationType.Horizontal)
		{
			float ang = _eulerAngle;
			ang = MathEx.abs(ang);

			int div = (int)ang / 180;

			ang = ang - (div * 180f);

			_spritePoint = (int)(ang / _spriteAngle);

			SetSprite(_dirSprites[_spritePoint]);
		}
		else if(_aniType == AnimationType.Vertical)
		{
			float ang = MathEx.directionToAngle(_direction);
			
			_spritePoint = (int)(ang / _spriteAngle);

			SetSprite(_dirSprites[_spritePoint]);
		}
		else if(_aniType == AnimationType.Vertical_Angled)
		{
			float ang = MathEx.directionToAngle(_velocity.normalized);
			float m = _velocity.magnitude > 1f ? 1f : _velocity.magnitude;
			

			int yAxis = (int)((MathEx.abs(m - 0.00001f) / 1f) / (1f / (float)_Ani_angleCount));

			_spritePoint = (int)(ang / _spriteAngle) + (_Ani_verticalCount * yAxis);


			SetSprite(_dirSprites[_spritePoint]);
		}
		else if(_aniType == AnimationType.Vertical_Velocity)
		{
			float val = _velocity.normalized.x;
			float pos = (val + 1) / 2f;
			pos = (_spriteAngle - 1f) * pos;
			_spritePoint = ((int)_spriteAngle - 1) - (int)pos;
			//_spritePoint = (int)pos;
			
			SetSprite(_dirSprites[_spritePoint]);
		}
		else if(_aniType == AnimationType.None)
		{
			//SetSprite(_dirSprites[0]);
		}
	}

	public void SetVerticalAngledCount(int verticalCount, int angleCount) //y x
	{
		_Ani_angleCount = angleCount;
		_Ani_verticalCount = verticalCount;
	}

	public void SetSpriteSet(string name , AnimationType type)
	{
		_aniType = type;

		if(_aniType == AnimationType.Horizontal)
		{
			name = name.Replace("\\",string.Empty);
			_dirSprites = ResourceManager.GetInstance().GetSpriteSet(name,2);
			_spriteAngle = 90f / ((float)_dirSprites.Length / 2f);
		}
		else if(_aniType == AnimationType.Vertical)
		{
			_dirSprites = ResourceManager.GetInstance().GetSpriteSet(name,2);
			_spriteAngle = 360f / (float)(_dirSprites.Length - 1);
		}
		else if(_aniType == AnimationType.Vertical_Angled)
		{
			_dirSprites = ResourceManager.GetInstance().GetSpriteSet(name,2);
			_spriteAngle = 360f / (float)(_Ani_verticalCount - 1);
		}
		else if(_aniType == AnimationType.Vertical_Velocity)
		{
			_dirSprites = ResourceManager.GetInstance().GetSpriteSet(name,2);
			_spriteAngle = _dirSprites.Length;
		}
		else if(_aniType == AnimationType.None)
		{
			SetSprite(name);
		}

		LoadPlaneData(name);
	}

	public void LoadPlaneData(string n)
	{
		string nm = (n.Contains("/") ? n.Substring(n.LastIndexOf("/") + 1) : n);
		string path = "Sprites/SpriteSet/Planes/" + n + "/" + nm + "_Plane";

		string[] data = ResourceManager.GetInstance().GetSaveData(path);
		if(data == null)
		{
			Debug.Log(n + " plane data does not exist");
			Debug.Log(path);
			return;
		}

		_infoBase = new Editor_PlaneInfoBase();
		_infoBase.LoadDataFile(data);

		_mass = _infoBase.mass;
		_maxSpeed = _infoBase.maxSpeed;
		_rotateLock = _infoBase.rotateLock;
		_speed = _infoBase.speed;
		_bodyAttack = _infoBase.bodyAttack;
		_boostAniProgress = _infoBase.boostAniProgress;
		_trailEmmit = _infoBase.trailEmmit; 
		_velocityFlip = _infoBase.velocityFlip;
		_directionAngle = _infoBase.directionAngle;
		_dodgeDist = _infoBase.dodgeDist;
		_hp = _infoBase.hp;
		
		int trailCount = _infoBase.trailCount;
		int boostCount = _infoBase.boostCount;

		for(int i = 0; i < trailCount; ++i)
		{
			TrailSetUp(_infoBase.trailPoint[0][i],_infoBase.trailInfo.trailMaterial,
						_infoBase.trailInfo.time,_infoBase.trailInfo.startWidth,_infoBase.trailInfo.endWidth);
		}

		for(int i = 0; i < boostCount; ++i)
		{
			BoostSetUp(_infoBase.boostAni,_infoBase.boostPoint[0][i]);
		}

		UpdateTrails();
		UpdateBoosts();

		//SetSpriteSet(_infoBase.spriteSet,_infoBase.animationType);
	}

	public void UpdateTrails()
	{
		for(int i = 0; i < _trail.Count; ++i)
		{
			_trail[i].transform.localPosition = Vector3.Lerp(_trail[i].transform.localPosition, _infoBase.trailPoint[_spritePoint][i], 0.2f);
			_trail[i].sortingOrder = _infoBase.trailSortingOredrs[_spritePoint][i];
		}
	}

	public void UpdateBoosts()
	{
		if(_infoBase != null)
		{
			for(int i = 0; i < _boostAni.Count; ++i)
			{
				_boostAni[i]._sprRenderer.transform.localPosition = _infoBase.boostPoint[_spritePoint][i];
			}
		}
	}

	float directionX = 0f;
	public void DirectionRotate()
	{
		if(_aniType != AnimationType.Vertical_Angled)
		{
			float a = 10f * -_direction.x;//Mathf.Lerp(directionX,_velocity.normalized.x,0.5f);

			_eulerAngle = Mathf.LerpAngle(_eulerAngle,a,0.2f);
		}
	}

	public void SpriteDisapear(float time)
	{
		_sprRenderer.enabled = false;
		_spriteDisapearTimer = time;
	}

	public void ControllLock(float time)
	{
		_controllLockTimer = time;
		_controllLock = true;
	}

	public override void afterProgress(float deltaTime)
	{
		base.afterProgress(deltaTime);

		if(_velocity == Vector3.zero)
			return;
			
//		direction = _velocity.x == 0f ? direction : (_velocity.x > 0 ? Define.Direction.Right : Define.Direction.Left);
		float ms = _maxSpeed;
		if(_controllLock)
			ms += 2f;

		float addSpeed = 0f;

		if(_additionalSpeed != 0f)
		{
			_addSpeedTimer += deltaTime;

			float t = (_addSpeedTimer / _addSpeedTime);
			addSpeed = _lerpAddSpeed ? Mathf.Lerp(_additionalSpeed,0f,t) : _additionalSpeed;

			if(t >= 1f)
			{
				_additionalSpeed = 0f;
			}
		}
		
		if(_velocity.magnitude >= ms + addSpeed )
		{
			float val = (ms + addSpeed) / _velocity.magnitude;
			_velocity = _velocity * val;
		}

		_position += _velocity * deltaTime;

		if(_velocity.magnitude != 0)
		{
			if(!_rotateLock)
			{
				if(_directionAngle)
					DirectionRotate();
				else
					_eulerAngle = MathEx.directionToAngle(_velocity.normalized);
			}
			

			if(_velocityFlip)
			{
				if(_velocity.x < 0f)
					_scale.y = -1f;
				else if(_velocity.x > 0f)
					_scale.y = 1f;
			}
		}

		SpriteUpdate();
	}

	public override void beforeUpdateTransform()
	{
		if(!deleted)
		{
			foreach(var trail in _trail)
			{
				trail.emitting = false;
				if(trail.positionCount > 0)
				{
					trail.Clear();
				}
			}
		}
	}

	public override void deleteEvent()
	{
		//EffectManager.GetInstance().AddEffect(_position,"Explosion_new").SetAngle(Random.Range(0f,360f));
		
		if(_fall)
		{
			Vector3 randDir = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f)).normalized;

			for(int i = 0; i < 4; ++i)
			{
				float range = Random.Range(0.6f,1.5f);
				EffectManager.GetInstance().ExplosionSmoke(_position,_position + randDir * range,0.13f,0.04f,22);
				randDir = new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f)).normalized;
			}
			
		}
		else
		{
			Vector3 dir = _velocity.normalized;

			for(int i = 0; i < 4; ++i)
			{
				float range = Random.Range(0.6f,1.5f);
				Vector3 targetDir = (dir + new Vector3(Random.Range(-0.7f,0.7f),Random.Range(-0.7f,0.7f))).normalized;
				EffectManager.GetInstance().
							ExplosionSmoke(_position,_position + targetDir * range,0.13f,0.04f,22);
			}
		}

		EffectManager.GetInstance().AddEffect(_position,"Explosion").SetSortingOrder(2).SetAngle(Random.Range(0f,360f));
		EffectManager.GetInstance().Explosion(_position,15,0.2f,0.15f,0.23f);

		BasicDeleteEvents();

		// for(int i = 0; i < 5; ++i)
		// 	ObjectManager.GetInstance().AddObject(Define.ObjectType.effect,"Piece").
		// 				SetDirection(_velocity + new Vector3(Random.Range(-5f,5f),Random.Range(-2f,2f))).SetPosition(_position);
		CameraControll.instance.Zoom(1.7f);
	}

	public void BasicDeleteEvents()
	{
		foreach(var item in  _delayHitList)
		{
			_delayPool.Enqueue(item);
		}

		_delayHitList.Clear();
		_noclip = true;
		miniMapIcon.gameObject.SetActive(false);
		_deco.DestroyAll();
		//CameraControll.instance.Zoom(1.7f);
	}

	public void MiniMapIconSetup()
	{
		miniMapIcon = new GameObject(name + " Icon").transform;
		SpriteRenderer spr = miniMapIcon.gameObject.AddComponent<SpriteRenderer>();
		spr.sprite = ResourceManager.GetInstance().GetSprite("MinimapIcon");
		miniMapIcon.gameObject.layer = LayerMask.NameToLayer("UI");

		CanvasScript.instance.SetChild(miniMapIcon);
	}

	public void GroundCheck()
	{
		if(_position.y <= 0f)
		{
			DecreaseHP(5);
			_velocity.y *= -1f;
		}
	}

	public void UpdateMiniMapIcon()
	{
		PlaceMapper map = ObjectManager.GetInstance()._place;
		ObjectBase center = map.mainObject;

		float width = (map._right.leftBottom.x + map._placeWidth) - map._left.leftBottom.x;
		float gap = (width / 2f) - center.position.x;

		float ratio = position.x + gap;
		// ratio = ratio < map._left.leftBottom.x ? ratio + map._mapWidth : 
		// 		ratio > (map._right.leftBottom.x + map._placeWidth) ? ratio - map._mapWidth : ratio;

		ratio = ratio / width;

		ratio = ratio > 1f ? ratio - 1f : ratio < 0 ? ratio + 1f : ratio;

		miniMapIcon.localPosition = CanvasScript.instance.
								CanvasPosToWorldPos(new Vector2(CanvasScript.instance.canvasWidth * ratio,CanvasScript.instance.canvasHeight - 20));
	}

	public void TrailSetUp(Vector2 pos, string material, float time, float startWidth, float endWidth)
	{
		GameObject trailObj = new GameObject("trail");
		trailObj.transform.position = pos;
		trailObj.transform.SetParent(tp);

		_trailMat = new Material(ResourceManager.GetInstance().GetMaterial(material));
		_trailMat.SetFloat("_random",Random.Range(.5f,1.5f));

		var trail = trailObj.AddComponent<TrailRenderer>();
		trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		trail.receiveShadows = false;
		trail.allowOcclusionWhenDynamic = false;
		trail.material = _trailMat;
		trail.time = time;
		trail.startWidth = startWidth;
		trail.endWidth = endWidth;

		_trail.Add(trail);
	}

	public void BoostSetUp(string n, Vector2 pos)
	{
		SpriteRenderer spr = new GameObject("boost").AddComponent<SpriteRenderer>();
		AnimationControllEx ani = new AnimationControllEx(spr);

		ani.AddAnimation("Loop", n + "/Loop");
		ani.AddAnimation("Burst", n + "/Burst");
		ani.ChangeAni("Loop",true);

		spr.transform.position = pos;
		spr.transform.SetParent(tp);

		_boostAni.Add(ani);
	}

	public void PhysicsDebugSetup()
	{
		directionViewer = new GameObject("dirLine").AddComponent<LineRenderer>();
		velocityViewer = new GameObject("velLine").AddComponent<LineRenderer>();

		directionViewer.startWidth = 0.02f;
        directionViewer.endWidth = 0.02f;

		velocityViewer.startWidth = 0.02f;
        velocityViewer.endWidth = 0.02f;

		directionViewer.material = ResourceManager.GetInstance().GetMaterial("SpriteDefault");
		velocityViewer.material = ResourceManager.GetInstance().GetMaterial("SpriteDefault");

		directionViewer.startColor = Color.green;
		directionViewer.endColor = Color.green;
		
		velocityViewer.startColor = Color.red;
		velocityViewer.endColor = Color.red;

		directionViewer.transform.SetParent(tp);
		velocityViewer.transform.SetParent(tp);
	}

	public void PhysicsDebugUpdate()
	{
		directionViewer.SetPosition(0,_position);
		directionViewer.SetPosition(1,_position + _direction);

		velocityViewer.SetPosition(0,_position);
		velocityViewer.SetPosition(1,_position + _velocity.normalized * 1.2f);
	}
}
