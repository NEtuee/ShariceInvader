using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor_CursorBase : MonoBehaviour
{
    public static List<Editor_CursorBase> selectedCursorList = new List<Editor_CursorBase>();

    public SpriteRenderer mainRenderer;
    public Collider2D mainCollider;
    public bool selected = false;

    public Vector3 centerPos;

    public static void DeselectAll()
    {
        foreach(var cursor in selectedCursorList)
        {
            cursor.Deselect();
        }

        selectedCursorList.Clear();
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
        cursor.Select();
        selectedCursorList.Add(cursor);
    }

    public static void MoveSelecetedCursor(Vector3 dist)
    {
        foreach(var cursor in selectedCursorList)
        {
            cursor.transform.position = cursor.centerPos + dist;
        }
    }

    public virtual void InitCursor()
    {
        mainRenderer = GetComponent<SpriteRenderer>();
        mainCollider = GetComponent<Collider2D>();
    }

    public bool PointInCollider(Vector2 point)
    {
        return mainCollider.OverlapPoint(point);
    }

    public void Select()
    {
        centerPos = transform.position;
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }

    public Vector3 GetPosition() {return transform.position;}
    public Vector3 GetSize() {return transform.localScale;}
    public float GetZAngle() {return transform.eulerAngles.z;}
}
