using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonToggleGroup : MonoBehaviour
{
    public UISelectButton[] toggles;

    [HideInInspector]
    public UISelectButton current;

    private int _prev = -1;
    private int _curr = -1;

    public void Awake()
    {
        for(int i = 0; i < toggles.Length; ++i)
        {
            toggles[i].isToggle = true;
            int j = i;
            toggles[i].selectEvent.AddListener(delegate{SelectUI(j);});
        }
    }

    public void SelectUI(int pos)
    {
        _prev = _curr;

        if(_prev != -1)
            toggles[_prev].Deselect();
        
        _curr = pos;

        current = toggles[_curr];
    }
}
