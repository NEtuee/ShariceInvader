using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class UISelectBase : MonoBehaviour
{

    [HideInInspector]
    public TextMesh mainText;
    [HideInInspector]
    public BoxCollider2D coll;
    [HideInInspector]
    public float uiWidth;
    [HideInInspector]
    public float uiHeight;
    [HideInInspector]
    public Transform tp;
    [HideInInspector]
    public UISelectBase parentUI;
    [HideInInspector]
    public UISortManager manager;

    public UISelectBase[] menuItems;

    public Color mainTextColor = Color.red;
    public Color selectTextColor = Color.white;

    public Vector3 menuDist = new Vector3(0f,0.1f);

    public int menuOrder = 0;

    public bool isSelected = false;
    public bool isOverlap = false;

    protected float _colliderMinY;

    public virtual void Initialize()
    {
        mainText = GetComponent<TextMesh>();
        coll = GetComponent<BoxCollider2D>();
        tp = transform;

        _colliderMinY = coll.size.y * 0.5f + coll.offset.y;

        UISizeCalc();
    }

    public virtual void Progress(float deltaTime){}

    public void UISizeCalc()
    {
        uiWidth = coll.size.x * 0.5f + coll.offset.x;
        uiHeight = coll.size.y * 0.5f + coll.offset.y;
    }

    public virtual void ColorSync(Color color){}

    public virtual void SelectAction(bool selectEvent = true)
    {
        if(isSelected)
        {
            Deselect(selectEvent);
        }
        else
        {
            Select(selectEvent);
        }
    }

    public virtual void Select(bool selectEvent = true)
    {
        isSelected = true;

        mainText.color = selectTextColor;

        if(selectEvent)
            SelectEvent();
    }

    public virtual void Deselect(bool selectEvent = true)
    {
        isSelected = false;

        mainText.color = mainTextColor;

        if(selectEvent)
            DeselectEvent();
    }

    public abstract void SelectEvent();
    public abstract void DeselectEvent();
    public abstract void UICloseEvent();

    public Bounds GetBounds() {return coll.bounds;}

    public bool MouseOvelapCheck(Vector2 point)
    {
        return coll.OverlapPoint(point);
    }

}
