using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEx : Singleton<ControllerEx>
{
    public enum ControllerType
    {
        KeyboardMouse,
        XboxController,
        PSController
    };

    public enum KeyType
    {
        Button,
        Axis,
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
        public KeyType type;
        public KeyCode code;
        public KeyState state;

        public string axisName;
        public float axis;


        public void ChangeState(KeyState s)
        {
            state = s;
        }

        public Key(KeyCode c)
        {
            type = KeyType.Button;
            code = c;
            state = KeyState.None;
        }

        public Key(string c)
        {
            type = KeyType.Axis;
            axisName = c;
            axis = 0f;
            state = KeyState.None;
        }
    }

    public Dictionary<string,Key> keyBindList;// = new Dictionary<string, Key>();
    public ControllerType controller = ControllerType.KeyboardMouse;
    public Vector2 centerAxis;
    public Camera mainView;

    private Dictionary<string,Key> _keyboardBind = new Dictionary<string, Key>();
    private Dictionary<string,Key> _xboxBind = new Dictionary<string, Key>();
    private Dictionary<string,Key> _psBind = new Dictionary<string, Key>();

    private string _controllerLists = "";

    private float _axisPushStr = 0.4f;
    private float _deviceCheckTimer = 1f;

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

    // public void AddKey(string n, KeyCode k = KeyCode.None, KeyType type = KeyType.Button)
    // {
    //     keyBindList.Add(n,new Key(k,type));
    // }

    // public void Setkey(string n, KeyCode k, KeyType t)
    // {
    //     if(keyBindList.ContainsKey(n))
    //     {
    //         keyBindList[n].code = k;
    //         keyBindList[n].type = t;
    //     }
    //     else
    //     {
    //         Debug.Log("Key does not exists");
    //     }
    // }

    public void CreateKeyBindDic()
    {
        _keyboardBind.Add("MainAttack",new Key(KeyCode.W));
        _keyboardBind.Add("DriveAttack",new Key(KeyCode.Mouse1));
        _keyboardBind.Add("WeaponChange",new Key(KeyCode.R));
        _keyboardBind.Add("Pause",new Key(KeyCode.Escape));
        _keyboardBind.Add("Cancel",new Key(KeyCode.Tab));

        _xboxBind.Add("MainAttack",new Key(KeyCode.JoystickButton0));
        _xboxBind.Add("DriveAttack",new Key("XBoxRightTrigger"));
        _xboxBind.Add("WeaponChange",new Key(KeyCode.JoystickButton2));
        _xboxBind.Add("Pause",new Key(KeyCode.JoystickButton7));
        _xboxBind.Add("Cancel",new Key(KeyCode.JoystickButton1));

        _psBind.Add("MainAttack",new Key(KeyCode.JoystickButton1));
        _psBind.Add("DriveAttack",new Key("PSRightTrigger"));
        _psBind.Add("WeaponChange",new Key(KeyCode.JoystickButton0));
        _psBind.Add("Pause",new Key(KeyCode.JoystickButton9));
        _psBind.Add("Cancel",new Key(KeyCode.JoystickButton2));

    }

    public void CreateKeys()
    {
        // if(keyBindList.Count != 0)
        //     return;

        CreateKeyBindDic();
        InputDeviceCheck();
        InputDevieKeyBind();


        // AddKey("MainAttack",KeyCode.W);
        // AddKey("DriveAttack",KeyCode.Mouse1);
        // AddKey("WeaponChange",KeyCode.R);
        // AddKey("Pause");
        // AddKey("Select");
        // AddKey("Cancel");
    }

    public void InputDevieKeyBind()
    {
        switch(controller)
        {
            case ControllerType.KeyboardMouse:
            keyBindList = _keyboardBind;
            break;
            case ControllerType.XboxController:
            keyBindList = _xboxBind;
            break;
            case ControllerType.PSController:
            keyBindList = _psBind;
            break;
        }
    }

    public void CheckCurrentInputDevice()
    {
        if(Input.anyKey)
        {
            bool joy = false;
            for(int i = 0; i < 14; ++i)
            {
                if(Input.GetKey(KeyCode.JoystickButton0 + i))
                {
                    joy = true;
                    break;
                }
            }

            if(controller != ControllerType.KeyboardMouse && !joy)
            {
                controller = ControllerType.KeyboardMouse;
                InputDevieKeyBind();
            }
            else if(controller == ControllerType.KeyboardMouse && joy)
            {
                _controllerLists = "";
                InputDeviceCheck();
                InputDevieKeyBind();
            }
            
        }
    }

    public void InputDeviceCheck()
    {
        string[] con = Input.GetJoystickNames();
        if(con != null && con[0] != _controllerLists)
        {
            ControllerType type = ControllerType.KeyboardMouse;

            if(con[0].Contains("Xbox"))
                type = ControllerType.XboxController;
            else// if(con[0].Contains("Controller"))
                type = ControllerType.PSController;

            _controllerLists = con[0];

            controller = type;
        }
    }

    public void BindKeys(string n, KeyCode k)
    {
        keyBindList[n].code = k;
        keyBindList[n].state = KeyState.None;
    }

    public void UpdateKeyState()
    {
        _deviceCheckTimer -= Time.deltaTime;
        if(_deviceCheckTimer <= 0f)
        {
            var con = controller;
            InputDeviceCheck();
            if(con != controller)
                InputDevieKeyBind();
            
            _deviceCheckTimer = 1f;
        }

        CheckCurrentInputDevice();
        Debugger.instance.SetDebugText(controller.ToString());

        foreach(var key in keyBindList)
        {
            var k = key.Value;
            
            if(k.type == KeyType.Button)
            {
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
                else if(!Input.GetKey(k.code) && (k.state == KeyState.Press || k.state == KeyState.Down))
                {
                    k.state = KeyState.Up;
                }
                else if(k.state == KeyState.Up)
                {
                    k.state = KeyState.None;
                }
            }
            else if(k.type == KeyType.Axis)
            {
                k.axis = Input.GetAxis(k.axisName);
                bool down = k.axis >= _axisPushStr;

                if(down && k.state == KeyState.Down)
                    k.state = KeyState.Press;
                else if(down && k.state != KeyState.Press)
                    k.state = KeyState.Down;
                else if(!down && (k.state == KeyState.Press || k.state == KeyState.Down))
                    k.state = KeyState.Up;
                else if(k.state == KeyState.Up)
                    k.state = KeyState.None;
            }
        }

        

        centerAxis = controller == ControllerType.KeyboardMouse ? GetScreenCenterAxis() : GetJoystickAxis();

        //Debugger.instance.SetDebugText(centerAxis.ToString());
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
        var axis = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        if(MathEx.abs(axis.magnitude) >= _axisPushStr)
            return axis.normalized;
        else
            return centerAxis;
    }

    public void SetMainViewCamera(Camera cam)
    {
        mainView = cam;
    }
}
