using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEx : Singleton<ControllerEx>
{
    public enum ControllerType
    {
        KeyboardMouse,
        Gamepad
    };

    public enum KeyState
    {
        Down,
        Press,
        Up,
        None
    };

    public class Key
    {
        public KeyCode code;
        public KeyState state;

        public void ChangeState(KeyState s)
        {
            state = s;
        }

        public Key(KeyCode c)
        {
            code = c;
            state = KeyState.None;
        }
    }

    public Dictionary<string,Key> keyBindList = new Dictionary<string, Key>();
    public ControllerType controller = ControllerType.KeyboardMouse;
    public Vector2 centerAxis;
    public Camera mainView;

    public bool KeyDown(string key) {return KeyCheck(key) == KeyState.Down;}
    public bool KeyPress(string key) {return KeyCheck(key) == KeyState.Press;}
    public bool KeyUp(string key) {return KeyCheck(key) == KeyState.Up;}

    public KeyState KeyCheck(string key)
    {
        if(keyBindList.ContainsKey(key))
        {
            return keyBindList[key].state;
        }
        else
        {
            Debug.Log("Key does not exists");
            return KeyState.None;
        }
    }

    public void AddKey(string n, KeyCode k = KeyCode.None)
    {
        keyBindList.Add(n,new Key(k));
    }

    public void Setkey(string n, KeyCode k)
    {
        if(keyBindList.ContainsKey(n))
        {
            keyBindList[n].code = k;
        }
        else
        {
            Debug.Log("Key does not exists");
        }
    }

    public void CreateKeys()
    {
        if(keyBindList.Count != 0)
            return;

        AddKey("MainAttack",KeyCode.W);
        AddKey("DriveAttack",KeyCode.Mouse1);
        AddKey("WeaponChange",KeyCode.R);
        AddKey("Pause");
        AddKey("Select");
        AddKey("Cancel");
    }

    public void BindKeys()
    {

    }

    public void UpdateKeyState()
    {
        foreach(var key in keyBindList)
        {
            var k = key.Value;
            
            if(Input.GetKeyDown(k.code))
            {
                k.state = KeyState.Down;
            }
            else if(Input.GetKey(k.code))
            {
                k.state = KeyState.Press;
            }
            else if(Input.GetKeyUp(k.code))
            {
                k.state = KeyState.Up;
            }
            else if(k.state == KeyState.Up)
            {
                k.state = KeyState.None;
            }
        }

        centerAxis = controller == ControllerType.KeyboardMouse ? GetScreenCenterAxis() : GetJoystickAxis();
    }

    public Vector2 GetWorldScreenCenterAxis(Vector3 c)
    {
        Vector2 pos = Input.mousePosition;
        Vector2 center = mainView.WorldToScreenPoint(c);

        return (pos - center).normalized;
    }

    public Vector2 GetScreenCenterAxis()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 center = new Vector2(Screen.width / 2f,Screen.height / 2f);

        return (pos - center).normalized;
    }

    public float GetCenterDistance()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 center = new Vector2(Screen.width / 2f,Screen.height / 2f);

        return Vector2.Distance(pos,center);
    }

    public Vector2 GetJoystickAxis()
    {
        return new Vector2(Input.GetAxis("Vertical"),Input.GetAxis("Horizontal"));
    }

    public void SetMainViewCamera(Camera cam)
    {
        mainView = cam;
    }
}
