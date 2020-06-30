using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISelectMenu : UISelectBase
{
    public UnityEvent selectEvent = new UnityEvent();
    public UnityEvent deselectEvent = new UnityEvent();

    public Vector3 startSpace = new Vector3(0.2f,0.3f);
    public float bottomSpace = 0.3f;
    public float speed = 1f;

    public bool autoDeselect = false;
    public bool open = false;

    private static int selectOrder = 0;
    private static bool closing = false;
    private bool progressing = false;
    public float _timer = 1f;

    public override void Initialize()
    {
        base.Initialize();

        int i = 0;
        foreach(var item in menuItems)
        {
            item.menuOrder = i++;
            item.parentUI = this;
            item.manager = this.manager;
            item.Initialize();
            item.tp.position = tp.position + new Vector3(startSpace.x,0f,0f);
        }
    }

    public override void Progress(float deltaTime)
    {
        if(manager.currentMenu != menuItems && open)
        {
            for(int i = 0; i < menuItems.Length; ++i)
            {
                menuItems[i].Progress(deltaTime);
                // Debug.Log(menuItems[i].name);
                
            }

            // for(int i = 1; i < menuItems.Length; ++i)
            // {
            //     var endPos = menuItems[i - 1].tp.position + (menuDist);
            //     endPos.y = endPos.y + menuItems[i - 1].uiHeight;
            //     menuItems[i].tp.position = endPos;
            // }
        
        }

        // if(manager.currentMenu == menuItems && open)
        // {
        //     Debug.Log(name + "two");
        //     for(int i = 1; i < menuItems.Length; ++i)
        //     {
        //         var endPos = menuItems[i - 1].tp.position + (menuDist);
        //         endPos.y = endPos.y + menuItems[i - 1].uiHeight;
        //         menuItems[i].tp.position = endPos;
        //     }
        // }

        if(parentUI != null && menuOrder != 0)
        {
            if(parentUI != null)
            {
                var endPos = parentUI.menuItems[menuOrder - 1].tp.position + (parentUI.menuDist);
                endPos.y = endPos.y + parentUI.menuItems[menuOrder - 1].uiHeight;
                parentUI.menuItems[menuOrder].tp.position = endPos;
            }
        }
        // if(parentUI != null && menuOrder != 0)
        // {
        //     if(parentUI != null)
        //     {
        //         var endPos = parentUI.menuItems[menuOrder - 1].tp.position + (menuDist);
        //         endPos.y = endPos.y + parentUI.menuItems[menuOrder - 1].uiHeight;
        //         parentUI.menuItems[menuOrder].tp.position = endPos;
        //     }

        // }

        MenuMovementProgress(deltaTime);
    }

    public UISelectBase[] GetMenuItems() {return menuItems;}

    public void MenuMovementProgress(float deltaTime)
    {
        if(progressing)
        {
            _timer += speed * deltaTime;

            if(open)
            {
                float bottom = MathEx.easeOutCubic(0f,bottomSpace,_timer);
                var bounds = GetBounds();
                uiHeight = -(bounds.min.y - menuItems[menuItems.Length - 1].GetBounds().min.y) + 
                            bottom;

                for(int i = 0; i < menuItems.Length; ++i)
                {
                    var endPos = tp.position + startSpace + (menuDist * (float)i);
                    menuItems[i].tp.position = 
                        MathEx.easeOutCubicVector2(tp.position + new Vector3(startSpace.x,0f,0f),endPos,_timer);

                    Color color = menuItems[i].mainText.color;
                    color.a = MathEx.easeInCubic(0f,1f,_timer);

                    if(_timer >= 1f)
                    {
                        menuItems[i].tp.position = endPos;
                        //menuItems[i].gameObject.SetActive(false);
                        color.a = 1f;
                    }

                    menuItems[i].mainText.color = color;
                    menuItems[i].ColorSync(color);
                }
            }
            else
            {
                float bottom = MathEx.easeOutCubic(bottomSpace, 0f, _timer);
                var bounds = GetBounds();
                uiHeight = -(bounds.min.y - menuItems[menuItems.Length - 1].GetBounds().min.y) + 
                            bottom;

                for(int i = 0; i < menuItems.Length; ++i)
                {
                    var startPos = tp.position + startSpace + (menuDist * (float)i);

                    Color color = menuItems[i].mainText.color;
                    color.a = MathEx.easeOutCubic(1f,0f,_timer);

                    if(_timer >= 1f)
                    {
                        menuItems[i].tp.position = tp.position + new Vector3(startSpace.x,0f,0f);
                        menuItems[i].gameObject.SetActive(false);

                        UISizeCalc();
                        color.a = 1f;
                    }
                    else
                        menuItems[i].tp.position = 
                            MathEx.easeOutCubicVector2(startPos,tp.position + new Vector3(startSpace.x,0f,0f),_timer);
                    
                    menuItems[i].mainText.color = color;
                    menuItems[i].ColorSync(color);
                }

            }

            if(_timer >= 1f)
            {
                progressing = false;
                _timer = 1f;
            }
        }
    }

    public void OpenMenu()
    {
        open = true;
        progressing = true;
        _timer = 1f - _timer;
    }

    public void CloseMenu()
    {
        open = false;
        progressing = true;
        _timer = 1f - _timer;
    }

    public void MenuSelect(int pos, bool selectEvent)
    {
        if(pos < 0 || menuItems == null || pos > menuItems.Length)
        {
            return;
        }

        menuItems[pos].Select(selectEvent);
    }

    public override void SelectEvent()
    {
        foreach(var item in menuItems)
            item.gameObject.SetActive(true);

        if(!open)
            OpenMenu();

        SoundManager.instance.Play("SE/MenuSelect",false);

        manager.lineCrossLock = true;

        manager.MenuBind(menuItems,closing ? selectOrder : 0);

        closing = false;
        if(parentUI != null)
            selectOrder = menuOrder;
        else
            selectOrder = 0;

        selectEvent.Invoke();
    }

    public override void DeselectEvent()
    {
        if(autoDeselect)
        {
            foreach(var item in menuItems)
            {
                if(item.isSelected)
                {
                    item.Deselect();
                }
            }
        }
        

        CloseMenu();

        SoundManager.instance.Play("SE/ButtonSelect",false);

        manager.lineCrossLock = false;

        deselectEvent.Invoke();
        closing = true;

        if(parentUI == null)
            manager.MenuBind(menuOrder);
        else
        {
            parentUI.Select();
        }
        //manager.MenuBind(parentUI.menuOrder);
    }

    public override void UICloseEvent()
    {
        if(open)
        {
            foreach(var item in menuItems)
            {
                item.UICloseEvent();
            }

            Deselect();
            MenuMovementProgress(1000f);
        }
    }
}
