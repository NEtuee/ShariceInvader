using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISortManager : MonoBehaviour
{
    [System.Serializable]
    public class MenuItemBase
    {
        public UISelectBase[] items;
        //public int selectedPos = -1;
        public void Set(UISelectBase[] s) {items = s;}
    };

    public UnityEvent deactiveEvent;

    public List<MenuItemBase> menuItems;
    public UISelectBase[] currentMenu;
    public Transform uiArrow;

    public bool lineCrossLock = false;
    public bool uiSelectLock = false;
    public bool keyCheckLock = false;

    public Vector3 menuDist = new Vector3(0f,1f);
    public float arrowSpeed = 0.3f;
    public float arrowDist = -0.1f;

    private UISelectBase selectedUI;
    private int selectedLine = 0;
    public int selectedMenuPos = 0;

    public void Awake()
    {
        Initialize();

        LineSelect(0);
    }

    public void Update()
    {
        ArrowMovement();
        KeyInputCheck();

        UpdateMenuItems();
        UpdateMenuItemPos(menuItems);
    }

    public void Active()
    {
        gameObject.SetActive(true);
        GameMain.instance.update = false;
        
        uiSelectLock = false;
        lineCrossLock = false;
        keyCheckLock = false;

        selectedMenuPos = 0;
        LineSelect(0);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GameMain.instance.update = true;

        foreach(var item in menuItems)
            foreach(var ui in item.items)
                ui.UICloseEvent();
                
        deactiveEvent.Invoke();
    }

    public void UpdateMenuItemPos(List<MenuItemBase> itemList)
    {
        for(int i = 0; i < itemList.Count; ++i)
        {
            UpdateMenuItemPos(itemList[i].items,menuDist);
        }
    }

    public void UpdateMenuItemPos(UISelectBase[] items, Vector3 dist)
    {
        for(int i = 0; i < items.Length; ++i)
        {
            if(i == 0)
                continue;
            
            var pos = items[i].tp.position;
            pos.y = items[i - 1].tp.position.y + items[i - 1].uiHeight;
            items[i].tp.position = pos - dist;
        }
    }

    public void UpdateMenuItems()
    {
        foreach(var menu in menuItems)
        {
            if(currentMenu != menu.items)
            {
                foreach(var item in menu.items)
                {
                    item.Progress(Time.deltaTime);
                }
            }
            
        }

        foreach(var item in currentMenu)
        {
            item.Progress(Time.deltaTime);
        }
    }

    public void Initialize()
    {
        foreach(var menu in menuItems)
        {
            int i = 0;
            foreach(var item in menu.items)
            {
                item.menuOrder = i++;
                item.manager = this;
                item.Initialize();
            }
        }
    }

    public void KeyInputCheck()
    {
        //if(Input.GetKeyDown(KeyCode.))
        if(keyCheckLock)
            return;
        
        if(ControllerEx.GetInstance().KeyDown("Up"))
        {
            if(selectedMenuPos - 1 >= 0)
            {
                SetSelectedUI(selectedMenuPos - 1);
            }
        }
        if(ControllerEx.GetInstance().KeyDown("Down"))
        {
            if(selectedMenuPos + 1 < currentMenu.Length)
            {
                SetSelectedUI(selectedMenuPos + 1);
            }
        }
        if(ControllerEx.GetInstance().KeyDown("Left"))
        {
            if(selectedLine - 1 >= 0)
            {
                LineSelect(selectedLine - 1);
            }
        }
        if(ControllerEx.GetInstance().KeyDown("Right"))
        {
            if(selectedLine + 1 < menuItems.Count)
            {
                LineSelect(selectedLine + 1);
            }
        }
        if(ControllerEx.GetInstance().KeyDown("MainAttack"))
        {
            selectedUI.SelectAction();
        }
        if(ControllerEx.GetInstance().KeyDown("Cancel"))
        {
            CancleMenu();
        }
        if(ControllerEx.GetInstance().KeyDown("Option"))
        {
            Close();
        }
        
        
    }

    public void CancleMenu()
    {
        if(currentMenu != menuItems[selectedLine].items)
        {
            selectedUI.parentUI.Deselect();
        }
        else
            Close();
    }

    public void ArrowMovement()
    {
        var bounds = selectedUI.GetBounds();
        var pos = new Vector3(bounds.min.x + arrowDist,bounds.min.y + ((bounds.max.y - bounds.min.y) * .5f));
        uiArrow.transform.position = Vector3.Lerp(uiArrow.transform.position,pos,arrowSpeed);
    }

    public void SetSelectedUI(int pos)
    {
        if(uiSelectLock)
            return;
        
        selectedMenuPos = pos;
        selectedUI = currentMenu[selectedMenuPos];
    }

    public void LineSelect(int line)
    {
        if(lineCrossLock)
            return;
        
        selectedLine = line;
        var item = menuItems[selectedLine];

        //item.selectedPos = MenuBind(item.items,item.selectedPos);
        selectedMenuPos = MenuBind(item.items,selectedMenuPos);
    }

    public void MenuBind(int menuPos)
    {
        selectedMenuPos = MenuBind(menuItems[selectedLine].items,menuPos);
    }

    public int MenuBind(UISelectBase[] menu, int menuPos)
    {
        currentMenu = menu;
        menuPos = currentMenu.Length <= menuPos ? currentMenu.Length - 1 : menuPos;
        SetSelectedUI(menuPos);

        return menuPos;
    }
}
