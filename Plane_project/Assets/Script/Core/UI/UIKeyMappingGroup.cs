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
                keyMappers[j].ClearKey();
                GraphicUpdate(keyMappers[j]);
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
    }

    public void KeyBind(UIKeyMapper key)
    {
        key.bindKey = ControllerEx.GetInstance().GetBindedKeyInfo(key.keyName,key.checkType);
    }

    public void GraphicUpdate(UIKeyMapper mapper)
    {
        if(mapper.checkType == ControllerEx.ControllerType.KeyboardMouse)
        {
            if(mapper.bindKey == null)
                mapper.SetButtonText("-");
            else
                mapper.SetButtonText(mapper.bindKey.code.ToString());
        }
        else if(mapper.checkType == ControllerEx.ControllerType.XboxController)
        {
            if(mapper.bindKey == null)
                mapper.SetButtonSprite(null);
            else
                mapper.SetButtonSprite(ControllerEx.GetInstance().GetXboxGraphic(mapper.bindKey));
        }
        else if(mapper.checkType == ControllerEx.ControllerType.PSController)
        {
            if(mapper.bindKey == null)
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
                                (mapper.bindKey.side && item.bindKey.side);
                    }

                    if(check)
                    {
                        item.ClearKey();
                        GraphicUpdate(item);
                        break;
                    }
                }
            }
        }

    }


}
