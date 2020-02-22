using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : PlaneBase {

	ObjectBase _target;

	Vector3 randD;
	bool random = false;
	float timer = 0f;


	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("Enemy",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.11f,.11f,_position));

		_sprRenderer.color = new Color(.5f,.3f,.3f);

		_maxSpeed = Random.Range(3f,4f);
		_speed = .1f;
		//_maxSpeed = 6.2f;
	}

	public override void initialize()
	{
		BasicInitialize();
		
		randD = new Vector3(Random.Range(-2.5f,2.5f),Random.Range(-2.5f,2.5f));
		SetAbsoluteForce(randD);
		SetNoClip(false);
		_hp = 1;

		RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

	public override void progress(float deltaTime)
	{
		_direction = (GameManager.instance.player.position - _position).normalized;

		// float dist = Vector3.Distance(_position,GameManager.instance.player.position);
		// if(dist <= 2f)
		// 	_maxSpeed = Random.Range(10f,11f);
		// if(timer == 0f)
		// {
		// 	if(dist > 3f)
		// 	{
		// 		BurstActive();
		// 		timer = .5f;
		// 		_maxSpeed = 16f;
		// 	}
		// }
		// else
		// {
		// 	timer -= deltaTime;
		// 	if(timer <= 0f)
		// 	{
		// 		timer = 0f;
		// 	}
		// }

		if(!random)
			_direction = (GameManager.instance.player.position - _position).normalized +
							randD.normalized * .1f;

		if(Vector3.Distance(_position,GameManager.instance.player.position) < 0.5f)
		{
			randD = (GameManager.instance.player.position - _position).normalized * Random.Range(1f,1.25f);;
			timer = Random.Range(2f,2.5f);
			random = true;
		}

		timer -= deltaTime;
		if(timer <= 0f)
		{
			randD = (_position - GameManager.instance.player.position).normalized;
			random = false;
			timer = Random.Range(0.5f,4f);
			randD = new Vector3(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f));

			if(Vector3.Distance(_position,GameManager.instance.player.position) < 1.5f)
			{
				randD = (_position - GameManager.instance.player.position).normalized;
				timer = Random.Range(0.5f,2f);
				random = true;
			}
		}

		BulletManager.GetInstance().CollisionCheck(this,BulletType.player);


		BasicUpdate(deltaTime);
	}
}
