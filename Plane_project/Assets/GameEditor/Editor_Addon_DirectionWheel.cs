using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_Addon_DirectionWheel : MonoBehaviour
{
    public InputField x;
    public InputField y;
    public RectTransform wheel;

    public Vector2 direction = Vector2.left;
    private Vector2 _origin;

    private bool _active = false;

    public void Start()
    {
        Editor_EventSystem.instance.clickEvent += ClickEvent;
        Editor_EventSystem.instance.uiMouseMoveEvent += MouseMoveEvent;
        Editor_EventSystem.instance.keyUpEvent += MouseUpEvent;

        direction = Vector2.left;
        UpdateValue();
        wheel.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(direction));
    }

    public void ClickEvent(RectTransform rect)
    {
        if(rect == wheel)
        {

            _origin = direction;
            _active = true;
        }
    }

    public void MouseMoveEvent(RectTransform rect)
    {
        if(_active)
        {
            Vector2 curr = (Input.mousePosition - wheel.position).normalized;

            direction = curr;
            wheel.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(direction));
            UpdateValue();
        }
    }

    public void MouseUpEvent(RectTransform rect)
    {
        _active = false;
    }

    public void UpdateValue()
    {
        x.text = direction.x.ToString();
        y.text = direction.y.ToString();
    }

    public void SetValue()
    {
        direction.x = float.Parse(x.text);
        direction.y = float.Parse(y.text);

        wheel.eulerAngles = new Vector3(0f,0f,MathEx.directionToAngle(direction));
    }
}
