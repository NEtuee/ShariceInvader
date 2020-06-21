using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMain : MonoBehaviour {

	public GameObject player;
	public BackgroundManager background;
	public StageManager stage;
	public MainHud mainHud;

	private CameraControll cam;
	private ObjectManager _objManager;
	private BulletManager _bulletManager;
	private EffectManager _effectManager;
	private CollisionManager _collisionManager;
	private DelayActManager _delayActManager;

	Define.GizmoHelper _gizmoHelper = new Define.GizmoHelper();

	void Awake()
	{
		ObjectManager.DeleteSingleton();
		EffectManager.DeleteSingleton();
		BulletManager.DeleteSingleton();
		CollisionManager.DeleteSingleton();
		DelayActManager.DeleteSingleton();

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

		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Lancer/Burst");
		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Lancer/Loop");

		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Pulse/Burst");
		AnimationControllEx.LoadAnimation("SpriteSet/Effects/Weapon/Pulse/Loop");

		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/Attack");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/DriveOn");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/DriveEnd");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/Boost");
		AnimationControllEx.LoadAnimation("UI/Weapon/Pulse/Change");

		AnimationControllEx.LoadAnimation("UI/Weapon/PS/Change");

		AnimationControllEx.LoadAnimation("SpriteSet/Bullets/Ray");

		
		PlaneBase obj = _objManager.AddObject<Player>(Define.ObjectType.player,"Player");//_objManager.AddObject(Define.ObjectType.one,player);
		obj.SetPositionEm(new Vector3(1f,5f));
		cam.SetTarget(obj);

		_objManager._place.SetMainObject(obj);

		mainHud.Initiailize();
	}

	void Update ()
	{
		float deltaTime = Timer.SetDeltaTime(Time.deltaTime);

		ControllerEx.GetInstance().UpdateKeyState();

		_objManager.UpdateTransform();
		mainHud.Progress(Timer.deltaTime);
		cam.SyncPosition();

		_objManager.progress(deltaTime);
		_bulletManager.progress(deltaTime);
		_effectManager.progress(deltaTime);
		background.progress(deltaTime);
		cam.progress(Timer.deltaTime);

		_delayActManager.progress(deltaTime);

		_collisionManager.UpdateCollisionList();
		_collisionManager.SyncCollisionList();

		stage.progress(deltaTime);

		_objManager.DeleteProgress();
		Physics2D.SyncTransforms();
		Timer.TimeScaleUpdate();

		if(Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(0);
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
