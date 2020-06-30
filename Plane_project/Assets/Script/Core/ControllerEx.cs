using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEx : Singleton<ControllerEx>
{
    public enum ControllerType
    {
        KeyboardMouse = 0,
        XboxController,
        PSController
    };

    public enum KeyType
    {
        Button = 0,
        Axis,
        TwoSideAxisButton,
    };

    public enum KeyState
    {
        Down = 0,
        Press,
        Up,
        None
    };

    public class Key
    {
        public KeyType type;
        public KeyCode code;
        public KeyState state;

        public string axisName = "";
        public float axis = 0f;

        public bool side = false;

        public void ChangeState(KeyState s)
        {
            state = s;
        }

        public Key(){}

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

        public Key(string an, bool s) // false == left
        {
            type = KeyType.TwoSideAxisButton;
            axisName = an;
            side = s;
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
    private Dictionary<string,bool> _axisButtonCheck = new Dictionary<string, bool>();

    private Sprite[] _xboxKeys;
    private Sprite[] _psKeys;

    private string _controllerLists = "";

    private float _axisPushStr = 0.4f;
    private float _deviceCheckTimer = 1f;
    private static readonly KeyCode[] keyCodes = System.Enum.GetValues(typeof(KeyCode))
                                                 .Cast<KeyCode>()
                                                 .Where(k => ((int)k < (int)KeyCode.Mouse2))
                                                 .ToArray();

    public bool KeyDown(string key) {return KeyCheck(key) == KeyState.Down;}
    public bool KeyPress(string key) {return KeyCheck(key) == KeyState.Press;}
    public bool KeyUp(string key) {return KeyCheck(key) == KeyState.Up;}

    public bool _keyBreak = false;

    public KeyState KeyCheck(string key)
    {
        if(keyBindList.ContainsKey(key))
        {
            return keyBindList[key].state;
        }
        else
        {
            //Debug.Log("Key does not exists");
            return KeyState.None;
        }
    }

    public void LoadControllerSprites()
    {
        _xboxKeys = ResourceManager.GetInstance().GetSpriteSet("UI/Keys/xbox");
        _psKeys = ResourceManager.GetInstance().GetSpriteSet("UI/Keys/ps");
    }

    public void CreateKeyBindDic()
    {
        _keyboardBind.Add("MainAttack",new Key(KeyCode.W));
        _keyboardBind.Add("DriveAttack",new Key(KeyCode.Mouse1));
        _keyboardBind.Add("WeaponChange",new Key(KeyCode.R));
        _keyboardBind.Add("Cancel",new Key(KeyCode.Tab));
        _keyboardBind.Add("Left",new Key(KeyCode.LeftArrow));
        _keyboardBind.Add("Right",new Key(KeyCode.RightArrow));
        _keyboardBind.Add("Up",new Key(KeyCode.UpArrow));
        _keyboardBind.Add("Down",new Key(KeyCode.DownArrow));
        _keyboardBind.Add("Option",new Key(KeyCode.Escape));

        _xboxBind.Add("MainAttack",new Key(KeyCode.JoystickButton0));
        _xboxBind.Add("DriveAttack",new Key("XBoxRightTrigger"));
        _xboxBind.Add("WeaponChange",new Key(KeyCode.JoystickButton2));
        _xboxBind.Add("Cancel",new Key(KeyCode.JoystickButton1));

        _psBind.Add("MainAttack",new Key(KeyCode.JoystickButton1));
        _psBind.Add("DriveAttack",new Key("PSRightTrigger"));
        _psBind.Add("WeaponChange",new Key(KeyCode.JoystickButton0));
        _psBind.Add("Cancel",new Key(KeyCode.JoystickButton2));
        
        SaveKeyBindInfo();
    }

    public void LoadKeyBindInfo()
    {
        var data = IOManager.ReadiniFile("keyMapping.ini");
        if(data == null)
        {
            CreateKeyBindDic();
            return;
        }

        foreach(var block in data)
        {
            ControllerType type = block.Key == "km" ? ControllerType.KeyboardMouse : 
                                (block.Key == "xbox" ? ControllerType.XboxController : 
                                (block.Key == "ps" ? ControllerType.PSController : ControllerType.PSController));
            if(block.Value == null)
                continue;
            
            foreach(var keyData in block.Value)
            {
                Key key = new Key();
                var property = keyData.data.Split(',');
                var keyType = (KeyType)int.Parse(property[0]);
                key.type = keyType;

                if(keyType == KeyType.Button)
                {
                    key.code = (KeyCode)int.Parse(property[1]);
                }
                else if(keyType == KeyType.Axis)
                {
                    key.axisName = property[2];
                }
                else if(keyType == KeyType.TwoSideAxisButton)
                {
                    key.axisName = property[2];
                    key.side = property[3] == "0";
                }

                BindLoadedKey(keyData.title,type,key);
            }
        }

        data = null;
    }

    public void SaveKeyBindInfo()
    {
        List<string> keys = new List<string>();
        keys.Add("[km]");
        foreach(var key in _keyboardBind)
        {
            keys.Add(key.Key + "=" + (int)key.Value.type + "," +
                                    (int)key.Value.code + "," +
                                    key.Value.axisName + "," + 
                                    (key.Value.side ? "0" : "1"));
        }
        keys.Add("[xbox]");
        foreach(var key in _xboxBind)
        {
            keys.Add(key.Key + "=" + (int)key.Value.type + "," +
                                    (int)key.Value.code + "," +
                                    key.Value.axisName + "," + 
                                    (key.Value.side ? "0" : "1"));
        }
        keys.Add("[ps]");
        foreach(var key in _psBind)
        {
            keys.Add(key.Key + "=" + (int)key.Value.type + "," +
                                    (int)key.Value.code + "," +
                                    key.Value.axisName + "," + 
                                    (key.Value.side ? "0" : "1"));
        }

        Debug.Log("Save");
        IOManager.WriteStringToFile_NoMark(keys.ToArray(),"keyMapping.ini");
    }

    public void BindLoadedKey(string keyName,ControllerType ct, Key key)
    {
        if(ct == ControllerType.KeyboardMouse)
        {
            _keyboardBind[keyName] = key;
        }
        else if((ct == ControllerType.XboxController))
        {
            _xboxBind[keyName] = key;
        }
        else if((ct == ControllerType.PSController))
        {
            _psBind[keyName] = key;
        }
    }

    public void DeleteBindInfo(Key key, ControllerType con)
    {
        string n = "";
        foreach(var item in keyBindList)
        {
            if(item.Value.axisName == key.axisName && item.Value.code == key.code && item.Value.type == key.type && item.Value.side == key.side)
            {
                n = item.Key;
                break;
            }
        }

        if(n == "")
        {
            Debug.Log("wath fucfucfucfuck");
            return;
        }

        keyBindList[n] = null;

        if(con == ControllerType.KeyboardMouse)
            _keyboardBind.Remove(n);
        if(con == ControllerType.XboxController)
            _xboxBind.Remove(n);
        if(con == ControllerType.PSController)
            _psBind.Remove(n);
    
        Debug.Log("bind key delete : " + n);
    }

    public void KeyBind(string keyName,ControllerType ct, Key key,Key prevKey)
    {
        //_keyBreak = true;

        var n = FindOverlapKey(key);

        if(n != null)
        {
            Debug.Log(n);
            Debug.Log(keyName);
        }
        
        // if(keyBindList.ContainsKey(keyName))
        // {
            key.state = KeyState.Press;
            keyBindList[keyName] = key;

            if(ct == ControllerType.KeyboardMouse)
            {
                _keyboardBind[keyName] = key;
            }
            else if(ct == ControllerType.XboxController)
            {
                _xboxBind[keyName] = key;
            }
            else if(ct == ControllerType.PSController)
            {
                _psBind[keyName] = key;
            }

        if(n != null && n != keyName)
        {
            prevKey.state = KeyState.Press;
            keyBindList[n] = prevKey;

            if(ct == ControllerType.KeyboardMouse)
            {
                _keyboardBind[n] = prevKey;
            }
            else if(ct == ControllerType.XboxController)
            {
                _xboxBind[n] = prevKey;
            }
            else if(ct == ControllerType.PSController)
            {
                _psBind[n] = prevKey;
            }

        }
        // }
        // else
        // {
        //     Debug.Log("KeyName Error : " + keyName);
        // }
    }

    public string FindOverlapKey(Key key)
    {
        foreach(var item in keyBindList)
        {
            if(item.Value.axisName == key.axisName && item.Value.code == key.code && item.Value.type == key.type && item.Value.side == key.side)
            {
                return item.Key;
            }
        }

        return null;
    }

    public void CreateKeys()
    {
        // if(keyBindList.Count != 0)
        //     return;

        LoadControllerSprites();

        LoadKeyBindInfo();
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
        if(con != null && con.Length > 0 && con[0] != _controllerLists)
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
//        Debugger.instance.SetDebugText(controller.ToString());

        if(_keyBreak)
        {
            return;
        }

        foreach(var key in keyBindList)
        {
            var k = key.Value;

            if(k == null)
                continue;
            
            if(k.type == KeyType.Button)
            {
                // if(Input.GetKeyDown(k.code))
                // {
                //     k.state = KeyState.Down;
                // }
                // else if(Input.GetKey(k.code))
                // {
                //     k.state = KeyState.Press;
                // }
                // else if(Input.GetKeyUp(k.code))
                // {
                //     k.state = KeyState.Up;
                // }
                // else if(!Input.GetKey(k.code) && (k.state == KeyState.Press || k.state == KeyState.Down))
                // {
                //     k.state = KeyState.Up;
                // }
                // else if(k.state == KeyState.Up)
                // {
                //     k.state = KeyState.None;
                // }

                bool down = Input.GetKey(k.code);
                
                if(down && k.state == KeyState.Down)
                    k.state = KeyState.Press;
                else if(down && k.state == KeyState.None)
                    k.state = KeyState.Down;
                else if(!down && (k.state == KeyState.Press || k.state == KeyState.Down))
                    k.state = KeyState.Up;
                else if(k.state == KeyState.Up)
                    k.state = KeyState.None;
            }
            else if(k.type == KeyType.Axis)
            {
                k.axis = Input.GetAxis(k.axisName);
                bool down = k.axis >= _axisPushStr;

                if(down && k.state == KeyState.Down)
                    k.state = KeyState.Press;
                else if(down && k.state == KeyState.None)
                    k.state = KeyState.Down;
                else if(!down && (k.state == KeyState.Press || k.state == KeyState.Down))
                    k.state = KeyState.Up;
                else if(k.state == KeyState.Up)
                    k.state = KeyState.None;
            }
            else if(k.type == KeyType.TwoSideAxisButton)
            {
                k.axis = Input.GetAxis(k.axisName);
                bool down = k.side ? k.axis > 0f : (k.axis < 0f);

                if(down && k.state == KeyState.Down)
                    k.state = KeyState.Press;
                else if(down && k.state == KeyState.None)
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

    public Key GetBindedKeyInfo(string key, ControllerType type)
    {
        if(type == ControllerType.KeyboardMouse)
        {
            if(_keyboardBind.ContainsKey(key))
                return _keyboardBind[key];
        }
        else if(type == ControllerType.XboxController)
        {
            if(_xboxBind.ContainsKey(key))
                return _xboxBind[key];
        }
        else if(type == ControllerType.PSController)
        {
            if(_psBind.ContainsKey(key))
                return _psBind[key];
        }

        return null;
    }

    public Key GetCurrentKeyInput()
    {
        if(controller == ControllerType.KeyboardMouse)
            return GetCurrentKeyboardMouseInput();
        else if(controller == ControllerType.XboxController)
            return GetCurrentXboxControllerInput();
        else if(controller == ControllerType.PSController)
            return GetCurrentPSContollerInput();
        
        return null;
    }

    public Sprite GetXboxGraphic(ControllerEx.Key key)
    {
        if(key.type == ControllerEx.KeyType.Button)
        {
            if(key.code == KeyCode.JoystickButton0)
                return _xboxKeys[6];
            else if(key.code == KeyCode.JoystickButton1)
                return _xboxKeys[7];
            else if(key.code == KeyCode.JoystickButton2)
                return _xboxKeys[8];
            else if(key.code == KeyCode.JoystickButton3)
                return _xboxKeys[9];
            else if(key.code == KeyCode.JoystickButton4)
                return _xboxKeys[11];
            else if(key.code == KeyCode.JoystickButton5)
                return _xboxKeys[12];
            else if(key.code == KeyCode.JoystickButton6)
                return _xboxKeys[15];
            else if(key.code == KeyCode.JoystickButton7)
                return _xboxKeys[14];
            else if(key.code == KeyCode.JoystickButton8)
                return _xboxKeys[0];
            else if(key.code == KeyCode.JoystickButton9)
                return _xboxKeys[1];
        }
        else if(key.type == ControllerEx.KeyType.Axis)
        {
            if(key.axisName == "XBoxLeftTrigger")
                return _xboxKeys[10];
            else if(key.axisName == "XBoxRightTrigger")
            {
                return _xboxKeys[13];
            }
        }
        else if(key.type == ControllerEx.KeyType.TwoSideAxisButton)
        {
            if(key.axisName == "XBoxDPadHorizontal")
                return _xboxKeys[key.side ? 5 : 4];
            else if(key.axisName == "XBoxDPadVertical")
                return _xboxKeys[key.side ? 2 : 3];
        }

        return null;
    }
    public Sprite GetPSGraphic(ControllerEx.Key key)
    {
        if(key.type == ControllerEx.KeyType.Button)
        {
            if(key.code == KeyCode.JoystickButton0)
                return _psKeys[8];
            else if(key.code == KeyCode.JoystickButton1)
                return _psKeys[6];
            else if(key.code == KeyCode.JoystickButton2)
                return _psKeys[7];
            else if(key.code == KeyCode.JoystickButton3)
                return _psKeys[9];
            else if(key.code == KeyCode.JoystickButton4)
                return _psKeys[10];
            else if(key.code == KeyCode.JoystickButton5)
                return _psKeys[11];
            else if(key.code == KeyCode.JoystickButton8)
                return _psKeys[14];
            else if(key.code == KeyCode.JoystickButton9)
                return _psKeys[15];
            else if(key.code == KeyCode.JoystickButton10)
                return _psKeys[0];
            else if(key.code == KeyCode.JoystickButton11)
                return _psKeys[1];
        }
        else if(key.type == ControllerEx.KeyType.Axis)
        {
            if(key.axisName == "PSLeftTrigger")
                return _psKeys[12];
            else if(key.axisName == "PSRightTrigger")
                return _psKeys[13];
        }
        else if(key.type == ControllerEx.KeyType.TwoSideAxisButton)
        {
            if(key.axisName == "PSDPadHorizontal")
                return _psKeys[key.side ? 5 : 4];
            else if(key.axisName == "PSDPadVertical")
                return _psKeys[key.side ? 2 : 3];
        }

        return null;
    }

    public Key GetCurrentKeyboardMouseInput()
    {
        if (Input.anyKeyDown)
        {
            for (int i = 0; i < keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(keyCodes[i]))
                {
                    Key key = new Key(keyCodes[i]);
                    return key;
                }
            }
        }
        
        return null;
    }

    public Key GetCurrentXboxControllerInput()
    {
        if (Input.anyKeyDown)
        {
            if(Input.GetKeyDown(KeyCode.JoystickButton0)) { return new Key(KeyCode.JoystickButton0);}
            if(Input.GetKeyDown(KeyCode.JoystickButton1)) { return new Key(KeyCode.JoystickButton1);}
            if(Input.GetKeyDown(KeyCode.JoystickButton2)) { return new Key(KeyCode.JoystickButton2);}
            if(Input.GetKeyDown(KeyCode.JoystickButton3)) { return new Key(KeyCode.JoystickButton3);}
            if(Input.GetKeyDown(KeyCode.JoystickButton4)) { return new Key(KeyCode.JoystickButton4);}
            if(Input.GetKeyDown(KeyCode.JoystickButton5)) { return new Key(KeyCode.JoystickButton5);}
            if(Input.GetKeyDown(KeyCode.JoystickButton6)) { return new Key(KeyCode.JoystickButton6);}
            if(Input.GetKeyDown(KeyCode.JoystickButton7)) { return new Key(KeyCode.JoystickButton7);}
            if(Input.GetKeyDown(KeyCode.JoystickButton8)) { return new Key(KeyCode.JoystickButton8);}
            if(Input.GetKeyDown(KeyCode.JoystickButton9)) { return new Key(KeyCode.JoystickButton9);}
        }

        if(Input.GetAxis("XBoxDPadHorizontal") > 0f) {return new Key("XBoxDPadHorizontal",true);}
        if(Input.GetAxis("XBoxDPadHorizontal") < 0f) {return new Key("XBoxDPadHorizontal",false);}
        if(Input.GetAxis("XBoxDPadVertical") > 0f) {return new Key("XBoxDPadVertical",true);}
        if(Input.GetAxis("XBoxDPadVertical") < 0f) {return new Key("XBoxDPadVertical",false);}
        if(Input.GetAxis("XBoxLeftTrigger") > _axisPushStr) {return new Key("XBoxLeftTrigger");}
        if(Input.GetAxis("XBoxRightTrigger") > _axisPushStr) {return new Key("XBoxRightTrigger");}

        return null;
    }

    public Key GetCurrentPSContollerInput()
    {
        if (Input.anyKeyDown)
        {
            if(Input.GetKeyDown(KeyCode.JoystickButton0)) { return new Key(KeyCode.JoystickButton0);}
            if(Input.GetKeyDown(KeyCode.JoystickButton1)) { return new Key(KeyCode.JoystickButton1);}
            if(Input.GetKeyDown(KeyCode.JoystickButton2)) { return new Key(KeyCode.JoystickButton2);}
            if(Input.GetKeyDown(KeyCode.JoystickButton3)) { return new Key(KeyCode.JoystickButton3);}
            if(Input.GetKeyDown(KeyCode.JoystickButton4)) { return new Key(KeyCode.JoystickButton4);}
            if(Input.GetKeyDown(KeyCode.JoystickButton5)) { return new Key(KeyCode.JoystickButton5);}
            if(Input.GetKeyDown(KeyCode.JoystickButton8)) { return new Key(KeyCode.JoystickButton8);}
            if(Input.GetKeyDown(KeyCode.JoystickButton9)) { return new Key(KeyCode.JoystickButton9);}
            if(Input.GetKeyDown(KeyCode.JoystickButton10)) { return new Key(KeyCode.JoystickButton10);}
            if(Input.GetKeyDown(KeyCode.JoystickButton11)) { return new Key(KeyCode.JoystickButton11);}
        }

        if(Input.GetAxis("PSDPadHorizontal") > 0f) {return new Key("PSDPadHorizontal",true);}
        if(Input.GetAxis("PSDPadHorizontal") < 0f) {return new Key("PSDPadHorizontal",false);}
        if(Input.GetAxis("PSDPadVertical") > 0f) {return new Key("PSDPadVertical",true);}
        if(Input.GetAxis("PSDPadVertical") < 0f) {return new Key("PSDPadVertical",false);}
        if(Input.GetAxis("PSLeftTrigger") > _axisPushStr) {return new Key("PSLeftTrigger");}
        if(Input.GetAxis("PSRightTrigger") > _axisPushStr) {return new Key("PSRightTrigger");}

        return null;
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
