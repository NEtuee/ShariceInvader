using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player : PlaneBase {

	public Vector3 target = new Vector3(0f,0f,0f);

	private float _timer = 0f;
	private CameraControll _cam;
	private Vector3 _controllPoint;
	private Vector3 _dodgeStartPos;
	private Transform[] _checker = new Transform[3];

	private Transform _miniMapHeightIcon;
	private TextMesh _angleCount;

	int a = 0;

	bool isnone = false;
	bool cuttingCurve = true;
	float cuttingCurveTimer = 0f;

	int weaponGague = 12;

	public override void firstSetting()
	{
		base.firstSetting();

		SetSpriteSet("Player_New",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.05f,.05f,_position));

		for(int i = 0; i < 3; ++i)
		{
			
			_checker[i] = new GameObject("checker"+i).transform;
			_checker[i].gameObject.AddComponent<SpriteRenderer>().sprite = ResourceManager.GetInstance().GetSprite("circle_" + i);
		}


		_cam = CameraControll.instance;

		_speed = 0f;
		_maxSpeed = 2.8f;

		_bodyAttack = 5;
		_dodgeDist = 3f;
		_hp = 10;
		_gravityScale = 0.3f;

		_timer = 3f;

		miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sprite = ResourceManager.GetInstance().GetSprite("PlayerMinimapIcon");
		miniMapIcon.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

		_miniMapHeightIcon = new GameObject(name + " HeightIcon").transform;
		SpriteRenderer spr = _miniMapHeightIcon.gameObject.AddComponent<SpriteRenderer>();
		spr.sprite = ResourceManager.GetInstance().GetSprite("UI/MiniMapHeightIcon");
		_miniMapHeightIcon.gameObject.layer = LayerMask.NameToLayer("UI");

		CanvasScript.instance.SetChild(_miniMapHeightIcon);

		_angleCount = CanvasScript.instance.gameObject.transform.Find("AngleCount").GetComponent<TextMesh>();
		HeightIconUpdate();
		
		WeaponChange(new Weapon_PhantomStinger(this));
	}

	public override void initialize()
	{
		BasicInitialize();
		
		_burst = true;
		_rotateLock = true;
		_velocityFlip = false;
		_noclip = true;
		//_immortal = true;

		_controllPoint = _direction;

		RegisteCollisionList();
	}

	public override void progress(float deltaTime)
	{
		mainWeapon.Progress(deltaTime);
		
		if(!_controllLock)
		{
			if(!mainWeapon.mainAttack)
			{
				Propel();
				DodgeCheck();
				Look(deltaTime);
			}
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			if(a == 0)
			{
				WeaponChange(new Weapon_Lancer(this));
				a = 1;
			}
			else if(a == 1)
			{
				WeaponChange(new Weapon_Test(this));
				a = 2;
			}
			else if(a == 2)
			{
				WeaponChange(new Weapon_PhantomStinger(this));
				a = 0;
			}
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			_controllLock = !_controllLock;
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

		_checker[0].position = _position + _direction;
		_checker[1].position = _position + _controllPoint;
		_checker[2].position = _position + _velocity.normalized;


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


		BulletManager.GetInstance().CollisionCheck(this,BulletType.enemy);
		HeightIconUpdate();
	}

	public override void WhenDecreaseHP()
	{
		_cam.Zoom(1.5f);
		_cam.Glitch(0.3f);
		Timer.GetInstance().SetTimeScaleTimer(0f,0.1f);
		Debug.Log("HIT");
	}

	public override void WeaponChange(WeaponBase weapon)
	{
		base.WeaponChange(weapon);
		weaponGague = 12;
		isnone = false;
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		Timer.GetInstance().SetTimeScale(1f);
	}

	public override void BurstActive(bool effectActive)
	{
		
		base.BurstActive(false);
		_cam.Shake(0.2f, _direction / 20f);
	}

	public void DodgeCheck()
	{
		if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			Timer.GetInstance().SetTimeScale(0.1f);
			
			Vector3 mouse =_cam.ScreenToWorldMouse();
			mouse = (mouse - _position).normalized;
			if(mainWeapon.immedyActiveSpecAttack)
			{
				mainWeapon.SpecialAttack(mouse);
			}
		}
		else if(Input.GetKeyUp(KeyCode.Mouse1))
		{
			Vector3 mouse =_cam.ScreenToWorldMouse();
			mouse = (mouse - _position).normalized;
			if(!mainWeapon.immedyActiveSpecAttack)
			{
				if(mainWeapon.SpecialAttack(mouse))
				{

					//_dodgeStartPos = _position;

					Dodge(mouse,false);
					_dodge = true;
					//_dodgeAttack = true;

					_cam.Shake(0.2f, _direction / 20f);

					// if(!isnone)
					// 	weaponGague -= 3;

					MainHud.instance.UpdateGague((float)weaponGague / 12f);

					if(weaponGague <= 0)
					{
						WeaponChange(new Weapon_None(this));
						isnone = true;
						weaponGague = 0;

						MainHud.instance.UpdateGague(0f);
					}
					Timer.GetInstance().SetTimeScale(1f);
				}
			}

		}
	}

	public void Propel()
	{
		bool keyCheck = Input.GetKeyDown(KeyCode.W);
		if(keyCheck)
		{
	//		BurstActive();
			_speed = .4f;

			if(mainWeapon.mainCoolDown == 0f)
			{
				// if(Input.GetKey(KeyCode.Mouse0))
				// 	_aim = true;
				//_burst = true;
				mainWeapon.MainAttack();

				// if(!isnone)
				// 	weaponGague -= 1;

				MainHud.instance.UpdateGague((float)weaponGague / 12f);

				if(weaponGague <= 0)
				{
					WeaponChange(new Weapon_None(this));
					isnone = true;
					weaponGague = 0;

					MainHud.instance.UpdateGague(0f);
				}
			}
		}
		else if(!Input.GetKey(KeyCode.W))
		{
			_speed = 0f;
		}
	}

	public void Look(float delta)
	{
		target = _cam.ScreenToWorldMouse();
		if(target.magnitude != 0)
		{
			_controllPoint = (target - _position).normalized;//target.normalized;

			//_direction = Vector3.Lerp(_direction,_controllPoint,(_speed == 0f ? 18f : 5f) * delta).normalized;

			if(_rotateLock)
			{
				float controllAngle = MathEx.directionToAngle(_controllPoint);
				float curve = 1f;
				if(_speed != 0)
				{
					float f= Vector3.Dot(_direction,_controllPoint);
					if(f < -0.8f && cuttingCurve)
					{
						AddForce(-_velocity * 0.65f);
						
						Timer.GetInstance().SetTimeScaleTimer(0.3f,0.5f,true);
						_cam.Shake(0.05f, _direction / 20f);
						EffectManager.GetInstance().AddEffect(_position + _direction * 0.25f,"CuttingCurve").SetAngle(_eulerAngle);
						EffectManager.GetInstance().AddEffect(_position,_sprRenderer.sprite,0.2f).SetAngle(Mathf.LerpAngle(_eulerAngle,controllAngle,0.25f));
						_eulerAngle = Mathf.LerpAngle(_eulerAngle,controllAngle,0.5f);
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

				if(_direction.y < 0f)
					_scale.y = -1f;
				else if(_direction.y > 0f)
					_scale.y = 1f;
			}

		}
	}

	public void HeightIconUpdate()
	{
		PlaceMapper map = ObjectManager.GetInstance()._place;

		float currHeight = _position.y;
		
		_miniMapHeightIcon.localPosition = CanvasScript.instance.
								CanvasPosToWorldPos(new Vector2(CanvasScript.instance.canvasWidth / 2f,
													CanvasScript.instance.canvasHeight * (currHeight / map._mapHeight)));
		Vector2 pos = map.WorldPosToMapPos(position);
		_angleCount.text = Mathf.Round(360f * map.GetPosPercentage(this).x).ToString();//Math.Truncate(pos.x * 10f) / 10f + " : " + Math.Truncate(pos.y * 10f) / 10f;
	}

}
