using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonToggleGroup : MonoBehaviour
{
    public UISelectButton[] toggles;

    [HideInInspector]
    public UISelectButton current;

    public bool ovelapCheck = true;

    private int _prev = -1;
    private int _curr = -1;

    public void Awake()
    {
        for(int i = 0; i < toggles.Length; ++i)
        {
            toggles[i].isToggle = true;
            toggles[i].deselectLock = ovelapCheck;
            int j = i;
            toggles[i].selectEvent.AddListener(delegate{SelectUI(j);});
        }
    }

    public void SelectUI(int pos)
    {
        if(_curr != -1 && _curr == pos && ovelapCheck)
        {
            return;
        }

        _prev = _curr;

        if(_prev != -1)
        {
            toggles[_prev].deselectLock = false;
            toggles[_prev].Deselect();
            toggles[_prev].deselectLock = ovelapCheck;
        }
        _curr = pos;

        current = toggles[_curr];
    }
}
