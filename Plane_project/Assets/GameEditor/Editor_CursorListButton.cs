using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_CursorListButton : MonoBehaviour
{
    public Text title;
    public RectTransform rect;
    public Button button;
    public Editor_CursorBase cursor;


    private bool multi = false;

    public void Init(Editor_CursorBase c)
    {
        cursor = c;

        title.text = c.name;
        button.image.color = Color.white;
    }

    public void BUttonProgress()
    {
        multi = Input.GetKey(KeyCode.LeftShift);
        
        if(!cursor.selected)
            Select();
        else
            cursor.Deselect();

        Editor_CursorController.instance.UpdateCursorInfo(cursor,multi);
        Editor_CursorController.instance.UIActive(Editor_CursorBase.selectedCursorList.Count != 0);
        Editor_CursorController.instance.UpdateButtonColor();
    }

    public void Select()
    {
        if(multi)
        {
            Editor_CursorBase.SelectMultipleCursor(cursor);
        }
        else
        {
            Editor_CursorBase.SelectSingleCursor(cursor);
        }

        button.image.color = Color.green;
    }

    public void Deselect()
    {
        button.image.color = Color.white;
    }
}
