using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyShip : PlaneBase
{
    float timer = 0f;
    Vector3 dir;
	public override void firstSetting()
	{
		base.firstSetting();
		SetSpriteSet("SpriteSet/Planes/SupplyShip",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.4f,.4f,_position));

		_maxSpeed = 5f;
		_speed = .1f;
	}

	public override void initialize()
	{
		BasicInitialize();
		SetNoClip(true);
		maxHp = _hp = 1;

        timer = 1f;
        _bodyAttack = 0;
        _gravityScale = 0f;

        _direction = Vector2.down;

        dir = new Vector3(Player.instance.direction.x > 0 ? 1f : -1f,0f);

		RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);
	}

	public override void progress(float deltaTime)
	{
		BasicUpdate(deltaTime);

        _direction = Vector3.Lerp(dir,_direction,0.7f);

        if(timer >= 0f)
        {
            timer -= deltaTime;
            if(timer <= 0f)
            {
                SetNoClip(false);
                timer = 0f;
            }
        }
	}
}
