using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMain : SingletonMono<GameMain> {

	public GameObject player;
	public BackgroundManager background;
	public StageManager stage;
	public MainHud mainHud;
	public UISortManager optionScreen;

	public bool update = true;

	public EmptyObject emptyObject;

	public GameObject dialog_left;
    public SpriteRenderer dialog_leftSprite;
    public SpriteFontTextMesh dialog_leftName;
    public TextMesh dialog_leftDialog;

    public GameObject dialog_right;
    public SpriteRenderer dialog_rightSprite;
    public SpriteFontTextMesh dialog_rightName;
    public TextMesh dialog_rightDialog;
	
	public ResultUIControll result;

	private CameraControll cam;
	private ObjectManager _objManager;
	private BulletManager _bulletManager;
	private EffectManager _effectManager;
	private CollisionManager _collisionManager;
	private DelayActManager _delayActManager;

	private float _endTimer = 0f;
	private bool _end = false;

	Define.GizmoHelper _gizmoHelper = new Define.GizmoHelper();

	void Awake()
	{
		SetSingleton(this);

		ControllerEx.DeleteSingleton();
		ObjectManager.DeleteSingleton();
		EffectManager.DeleteSingleton();
		BulletManager.DeleteSingleton();
		CollisionManager.DeleteSingleton();
		DelayActManager.DeleteSingleton();

		OptionManager.GetInstance().LoadSettings();
		OptionManager.GetInstance().UpdateOptions();

		cam = Camera.main.GetComponent<CameraControll>();
		cam.firstSetting();
		
		_objManager = ObjectManager.GetInstance();
		_effectManager = EffectManager.GetInstance();
		_bulletManager = BulletManager.GetInstance();
		_collisionManager = CollisionManager.GetInstance();
		_delayActManager = DelayActManager.GetInstance();

		ControllerEx.GetInstance().CreateKeys();
		ControllerEx.GetInstance().SetMainViewCamera(GameObject.Find("MainScreenCamera").GetComponent<Camera>());
	}

	void Start ()
	{
		_objManager.firstSetting();
		_effectManager.firstSetting();
		_bulletManager.firstSetting();
		_collisionManager.firstSetting();
		background.firstSetting();
		_delayActManager.firstSetting();
		stage.firstSetting();

		ResultRecorder.GetInstance().Initialize();

		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Lancer/Burst");
		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Lancer/Loop");

		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Pulse/Burst");
		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Pulse/Loop");

		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/PS/Burst");
		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/PS/Loop");

		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/Attack");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/DriveOn");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/DriveEnd");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/Boost");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/Change");

		AnimationControllEx.LoadAnimation("UI/Weapon/PS/Change");

		AnimationControllEx.LoadAnimation("SpriteSet/Bullets/Ray");

		SetInGameDialog();

		_objManager._place.SetMainObject(emptyObject);
		
		// PlaneBase obj = _objManager.AddObject<Player>(Define.ObjectType.player,"Player");//_objManager.AddObject(Define.ObjectType.one,player);
		// obj.SetPositionEm(new Vector3(1f,5f));
		// cam.SetTarget(obj);

		//_objManager._place.SetMainObject(obj);

		//mainHud.Initiailize();
		FadeManager.instance.FadeIn(1f,1f);
	}

	void Update ()
	{
		float deltaTime = Timer.SetDeltaTime(Time.deltaTime);

		ControllerEx.GetInstance().UpdateKeyState();

		if(ControllerEx.GetInstance().KeyDown("Option") && !DialogManager.instance.dialog)
		{
			if(!optionScreen.gameObject.activeInHierarchy)
				optionScreen.Active();
		}

		if(!update)
			return;

		if(_end)
		{
			_endTimer += deltaTime;
			if(_endTimer >= 3f)
			{
				try
				{
					result.Active();
				}
				catch(Exception e)
				{
					Debugger.instance.AddDebugText(e.Message);
					Debugger.instance.AddDebugText(e.HelpLink);
					Debugger.instance.AddDebugText(e.StackTrace);
					Debugger.instance.AddDebugText(e.Source);
				}
				
			}
		}

		ResultRecorder.GetInstance().timer += Time.deltaTime;

		_objManager.UpdateTransform();
		mainHud.Progress(Timer.deltaTime);
		cam.SyncPosition();

		_effectManager.progress(deltaTime);
		_objManager.progress(deltaTime);
		_bulletManager.progress(deltaTime);
		background.progress(deltaTime);
		cam.progress(Timer.deltaTime);

		_delayActManager.progress(deltaTime);

		_collisionManager.UpdateCollisionList();
		_collisionManager.SyncCollisionList();

		if(!FadeManager.instance.IsFading())
			stage.progress(deltaTime);

		if(Player.instance != null && Player.instance.deleted && !_end)
		{
			FadeManager.instance.FadeOut(3f);
			_end = true;
		}

		_objManager.DeleteProgress();
		Physics2D.SyncTransforms();
		Timer.TimeScaleUpdate();

	}

	public void Restart()
	{
		SceneManager.LoadScene(1);
	}

	public void SetInGameDialog()
	{
		DialogManager.instance.SetLeftSideObjects(dialog_left,dialog_leftSprite,dialog_leftName,dialog_leftDialog);
		DialogManager.instance.SetRightSideObjects(dialog_right,dialog_rightSprite,dialog_rightName,dialog_rightDialog);
	}

	public void OnDrawGizmos()
	{
		if(_objManager != null)
		{
			int c = _objManager._place._placeCount;
			for(int i = 0; i < c; ++i)
			{
				_gizmoHelper.DrawLeftBottomCenterRect(
					_objManager._place._places[i].leftBottom,
					_objManager._place._placeWidth,_objManager._place._mapHeight + i);
			}
		}

	}

}
