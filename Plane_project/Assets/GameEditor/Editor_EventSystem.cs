using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Editor_EventSystem : SingletonMono<Editor_EventSystem>
{
    public delegate void clickEventBase(RectTransform r);
    public enum MouseState
    {
        CameraMove,
        UIControll,
    };

    public GraphicRaycaster raycaster;
    public Image noticeImage;
    public Text noticeText;
    public MouseState mouseState;

    public clickEventBase clickEvent = new clickEventBase((RectTransform t)=>{});

    private Vector2 _clickedPos;
    private Vector2 _currScreenPos;
    private Vector2 _currWorldPos;

    private List<RaycastResult> _results = new List<RaycastResult>();

    private float _noticeTimer;
    private float _noticeTimerOrigin;

    private Color _noticeImageColor;
    private Color _noticeTextColor;

    RectTransform selectedTarget;
    Editor_UIBase selectedUI;
    Editor_CameraControllBase cam;
    
    public void Awake()
    {
        SetSingleton(this);
        cam = GetComponent<Editor_CameraControllBase>();

        _noticeImageColor = noticeImage.color;
        _noticeTextColor = noticeText.color;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickedPos = _currScreenPos = Input.mousePosition;

            switch(mouseState)
            {
            case MouseState.CameraMove:
                CamControllClick();
            break;
            case MouseState.UIControll:
                UIRaycast();

                if(selectedTarget != null)
                    clickEvent(selectedTarget);
                if(selectedUI != null)
                    selectedUI.Select();
            break;
            };

        }
        else if(Input.GetKey(KeyCode.Mouse0))
        {
            _currScreenPos = Input.mousePosition;

            switch(mouseState)
            {
            case MouseState.CameraMove:
                CamControllMove();
            break;
            case MouseState.UIControll:
                if(selectedUI != null)
                    selectedUI.Move(GetMouseMovePosition());
            break;
            };

        
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            //selectedTarget = null;

            KeyUpEvents();
        }

        if(selectedTarget == null)
            CamControllWheel();
        HotKeyCheck();

        if(_noticeTimer != 0f)
        {
            _noticeTimer -= Time.deltaTime;
            Color i = _noticeImageColor;
            Color j = _noticeTextColor;
            i.a = 0f;
            j.a = 0f;

            float factor = (_noticeTimerOrigin - _noticeTimer) / _noticeTimerOrigin;

            noticeImage.color = Color.Lerp(_noticeImageColor,i,factor);
            noticeText.color = Color.Lerp(_noticeTextColor,j,factor);

            if(_noticeTimer <= 0f)
            {
                _noticeTimer = 0f;

                noticeImage.color = _noticeImageColor;
                noticeText.color = _noticeTextColor;

                noticeImage.gameObject.SetActive(false);
                noticeText.gameObject.SetActive(false);
            }
        }
    }

    public void ActiveNotice(string title)
    {
        _noticeTimer = _noticeTimerOrigin = 3f;
                
        noticeImage.color = _noticeImageColor;
        noticeText.color = _noticeTextColor;

        noticeImage.gameObject.SetActive(true);
        noticeText.gameObject.SetActive(true);

        noticeText.text = title;
    }

    public void HotKeyCheck()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            ChangeMouseState(MouseState.CameraMove);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            ChangeMouseState(MouseState.UIControll);
        }
    }

    public void ChangeMouseState(MouseState state)
    {
        KeyUpEvents();

        mouseState = state;
        selectedTarget = null;

        switch(mouseState)
        {
        case MouseState.CameraMove:

        break;
        case MouseState.UIControll:
            
        break;
        };
    }

    public void KeyUpEvents()
    {
        switch(mouseState)
        {
        case MouseState.CameraMove:

        break;
        case MouseState.UIControll:
            if(selectedUI != null)
            {
                selectedUI.Release();
                selectedUI = null;
            }

            _results.Clear();
        break;
        };
    }

    public Vector2 GetMouseMovePosition()
    {
        return _currScreenPos - _clickedPos;
    }

    public void UIRaycast()
    {
        var ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        raycaster.Raycast(ped, _results);

        if(_results.Count != 0)
        {
            selectedTarget = _results[0].gameObject.GetComponent<RectTransform>();

            var target = _results[0].gameObject.GetComponent<Editor_UIBase>();
            if(target != null)
            {
                selectedUI = target;
                selectedTarget.SetAsLastSibling();
            }
        }
        else
        {
            selectedTarget = null;
            selectedUI = null;
        }
    }

    public void CamControllClick()
    {
        cam.MovePosStart(_clickedPos);
    }

    public void CamControllMove()
    {
        cam.MovePosCenterBase(_currScreenPos);
    }

    public void CamControllWheel()
    {
        cam.AddSize(-Input.mouseScrollDelta.y * 0.5f);
    }


}
