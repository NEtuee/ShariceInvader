using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEx : Singleton<ControllerEx>
{
    enum ControllerType
    {
        KeyboardMouse,
        Gamepad
    };

    public Vector2 GetScreenCenterAxis()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 center = new Vector2(Screen.width,Screen.height);

        return (pos - center).normalized;
    }

    public Vector2 GetJoystickAxis()
    {
        return new Vector2(Input.GetAxis("Vertical"),Input.GetAxis("Horizontal"));
    }
}
