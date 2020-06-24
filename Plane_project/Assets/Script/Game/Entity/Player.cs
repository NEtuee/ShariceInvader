using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : PlaneBase {

	public static Player instance;

	public Vector3 target = new Vector3(0f,0f,0f);

	private float _timer = 0f;
	private CameraControll _cam;
	private Vector3 _controllPoint;
	private Vector3 _dodgeStartPos;

	private Transform _miniMapHeightIcon;
	private TextMesh _angleCount;

	private SpriteRenderer _outline;
	private Sprite[] _outlineSprite;

	int a = 0;

	bool cuttingCurve = true;
	bool _hpRegen = false;
	bool _driveCheck = false;

	float cuttingCurveTimer = 0f;

	float _regenTimer = 0f;

	public override void firstSetting()
	{
		instance = this;

		base.firstSetting();

		SetSpriteSet("SpriteSet/Planes/Player_New",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.05f,.05f,_position));

		_cam = CameraControll.instance;

		_minimapIconHeight = false;

		_speed = 0f;
		_maxSpeed = 3.5f;

		_bodyAttack = 50;
		_dodgeDist = 3f;
		maxHp = _hp = 100;
		_gravityScale = 0.3f;

		_timer = 3f;

		miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sprite = ResourceManager.GetInstance().GetSprite("UI/map_arrow");
		miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
		miniMapIcon.transform.localPosition = CanvasScript.instance.
						CanvasPosToWorldPos(new Vector2(CanvasScript.instance.canvasWidth * 0.5f,CanvasScript.instance.canvasHeight));

		_miniMapHeightIcon = new GameObject(name + " HeightIcon").transform;
		SpriteRenderer spr = _miniMapHeightIcon.gameObject.AddComponent<SpriteRenderer>();
		spr.sprite = ResourceManager.GetInstance().GetSprite("UI/MiniMapHeightIcon");
		_miniMapHeightIcon.gameObject.layer = LayerMask.NameToLayer("UI");

		CanvasScript.instance.SetChild(_miniMapHeightIcon);

		_angleCount = CanvasScript.instance.gameObject.transform.Find("AngleCount").GetComponent<TextMesh>();
		HeightIconUpdate();

		_outline = _deco.AddDeco(Vector2.zero)._sprRenderer;
		_outline.sortingOrder = -1;
		_outline.material = ResourceManager.GetInstance().GetPixelSnapMaterial();
		_outlineSprite = ResourceManager.GetInstance().GetSpriteSet("SpriteSet/Outlines/Player");

		foreach(var ani in _boostAni)
        {
			ani.AddAnimation("CuttingCurve","SpriteSet/Effects/CuttingCurve");
        }
	}

	public override void initialize()
	{
		BasicInitialize();
		
		_burst = true;
		_rotateLock = true;
		_velocityFlip = false;
		//_immortal = true;

		_controllPoint = _direction;

		weaponInven.AddWeapon(WeaponBase.WeaponList.Lancer);
		weaponInven.AddWeapon(WeaponBase.WeaponList.Pulse);
		weaponInven.AddWeapon(WeaponBase.WeaponList.PhantomStinger);

		weaponInven.WeaponChange();

		RegisteCollisionList();
	}

	public override void progress(float deltaTime)
	{
		weaponInven.WeaponProgress(deltaTime);

		_outline.enabled = _immortal;
		if(_immortal)
		{
			_outline.sprite = _outlineSprite[_spritePoint];
		}
		
		if(!_controllLock)
		{
			if(!weaponInven.mainAttack)
			{
				Propel();
				DodgeCheck();
				Look(deltaTime);
			}

			weaponInven.GagueProgress(deltaTime);
		}

		if(ControllerEx.GetInstance().KeyDown("WeaponChange") && !_driveCheck && !weaponInven.mainAttack)
		{
			weaponInven.WeaponChange();
		}
	

		BasicUpdate(deltaTime);

		if(_noclip)
		{
			_timer -= deltaTime;
			if(_timer <= 0f)
			{
				_timer = 3f;
				_noclip = false;
			}
		}

		if(cuttingCurveTimer != 0f)
		{
			cuttingCurveTimer -= deltaTime;
			EffectManager.GetInstance().AddEffect(_position,_sprRenderer.sprite,0.2f)
										.ColorLerp(new Color(1,1,1,1),new Color(1,1,1,0f))
										.SetAngle(Mathf.LerpAngle(_eulerAngle,angle,0.25f));
										
										
			if(cuttingCurveTimer <= 0f)
			{
				cuttingCurveTimer = 0f;
				cuttingCurve = true;
			}
		}

		if(_regenTimer != 0f)
		{
			_regenTimer -= deltaTime;
			if(_regenTimer <= 0f)
			{
				_regenTimer = 0f;
				_hpRegen = true;

				MainHud.instance.ShieldRecover();
			}
		}

		if(_hpRegen)
		{
			ChangeHp(1);

			if(_hp == maxHp)
				_regenTimer = 0f;
			else
				_regenTimer = 0.05f;

			_hpRegen = false;
		}

		BulletManager.GetInstance().CollisionCheck(this,BulletType.enemy);
		HeightIconUpdate();
		MainHud.instance.UpdateScaleBar(-ObjectManager.GetInstance()._place.GetPosPercentage(_position).x);

	}

	public override void ImmortalEffect()
	{
		string s = "";
		int rand = UnityEngine.Random.Range(0,3);
		if(rand == 0)
			s = "0";
		else if(rand == 1)
			s = "1";
		else if(rand == 2)
			s = "2";

		EffectManager.GetInstance().AddEffect(_position,"SpriteSet/Effects/Weapon/Lancer/Shield/" + s,false);
	}

	public override void WhenDecreaseHP(int d)
	{
		if(d > 0)
		{
			_cam.Zoom(2.9f);
			_cam.Glitch(0.3f);
			Timer.SetTimeScaleTimer(0f,0.09f);
			MainHud.instance.ActiveUIGlitch(.774f,.994f,.862f,.0253f,.5f);
			MainHud.instance.ShieldDamage();
			_hpRegen = false;
		}
		else
		{
			_cam.Zoom(2.7f);
			_cam.Glitch(0.05f);
			Timer.SetTimeScaleTimer(0f,0.05f);
		}

		_regenTimer = 3.5f;
		
		Debug.Log("HIT");
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		Timer.SetTimeScale(1f);

		Debug.Log("dead");
	}

	public override void BurstActive(bool effectActive)
	{
		
		base.BurstActive(false);
		_cam.Shake(0.2f, _direction / 15f);
	}

	public void DodgeCheck()
	{
		if(ControllerEx.GetInstance().KeyDown("DriveAttack"))
		{
			_driveCheck = true;

			if(weaponInven.gagueExist)
			{
				Timer.SetTimeScale(0.1f);
			
				Vector3 mouse = _direction;
				if(weaponInven.immedyActiveSpecAttack)
				{
					weaponInven.DriveAttack(mouse);
				}
				else
				{
					weaponInven.DriveOn();
				}
			}
		}
		else if(ControllerEx.GetInstance().KeyUp("DriveAttack") && _driveCheck)
		{
			_driveCheck = false;

			if(weaponInven.gagueExist)
			{
				Vector3 mouse = _direction;
				if(!weaponInven.immedyActiveSpecAttack)
				{
					if(weaponInven.DriveAttack(mouse))
					{
						Dodge(mouse,false);
						_dodge = true;

						_cam.Shake(0.2f, _direction / 20f);

						Timer.SetTimeScale(1f);
					}
				}
			}

		}
	}

	public void Propel()
	{
		if(ControllerEx.GetInstance().KeyDown("MainAttack"))
		{
			_speed = .4f;

			if(weaponInven.mainCooldown == 0f && !_driveCheck)
			{
				weaponInven.MainAttack();

			}
		}
		else if(!ControllerEx.GetInstance().KeyPress("MainAttack"))
		{
			_speed = 0f;
		}
	}

	public void Look(float delta)
	{
		_controllPoint = ControllerEx.GetInstance().centerAxis;

		if(_rotateLock)
		{
			float controllAngle = MathEx.directionToAngle(_controllPoint);
			float curve = 1f;
			if(_speed != 0)
			{
				float f= Vector3.Dot(_direction,_controllPoint);
				if(f < -0.8f && cuttingCurve)
				{
					SetAdditionalSpeed(3f,1f,true);
					AddForce(_velocity * .5f);
					
					if(Timer.timeScale == 1f)
						Timer.SetTimeScaleTimer(0.3f,0.5f,true);


					_cam.Shake(0.05f, _direction / 20f);

					foreach(var ani in _boostAni)
        			{
						ani.ChangeAni("CuttingCurve",false);
        			}

					//EffectManager.GetInstance().AddEffect(_position + _direction * 0.25f,"SpriteSet/Effects/CuttingCurve").SetAngle(_eulerAngle);
					EffectManager.GetInstance().AddEffect(_position,_sprRenderer.sprite,0.2f).SetAngle(Mathf.LerpAngle(_eulerAngle,controllAngle,0.25f));
					_eulerAngle = Mathf.LerpAngle(_eulerAngle,controllAngle,0.9f);
					cuttingCurve = false;
					cuttingCurveTimer = 0.5f;
				}
				else
				{
					//_additionalSpeed = 0f;
				}
				// float sub = MathEx.abs(controllAngle - _eulerAngle);
				// if(sub >= 90f)
				// 	Debug.Log("tre");
			}

			_eulerAngle = MathEx.clamp360Degree(Mathf.LerpAngle(_eulerAngle,controllAngle,(_speed == 0f ? 10f : 5f) * curve * delta));
			_direction = MathEx.angleToDirection(_eulerAngle * Mathf.Deg2Rad);


			//_eulerAngle = MathEx.directionToAngle(_direction);

			if(_eulerAngle > 180f)
				_scale.y = -1f;
			else
				_scale.y = 1f;

			tp.localScale = _scale;
		}


	}

	public void HeightIconUpdate()
	{
		PlaceMapper map = ObjectManager.GetInstance()._place;

		float currHeight = _position.y;
		
		_miniMapHeightIcon.localPosition = CanvasScript.instance.
								CanvasPosToWorldPos(new Vector2(CanvasScript.instance.canvasWidth / 2f,
													CanvasScript.instance.canvasHeight * (currHeight / map._mapHeight)));
		//Vector2 pos = map.WorldPosToMapPos(position);
		//_angleCount.text = Mathf.Round(360f * map.GetPosPercentage(this).x).ToString();//Math.Truncate(pos.x * 10f) / 10f + " : " + Math.Truncate(pos.y * 10f) / 10f;
	}

}
