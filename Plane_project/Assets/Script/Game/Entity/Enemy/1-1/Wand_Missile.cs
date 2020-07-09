using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_Missile : PlaneBase
{
    Vector3 randD;
	bool random = false;
	bool act = false;
	float timer = 0f;

	public float actTime = 0f;

	float deleteTime = 0f;

    public override void firstSetting()
    {
        base.firstSetting();

        SetSpriteSet("SpriteSet/Planes/StarFish/starfish_missile",AnimationType.None);
		SetCollider(new Define.SimpleCircleCollider(.22f,.22f,_position));
		BoostSetUp("SpriteSet/Effects/Boost_New",new Vector2(-0.1f,0f));
		
		_boostAniProgress = true;

		_sprRenderer.flipX = true;

		_maxSpeed = Random.Range(3.8f,4.5f);
		_speed = .1f;

		_rotateLock = true;
    }

    public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(this);
	}

    public override void initialize()
    {
        BasicInitialize();
		
		SetNoClip(false);
		maxHp = _hp = 10;
		deleteTime = Random.Range(3.5f,4.5f);

		RegisteCollisionList();

    }

	public override ObjectBase SetPositionEm(Vector3 pos)
	{
		base.SetPositionEm(pos);
		float rat = Vector2.Distance(_position,CameraControll.instance.position);
		rat = rat < 2f ? 1f : rat;
		if(rat >= 2f) 
		{
			rat = 1f - (MathEx.abs((2f - rat)) * .1f);
		}
		SoundManager.instance.Play("SE/Missile_",false,4,rat);

		return this;
	}

    public override void progress(float deltaTime)
    {
        if(!act)
		{
			timer += deltaTime;

			if(timer >= actTime)
			{
				timer = 0f;
				act = false;
				_rotateLock = false;
			}
			else
			{
				BasicUpdate(deltaTime);
				return;
			}
		}

		deleteTime -= deltaTime;
		if(deleteTime <= 0f)
		{
			DecreaseHP(_hp + 10);
		}


		_direction = (Player.instance.position - _position).normalized;

		if(!random)
			_direction = (Player.instance.position - _position).normalized +
							randD.normalized * .1f;

		if(Vector3.Distance(_position,Player.instance.position) < 0.5f)
		{
			randD = (Player.instance.position - _position).normalized * Random.Range(1f,1.25f);;
			timer = Random.Range(2f,2.5f);
			random = true;
		}

		timer -= deltaTime;
		if(timer <= 0f)
		{
			randD = (_position - Player.instance.position).normalized;
			random = false;
			timer = Random.Range(0.5f,2f);
			randD = new Vector3(Random.Range(-2f,2f),Random.Range(-2f,2f));

			if(Vector3.Distance(_position,Player.instance.position) < 1.5f)
			{
				randD = (_position - Player.instance.position).normalized;
				timer = Random.Range(0.5f,2f);
				random = true;
			}
		}

		BulletManager.GetInstance().CollisionCheck(this,BulletType.player);
		BasicUpdate(deltaTime);
    }
}
