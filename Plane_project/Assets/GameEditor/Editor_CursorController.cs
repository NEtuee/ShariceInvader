using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_CursorController : SingletonMono<Editor_CursorController>
{
    public delegate void deleteCursor(Editor_CursorBase cursor);


    public deleteCursor deleteCursorEvent = new deleteCursor((Editor_CursorBase cursor)=>{});
    public GameObject cursorBase;
    public Text nameText;
    public InputField posX;
    public InputField posY;

    public Toggle hideCursor;

    public Editor_UIBase uIBase;

    public List<Editor_CursorBase> cursorList = new List<Editor_CursorBase>();

    private bool _cursorClicked = false;
    private bool _multipleSelected = false;
    private Queue<Editor_CursorBase> _cursorPool = new Queue<Editor_CursorBase>();

    

    private Vector2 clickedPos;

    private GameObject _cursorParent;

    public void Start()
    {
        SetSingleton(this);

        Editor_EventSystem.instance.nullClickEvent += CursorSelectEvent;
        Editor_EventSystem.instance.mouseMoveEvent += CursorMoveEvent;
        Editor_EventSystem.instance.keyUpEvent += KeyUpEvent;

        Editor_CursorBase.cursorValueChangedEvent += UpdateCursorInfo;

        _cursorParent = new GameObject("Cursors");

        UIActive(false);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            int count = Editor_CursorBase.selectedCursorList.Count;

            if(count == 1)
                DeleteCursor(Editor_CursorBase.selectedCursorList[0]);
            else if(count > 1 && count != 0)
                DeleteCursor(Editor_CursorBase.selectedCursorList.ToArray());
        }
    }

    public Editor_CursorBase AddCursor(string n, Vector2 pos, Sprite sprite)
    {
        Editor_CursorBase cursor = null;

        if(_cursorPool.Count == 0)
        {
            cursor = Instantiate(cursorBase,Vector3.zero,Quaternion.identity).GetComponent<Editor_CursorBase>();
            cursor.firstSetting();
        }
        else
        {
            cursor = _cursorPool.Dequeue();
        }

        cursor.name = n;
        cursor.initCursor();

        cursor.transform.position = pos;
        cursor.mainRenderer.sprite = sprite;
        cursor.gameObject.SetActive(true);

        cursor.transform.SetParent(_cursorParent.transform);

        cursorList.Add(cursor);

        return cursor;
    }

    public void UIActive(bool val)
    {
        uIBase.ContentLock(!val);
    }

    public void HideCursor()
    {
        _cursorParent.SetActive(!hideCursor.isOn);
    }

    private Editor_CursorBase[] _deleteCursors;
    public void DeleteCursor(Editor_CursorBase cursor)
    {
        _deleteCursors = new Editor_CursorBase[]{cursor};
        Editor_EventSystem.instance.ActiveMessageBox("DeleteCursor","Are you sure you want to delete\n" + cursor.name + " ?",DeleteCursor);
    }
    public void DeleteCursor(Editor_CursorBase[] cursor)
    {
        _deleteCursors = cursor;
        Editor_EventSystem.instance.ActiveMessageBox("DeleteCursor","Are you sure you want to delete the " + cursor.Length + " selected cursors?",DeleteCursor);
    }
    public void DeleteCursor()
    {
        foreach(var cursor in _deleteCursors)
        {
            cursor.Deselect();
            cursor.gameObject.SetActive(false);
            cursorList.Remove(cursor);

            _cursorPool.Enqueue(cursor);

            deleteCursorEvent(cursor);
        }
        

        if(cursorList.Count == 0)
            UIActive(false);
        
        if(_deleteCursors.Length > 1)
            Editor_EventSystem.instance.ActiveNotice(_deleteCursors.Length.ToString() + " Cursors deleted");
        else
            Editor_EventSystem.instance.ActiveNotice(_deleteCursors[0].name + " Cursor deleted");
    }

    public void DisableAllCursor()
    {
        Editor_CursorBase.DeselectAll();

        foreach(var cursor in cursorList)
        {
            cursor.gameObject.SetActive(false);
            _cursorPool.Enqueue(cursor);
        }

        cursorList.Clear();
    }

    public void WhenXValueChanged()
    {
        if(posX.text == "")
            return;

        Editor_CursorBase.SetXValue(float.Parse(posX.text));

        Editor_CursorBase.UpdateCenterPosition();
    }

    public void WhenYValueChanged()
    {
        if(posY.text == "")
            return;

        Editor_CursorBase.SetYValue(float.Parse(posY.text));

        Editor_CursorBase.UpdateCenterPosition();
    }

    public void UpdateCursorInfo(Editor_CursorBase cursor, bool multiSelect)
    {
        if(!multiSelect)
        {
            nameText.text = cursor.name;
            posX.text = cursor.transform.position.x.ToString();
            posY.text = cursor.transform.position.y.ToString();
        }
        else
        {
            if(Editor_CursorBase.selectedCursorList.Count > 1)
            {
                nameText.text = "Multiple Cursor";

                if(posX.text != cursor.transform.position.x.ToString())
                    posX.text = "";
                if(posY.text != cursor.transform.position.y.ToString())
                    posY.text = "";

                _multipleSelected = true;
            }
            else
            {
                nameText.text = cursor.name;
                posX.text = cursor.transform.position.x.ToString();
                posY.text = cursor.transform.position.y.ToString();
            }
        }
    }

    public void UpdateCursorInfo(Editor_CursorBase c)
    {
        if(Editor_CursorBase.selectedCursorList.Count > 0 && !_multipleSelected)
        {
            var cursor = Editor_CursorBase.selectedCursorList[0];
            nameText.text = cursor.name;
            posX.text = cursor.transform.position.x.ToString();
            posY.text = cursor.transform.position.y.ToString();
        }
    }

    public void CursorMoveEvent(RectTransform rect)
    {
        if(hideCursor.isOn)
            return;
            
        if(_cursorClicked && Editor_CursorBase.selectedCursorList.Count != 0)
        {
            Vector2 pos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - clickedPos;
            Editor_CursorBase.MoveSelecetedCursor(pos);
        }
    }

    public void KeyUpEvent(RectTransform rect)
    {
        if(_cursorClicked)
        {
            Editor_CursorBase.UpdateCenterPosition();
        }
    }

    public void CursorSelectEvent(RectTransform rect)
    {
        if(hideCursor.isOn)
            return;
            
        clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool multiSelect = Input.GetKey(KeyCode.LeftShift);
        bool uiLock = false;
        _cursorClicked = false;
        _multipleSelected = false;

        foreach(var cursor in cursorList)
        {
            if(cursor.PointInCollider(clickedPos))
            {
                if(multiSelect)
                {
                    Editor_CursorBase.SelectMultipleCursor(cursor);
                }
                else
                {
                    Editor_CursorBase.SelectSingleCursor(cursor);
                }

                UpdateCursorInfo(cursor,multiSelect);

                _cursorClicked = true;
                uiLock = true;

                break;
            }
        }

        UIActive(uiLock);

        if(!_cursorClicked)
        {
            Editor_CursorBase.DeselectAll();
        }
    }
}
