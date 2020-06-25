using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : SingletonMono<CanvasScript>, Define.IProgress {

	public static Queue<SpriteRenderer> minimapIcons = new Queue<SpriteRenderer>();

	public Vector2 canavsSize{get{return new Vector2(canvasWidth / pixelPerUnit,canvasHeight / pixelPerUnit);}}

	//[HideInInspector]
	public float canvasWidth = 1280f;
	//[HideInInspector]
	public float canvasHeight = 720f;
	[HideInInspector]
	public int pixelPerUnit = 100;

	public Camera mainCam{get{return _mainCam;}}

	private Define.GizmoHelper _gizmoHelper = new Define.GizmoHelper();
	private Camera _mainCam;

	private float worldWidth;
	private float worldHeight;

	public void Awake()
	{
		minimapIcons.Clear();
		CamSetting();
		SetSingleton(this); 
	}

	public void firstSetting()
	{
		CamSetting();

	}

	public void initialize()
	{

	}

	public void progress(float deltaTime)
	{

	}

	public void release()
	{

	}

	public SpriteRenderer GetMinimapIcon()
	{
		if(minimapIcons.Count == 0)
		{
			var miniMapIcon = new GameObject("Icon").transform;
			SpriteRenderer spr = miniMapIcon.gameObject.AddComponent<SpriteRenderer>();
			miniMapIcon.gameObject.layer = LayerMask.NameToLayer("UI");

			CanvasScript.instance.SetChild(miniMapIcon);

			return spr;
		}

		return minimapIcons.Dequeue();
	}

	public void ReturnIcon(SpriteRenderer spr) {spr.gameObject.SetActive(false); minimapIcons.Enqueue(spr);}

	public void SetChild(Transform t)
	{
		t.SetParent(transform);
	}

	public void CamSetting()
	{
		_mainCam = gameObject.GetComponent<Camera>();
		// _mainCam.clearFlags = CameraClearFlags.Nothing;
		// _mainCam.cullingMask = 1 << LayerMask.NameToLayer("UI");
		// _mainCam.orthographic = true;
		// _mainCam.orthographicSize = canvasHeight / (float)pixelPerUnit * 0.5f;
		// _mainCam.nearClipPlane = 0f;

		// Vector3 pos = transform.position;
		// pos.z = -10f;
		// transform.position = pos;

		worldWidth = canvasWidth / pixelPerUnit;
		worldHeight = canvasHeight / pixelPerUnit;

//		gameObject.AddComponent<GUILayer>();
		GetComponent<TouchDetection>().baseCam = _mainCam;
	}

	public Vector2 CanvasPosToWorldPos(Vector2 p)
	{
		float x = p.x / canvasWidth;
		float y = p.y / canvasHeight;

		return new Vector2((worldWidth * x) - (worldWidth / 2f), 
								(worldHeight * y) - (worldHeight / 2f));
	}

	public void OnDrawGizmos()
	{
		_gizmoHelper.DrawRect(transform.position,canvasWidth / pixelPerUnit * 0.5f,canvasHeight / pixelPerUnit * 0.5f);
	}
}
