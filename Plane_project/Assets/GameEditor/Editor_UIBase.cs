using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_UIBase : MonoBehaviour
{
    public bool selected = false;
    public RectTransform rectTp;
    public Text hideButtonText;
    
    public GameObject mainContent;
    public GameObject contentLockPanel;

    private bool _hide = false;
    private Vector2 _mainPos;


    public void Select()
    {
        _mainPos = rectTp.anchoredPosition;
    }

    public void Move(Vector2 pos)
    {
        rectTp.anchoredPosition = _mainPos + pos;
    }

    public void Release()
    {

    }

    public void HideButton()
    {
        _hide = !_hide;
        hideButtonText.text = _hide ? "▼" : "-";
        mainContent.SetActive(!_hide);
    }
    
    public void ContentLock(bool b)
    {
        contentLockPanel.SetActive(b);
    }
}
