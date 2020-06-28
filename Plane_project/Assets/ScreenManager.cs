using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : Singleton<ScreenManager>
{
    public static Vector2Int[] screenResolution = {
        new Vector2Int(800,600),
        new Vector2Int(960,720),
        new Vector2Int(1440,1080),
        new Vector2Int(1600,1200),

    };

    public Vector2Int screenSize;

    private int screenPos;

    public void SetScreenResolution(int pos)
    {
        screenPos = pos;
        screenSize = screenResolution[pos];

        Screen.SetResolution(screenSize.x,screenSize.y,false);
    }
}
