// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PhysicsTest : MonoBehaviour {

// 	public Vector2 velocity = new Vector2();
// 	public Vector2 friction = new Vector2();

// 	public Collider2D[] wall;

// 	public GameObject obj;

// 	public bool bounding = false;
// 	public float gravityScale = 1f;

// 	public LineRenderer velLine;
// 	public LineRenderer fricLine;


// 	float gravity = -0.098f;
// 	float mass = 1f;
// 	float frictionFactor = 0.02f;
// 	float dragFactor = 0.1f;

// 	Collider2D coll;

// 	public void Start()
// 	{
// 		coll = GetComponent<Collider2D>();
// 		CreateFloor();
// 		Application.targetFrameRate = 20;
// 	}

// 	float Cross(Vector2 a, Vector2 b)
// 	{
// 		return a.x * b.y - a.y * b.x;
// 	}

// 	void Update () {

// 		//ApplyForce(Vector2.right * Time.deltaTime);

// 		KeyInputManager.KeyUpdate();

// 		if(KeyInputManager.KeyPressed("LEFT") && velocity.y == 0f)
// 		{
// 			AddForce(Vector2.left * 2);

// 			if(KeyInputManager.KeyPressed("UP"))
// 			{
// 				Debug.Log("check");
// 				AddForce(Vector2.up * 5f);
// 			}
// 			else
// 				AddForce(Vector2.up * 2f);
// 		}
// 		if(KeyInputManager.KeyPressed("RIGHT") && velocity.y == 0f)
// 		{
// 			AddForce(Vector2.right * 2);

// 			if(KeyInputManager.KeyPressed("UP"))
// 			{
// 				AddForce(Vector2.up * 5f);
// 			}
// 			else
// 				AddForce(Vector2.up * 2f);
// 		}


// 		if(KeyInputManager.KeyPressed("UP") && velocity.y == 0f)
// 		{
// 			velocity.y = 0f;
// 			AddForce(Vector2.up * 5f);
// 		}
// 		if(KeyInputManager.KeyPressed("Down"))
// 		{
// 			AddForce(Vector2.down * .1f);
// 		}

// 		velocity.x = velocity.x > 5f ? 5f : velocity.x;
// 		velocity.x = velocity.x < -5f ? -5f : velocity.x;

// 		FrictionCheck();
// 		GravityCheck();

// 		int point = (int)(transform.position.x / 0.32f);
// 		if(point >= 0 && point < wall.Length)
// 			Collision(wall[point].bounds);

// 		if(MathEx.Vector2Compare(friction,velocity) == 1)
// 		{
// 			friction = velocity = Vector2.zero;
// 		}

// 		Vector2 pos = transform.position;
// 		pos += velocity * Time.deltaTime;
// 		transform.position = pos;

// 		Physics2D.SyncTransforms();

// 		Vector3 cam = Camera.main.transform.position;
// 		cam = Vector2.Lerp(cam, transform.position, 0.2f);
// 		cam.z = -10f;
// 		Camera.main.transform.position = cam;

// 		velLine.SetPosition(0,transform.position);
// 		fricLine.SetPosition(0,transform.position);
// 		velLine.SetPosition(1,transform.position + (Vector3)(velocity * 0.5f));
// 		fricLine.SetPosition(1,transform.position + (Vector3)(friction * 30f));

// 		Physics2D.SyncTransforms();

// 		KeyInputManager.KeyInit();
// 	}

// 	void OnDrawGizmosSelected()
// 	{
// 		if(coll != null)
// 			GizmoHelper.DrawBounds(coll.bounds,Color.red);
// 	}

// 	public void AddForce(Vector2 f)
// 	{
// 		velocity += f;
// 		MathEx.nearZero(ref velocity);
// 		//GetComponent<Rigidbody2D>().AddForce(f * 0.1f);
// 	}

// 	public void AddAbsolForce(Vector2 f)
// 	{
// 		velocity = f;
// 		MathEx.nearZero(ref velocity);
// 	}

// 	public void ApplyForce(Vector2 f)
// 	{
// 		AddForce(f / mass);
// 	}

// 	public void FrictionCheck()
// 	{
// 		friction = -velocity.normalized * (frictionFactor);
// 		// if(MathEx.Vector2Compare(friction,velocity) == 1)
// 		// {
// 		// 	friction = -velocity;
// 		// }
		
// 		AddForce(friction);
// 	}

// 	public void CreateFloor()
// 	{
// 		List<Collider2D> colist = new List<Collider2D>();
// 		for(int i = 0; i < 300; ++i)
// 		{
// 			colist.Add(Instantiate(obj,new Vector3(i * 0.32f,0f,0f),Quaternion.identity).GetComponent<Collider2D>());
// 		}

// 		wall = colist.ToArray();
// 	}

// 	public void GravityCheck()
// 	{
// 		AddForce(new Vector2(0f,gravity * mass * gravityScale));
// 	}

// 	public void Collision(Bounds bounds)
// 	{
// 		if(coll.bounds.Intersects(bounds))
// 		{
// 			float x = Mathf.Max(coll.bounds.min.x,bounds.min.x);
// 			float y = Mathf.Min(coll.bounds.max.y, bounds.max.y);
// 			float w = Mathf.Min(coll.bounds.max.x, bounds.max.x);
// 			float h = Mathf.Max(coll.bounds.min.y,bounds.min.y);

// 			if(MathEx.abs(w - x) > MathEx.abs(y - h))
// 			{
// 				if(h == coll.bounds.min.y)
// 				{
// 					Vector2 p = transform.position;
// 					p.y += MathEx.abs(y - h);
// 					transform.position = p;
// 					if(!bounding)
// 					{
// 						if(velocity.y < 0)
// 							AddForce(new Vector3(0f,-velocity.y));
// 					}
// 					else
// 					{
// 						if(velocity.y < 0)
// 							velocity.y = -velocity.y * 0.2f;
// 					}
// 				}
// 				else
// 				{
// 					Vector2 p = transform.position;
// 					p.y -= MathEx.abs(y - h);
// 					transform.position = p;
// 					if(!bounding)
// 						if(velocity.y > 0)
// 							AddForce(new Vector3(0f,-velocity.y));
// 					else
// 					{
// 						if(velocity.y > 0)
// 							velocity.y = -velocity.y;
// 					}
// 				}
// 			}
// 			else
// 			{
// 				if(x == coll.bounds.min.x)
// 				{
// 					Vector2 p = transform.position;
// 					p.x += MathEx.abs(w - x);
// 					transform.position = p;
// 					if(!bounding)
// 						if(velocity.x < 0)
// 							AddForce(new Vector3(0f,-velocity.x));
// 					else
// 					{
// 						if(velocity.x < 0)
// 							velocity.x = -velocity.x;
// 					}
// 				}
// 				else
// 				{
// 					Vector2 p = transform.position;
// 					p.x -= MathEx.abs(w - x);
// 					transform.position = p;
// 					if(!bounding)
// 						if(velocity.x > 0)
// 							AddForce(new Vector3(0f,-velocity.x));
// 					else
// 					{
// 						if(velocity.x > 0)
// 							velocity.x = -velocity.x;
// 					}
// 				}
// 			}
// 			friction += -velocity.normalized * (mass * dragFactor);
// 			AddForce(-velocity.normalized * (mass * dragFactor));
// 			Physics2D.SyncTransforms();
// 		}
// 	}
// }
