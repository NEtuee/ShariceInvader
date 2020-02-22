using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMarker : Drawable
{
    static int count = 0;
    public override void firstSetting()
	{
		base.firstSetting();
		SetSprite(count.ToString());
		_speed = .2f;
        ++count;
		count = count == 3 ? 0 : count;
		SetSortingOrder(-1);
		//_maxSpeed = 6.2f;
	}

	public override void initialize()
	{
	}

	public override void progress(float deltaTime)
	{

	}
}
