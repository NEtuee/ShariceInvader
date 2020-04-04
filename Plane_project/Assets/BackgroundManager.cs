using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour, Define.IManager
{
    public BackgroundScroller[] backgrounds;

    private PlaceMapper _place;

    public void firstSetting()
	{
		for(int i = 0; i < backgrounds.Length; ++i)
        {
            backgrounds[i].Init();
        }

        _place = ObjectManager.GetInstance()._place;
	}

	public void progress(float deltaTime)
	{
        if(_place.mainObject != null)
        {
            Vector2 percentage = _place.GetPosPercentage(CameraControll.instance.transform.position);
            for(int i = 0; i < backgrounds.Length; ++i)
            {
                backgrounds[i].ScreenScroll(percentage);
            }
        }
	}

	public void lateProgress(float deltaTime)
	{

	}
}
