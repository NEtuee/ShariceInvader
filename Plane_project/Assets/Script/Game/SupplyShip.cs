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
		SetSpriteSet("SupplyShip",AnimationType.Horizontal);
		SetCollider(new Define.SimpleCircleCollider(.4f,.4f,_position));

		_maxSpeed = 5f;
		_speed = .1f;
	}

	public override void initialize()
	{
		BasicInitialize();
		SetNoClip(true);
		_hp = 1;

        timer = 1f;
        _bodyAttack = 0;
        _gravityScale = 0f;
        _trail.transform.position = new Vector3(-0.5f,0f);
        _trail.startWidth = .05f;
		_trail.endWidth = .0025f;

        _direction = Vector2.down;

        dir = new Vector3(GameManager.instance.player.direction.x > 0 ? 1f : -1f,0f);

		RegisteCollisionList();
	}

	public override void deleteEvent()
	{
		base.deleteEvent();
		ComboCount.instance.AddComboCount(1);

        GameManager.instance.player.WeaponChange(new Weapon_Lancer(null));
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
