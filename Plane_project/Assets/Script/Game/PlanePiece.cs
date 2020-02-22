using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePiece : Drawable {

	private float _timer = 0f;
	private float _dead = 2f;

	public override void firstSetting()
	{
		base.firstSetting();
		_speed = 0.5f;
		//SetSprite("");
	}

	public override void initialize()
	{
		_timer = 0f;
		_dead = 2f;
	}

	public override void progress(float deltaTime)
	{
		_direction += new Vector3(0f,deltaTime * (Define.PhysicsSetting.gravity));

		Move(deltaTime);

		_timer -= deltaTime;
		_dead -= deltaTime;

		if(_timer <= 0f)
		{
			float s = Random.Range(.1f,1f);
			EffectManager.GetInstance().AddEffect(_position,"FireSmoke")
									.SetAngle(Random.Range(0f,360f))
									.SetScale(s,s,s);

			_timer = .03f;
		}

		if(_dead <= 0f)
			Delete();
	}
}
