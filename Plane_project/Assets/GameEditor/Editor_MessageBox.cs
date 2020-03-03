using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_MessageBox : MonoBehaviour
{
    public Text title;
    public Text contents;

    private Action _okAction;

    public void Active(string t, string c, Action a)
    {
        title.text = t;
        contents.text = c;

        _okAction = a;

        PanelActive(true);
    }

    public void OkProgress()
    {
        PanelActive(false);
        _okAction();
    }

    public void CancelProgress()
    {
        PanelActive(false);
    }

    public void PanelActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
