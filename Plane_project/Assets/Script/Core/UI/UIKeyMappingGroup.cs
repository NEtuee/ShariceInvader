using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyMappingGroup : MonoBehaviour
{
    public UISelectMenu parent;
    public UIKeyMapper[] keyMappers;

    private static bool _isChange = false;

    public void Awake()
    {
        for(int i = 0; i < keyMappers.Length; ++i)
        {
            int j = i;

            KeyBind(keyMappers[j]);
            GraphicUpdate(keyMappers[j]);

            keyMappers[i].selectEvent.AddListener(delegate{
                //keyMappers[j].ClearKey();
                GraphicUpdate(keyMappers[j],true);
            });
            keyMappers[i].deselectEvent.AddListener(delegate{
                OverlapCheck(keyMappers,keyMappers[j]);
                GraphicUpdate(keyMappers[j]);
            });
        }

        parent.deselectEvent.AddListener(delegate{
            if(_isChange)
                ControllerEx.GetInstance().SaveKeyBindInfo();
            
            _isChange = false;
        });

        // parent.manager.deactiveEvent.AddListener(delegate{
        //     if(_isChange)
        //         ControllerEx.GetInstance().SaveKeyBindInfo();
            
        //     _isChange = false;
        // });
    }

    public void KeyBind(UIKeyMapper key)
    {
        key.bindKey = ControllerEx.GetInstance().GetBindedKeyInfo(key.keyName,key.checkType);
    }

    public void GraphicUpdate(UIKeyMapper mapper, bool setNull = false)
    {
        if(mapper.checkType == ControllerEx.ControllerType.KeyboardMouse)
        {
            if(mapper.bindKey == null || setNull)
                mapper.SetButtonText("-");
            else
            {
                string s = mapper.bindKey.code.ToString();
                s = s == "Mouse0" ? "LMB" : (s == "Mouse1" ? "RMB" : s);
                mapper.SetButtonText(s);
            }
        }
        else if(mapper.checkType == ControllerEx.ControllerType.XboxController)
        {
            if(mapper.bindKey == null || setNull)
                mapper.SetButtonSprite(null);
            else
                mapper.SetButtonSprite(ControllerEx.GetInstance().GetXboxGraphic(mapper.bindKey));
        }
        else if(mapper.checkType == ControllerEx.ControllerType.PSController)
        {
            if(mapper.bindKey == null || setNull)
                mapper.SetButtonSprite(null);
            else
                mapper.SetButtonSprite(ControllerEx.GetInstance().GetPSGraphic(mapper.bindKey));
        }
    }

    public void OverlapCheck(UIKeyMapper[] mappers, UIKeyMapper mapper)
    {
        _isChange = true;

        foreach(var item in mappers)
        {
            if(item == mapper)
                continue;
            
            if(item.bindKey != null)
            {
                if(item.bindKey.type == mapper.bindKey.type)
                {
                    bool check = false;

                    if(mapper.bindKey.type == ControllerEx.KeyType.Button)
                    {
                        check = mapper.bindKey.code == item.bindKey.code;
                    }
                    else if(mapper.bindKey.type == ControllerEx.KeyType.Axis)
                    {
                        check = mapper.bindKey.axisName == item.bindKey.axisName;
                    }
                    else if(mapper.bindKey.type == ControllerEx.KeyType.TwoSideAxisButton)
                    {
                        check = mapper.bindKey.axisName == item.bindKey.axisName &&
                                (mapper.bindKey.side == item.bindKey.side);
                    }

                    if(check)
                    {
                        // ControllerEx.GetInstance().DeleteBindInfo(item.bindKey,item.checkType);
                        // item.ClearKey();
                        Debug.Log(item.keyName);
                        item.bindKey = ControllerEx.GetInstance().keyBindList[item.keyName];
                        GraphicUpdate(item);
                        break;
                    }
                }
            }
        }

    }


}
