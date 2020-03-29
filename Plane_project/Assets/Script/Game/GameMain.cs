using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMain : MonoBehaviour {

	public GameObject player;
	public BackgroundManager background;

	private CameraControll cam;
	private ObjectManager _objManager;
	private BulletManager _bulletManager;
	private EffectManager _effectManager;
	private CollisionManager _collisionManager;
	private Timer timer;

	Define.GizmoHelper _gizmoHelper = new Define.GizmoHelper();

	void Awake()
	{
		ObjectManager.DeleteSingleton();
		EffectManager.DeleteSingleton();
		BulletManager.DeleteSingleton();
		CollisionManager.DeleteSingleton();
		// Timer.DeleteSingleton();

		cam = Camera.main.GetComponent<CameraControll>();
		cam.firstSetting();
		_objManager = ObjectManager.GetInstance();
		_effectManager = EffectManager.GetInstance();
		_bulletManager = BulletManager.GetInstance();
		_collisionManager = CollisionManager.GetInstance();

		ControllerEx.GetInstance().CreateKeys();


		GetComponent<GameManager>().firstSetting();
		timer = Timer.GetInstance();
	}

	void Start ()
	{
		_objManager.firstSetting();
		_effectManager.firstSetting();
		_bulletManager.firstSetting();
		_collisionManager.firstSetting();
		
		ObjectBase obj = _objManager.AddObject<Player>(Define.ObjectType.player,"Player").SetPosition(new Vector3(1f,5f));//_objManager.AddObject(Define.ObjectType.one,player);
		cam.SetTarget(obj.tp);
		GameManager.instance.player = obj.GetComponent<Player>();
		_objManager._place.SetMainObject(obj);

		background.firstSetting();

		_objManager.AddObject<TheMarker>(Define.ObjectType.enemy,"CCTV").SetPosition(_objManager._place.MapPosToWorldPos(new Vector3(0f,5f)));

		//EnemyCreator.ShootingDrone(20,new Vector3(0f,5f));

		//EnemyCreator.BoomDrone(100,new Vector2(2f,5f));


		EnemyCreator.CCTV(5,new Vector3(6f,5f));

		EnemyCreator.CCTV(5,new Vector3(8f,5f));
		EnemyCreator.CCTV(5,new Vector3(10f,5f));
		// EnemyCreator.CCTV(0,new Vector3(6f,5f));
		// EnemyCreator.CCTV(0,new Vector3(7f,5f));

		//EnemyCreator.CCTV(5,new Vector3(80f,5f));

		//_objManager.AddObject<CCTV>(Define.ObjectType.enemy,"CCTV").SetPosition(_objManager._place.MapPosToWorldPos(new Vector3(0f,5f)));
		//EnemyCreator.BoomDrone(50,new Vector3(0f,5f));
		//EnemyCreator.BoomDrone(50,new Vector3(3f,5f));

		//EnemyCreator.ShootingDrone(10,new Vector3(0f,5f));
	}

	void Update ()
	{
		float deltaTime = timer.SetDeltaTime(Time.deltaTime);
		ControllerEx.GetInstance().UpdateKeyState();
		
		if(Input.GetKeyDown(KeyCode.E))
		{
			//EffectManager.GetInstance().DrawBezierLine(Vector2.zero,new Vector2(0f,3f),new Vector2(1f,1f),new Vector2(-1f,2f),0.1f);
			_objManager.AddObject<MissileDrone>(Define.ObjectType.enemy,"MissileDrone").SetPosition(new Vector3(0f,5f));
			//EnemyCreator.BoomDrone(100,new Vector2(2f,5f));
		}

		_objManager.progress(deltaTime);
		PlayerFollower.instance.CC(deltaTime);
		follower.instance.CC(deltaTime);
		_bulletManager.progress(deltaTime);
		_effectManager.progress(deltaTime);

		_collisionManager.UpdateCollisionList();

		background.progress(deltaTime);

		cam.progress(timer.deltaTime);

		_collisionManager.SyncCollisionList();
		_objManager.DeleteProgress();

		Physics2D.SyncTransforms();

		timer.TimeScaleUpdate();

		if(Input.GetKeyDown(KeyCode.Escape))
			SceneManager.LoadScene(0);
	}

	void LateUpdate()
	{
		cam.SyncPosition();
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
