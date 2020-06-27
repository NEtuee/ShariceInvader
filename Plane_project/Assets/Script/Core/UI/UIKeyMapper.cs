using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyMapper : UISelectButton
{
    public ControllerEx.ControllerType checkType;
    public string keyName;

    public ControllerEx.Key bindKey;

    private bool checkKey = false;
    private bool keyBreak = false;

    public override void Progress(float deltaTime)
    {
        if(checkKey)
        {
            if(keyBreak)
            {
                keyBreak = false;
                return;
            }

            if(checkType == ControllerEx.ControllerType.KeyboardMouse)
                KeyBind(ControllerEx.GetInstance().GetCurrentKeyboardMouseInput());
            else if(checkType == ControllerEx.ControllerType.XboxController)
                KeyBind(ControllerEx.GetInstance().GetCurrentXboxControllerInput());
            else if(checkType == ControllerEx.ControllerType.PSController)
                KeyBind(ControllerEx.GetInstance().GetCurrentPSContollerInput());
        }
    }

    public override void ColorSync(Color color)
    {
        if(buttonGraphic != null)
        {
            var c = buttonGraphic.color;
            c.a = color.a;
            buttonGraphic.color = c;
        }

        if(buttonText != null)
        {
            var c = buttonText.textColor;
            c.a = color.a;
            buttonText.textColor = c;
            buttonText.UpdateColor();
        }
    }

    public void KeyBind(ControllerEx.Key key)
    {
        if(key == null)
            return;
        var save = bindKey;
        bindKey = key;
        key.state = ControllerEx.KeyState.Press;

        ControllerEx.GetInstance().KeyBind(keyName,checkType,key,save);

        keyBreak = true;
        Deselect();
    }

    public void ClearKey()
    {
        bindKey = null;
        checkKey = false;
    }

    public override void Select()
    {
        if(checkType == ControllerEx.ControllerType.KeyboardMouse)
        {
            if(ControllerEx.GetInstance().GetCurrentKeyboardMouseInput() == null)
            {
                return;
            }
        }
        else if(checkType == ControllerEx.ControllerType.XboxController)
        {
            if(ControllerEx.GetInstance().GetCurrentXboxControllerInput() == null)
            {
                return;
            }
        }
        else if(checkType == ControllerEx.ControllerType.PSController)
        {
            if(ControllerEx.GetInstance().GetCurrentPSContollerInput() == null)
            {
                return;
            }
        }
    

        base.Select();
    }

    public override void SelectEvent()
    {
        base.SelectEvent();
        checkKey = true;
        keyBreak = true;
        manager.keyCheckLock = true;
        ControllerEx.GetInstance()._keyBreak = true;
    }

    public override void DeselectEvent()
    {
        base.DeselectEvent();
        checkKey = false;
        manager.keyCheckLock = false;
        ControllerEx.GetInstance()._keyBreak = false;
    }
}
