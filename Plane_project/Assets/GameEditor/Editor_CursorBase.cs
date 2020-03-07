using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor_CursorBase : MonoBehaviour
{
    public delegate void cursorValueChanged(Editor_CursorBase cursor);
    public static List<Editor_CursorBase> selectedCursorList = new List<Editor_CursorBase>();
    public static cursorValueChanged cursorValueChangedEvent = new cursorValueChanged((Editor_CursorBase cursor)=>{});
    public static cursorValueChanged cursorSelectedEvent = new cursorValueChanged((Editor_CursorBase cursor)=>{});
    public static cursorValueChanged cursorDeselectedEvent = new cursorValueChanged((Editor_CursorBase cursor)=>{});
    public SpriteRenderer mainRenderer;
    public Collider2D mainCollider;
    public bool selected = false;

    public int uniqueNumber;

    public Vector3 centerPos;

    public static void DeselectAll()
    {
        foreach(var cursor in selectedCursorList)
        {
            cursor.DeselectProgress();
        }

        cursorDeselectedEvent(null);

        selectedCursorList.Clear();
    }
    public static void DeselectCursor(Editor_CursorBase cursor)
    {
        selectedCursorList.Remove(cursor);
        cursorDeselectedEvent(cursor);
    }
    public static void SetXValue(float x)
    {
        foreach(var cursor in selectedCursorList)
        {
            Vector2 pos = cursor.transform.position;
            pos.x = x;
            cursor.transform.position = pos;

            cursorValueChangedEvent(cursor);
        }
    }
    public static void SetYValue(float y)
    {
        foreach(var cursor in selectedCursorList)
        {
            Vector2 pos = cursor.transform.position;
            pos.y = y;
            cursor.transform.position = pos;

            cursorValueChangedEvent(cursor);
        }
    }
    public static void SelectSingleCursor(Editor_CursorBase cursor)
    {
        DeselectAll();
        SelectCursor(cursor);
    }

    public static void SelectMultipleCursor(Editor_CursorBase cursor)
    {
        SelectCursor(cursor);
    }

    public static void SelectCursor(Editor_CursorBase cursor)
    {
        if(cursor.selected)
        {
            cursor.Deselect();
        }
        else
        {
            cursor.Select();
            selectedCursorList.Add(cursor);
            cursorSelectedEvent(cursor);
        }
    }

    public static void MoveSelecetedCursor(Vector3 dist)
    {
        foreach(var cursor in selectedCursorList)
        {
            cursor.SetPosition(cursor.centerPos + dist);
        }
    }

    public static void UpdateCenterPosition()
    {
        foreach(var cursor in selectedCursorList)
        {
            cursor.centerPos = cursor.transform.position;
        }
    }

    public virtual void firstSetting()
    {
        mainRenderer = GetComponent<SpriteRenderer>();
        mainCollider = GetComponent<Collider2D>();
    }

    public virtual void initCursor()
    {
        selected = false;
        ChangeColor(1f,1f,1f);
    }

    public bool PointInCollider(Vector2 point)
    {
        return mainCollider.OverlapPoint(point);
    }

    public void Select()
    {
        centerPos = transform.position;
        selected = true;

        ChangeColor(0f,1f,0f);
    }

    public void Deselect()
    {
        if(selected)
        {
            DeselectProgress();
            DeselectCursor(this);
        }
    }

    public void DeselectProgress()
    {
        selected = false;
        ChangeColor(1f,1f,1f);
    }

    public void ChangeColor(float r, float g, float b)
    {
        mainRenderer.color = new Color(r,g,b,0.3f);
    }

    public void SetPosition(Vector2 pos) {transform.position = pos; cursorValueChangedEvent(this);} 
    public Vector3 GetPosition() {return transform.position;}
    public Vector3 GetSize() {return transform.localScale;}
    public float GetZAngle() {return transform.eulerAngles.z;}
}
