using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISelectButton : UISelectBase
{
    public UnityEvent selectEvent = new UnityEvent();
    public UnityEvent deselectEvent = new UnityEvent();

    public SpriteRenderer buttonGraphic;
    public SpriteFontTextMesh buttonText;

    public bool isToggle = false;
    public bool deselectLock = false;

    public void SetButtonSprite(Sprite spr) {buttonGraphic.sprite = spr;}
    public void SetButtonText(string s) {buttonText.SetText(s);}

    public override void Deselect(bool selectEvent = true)
    {
        if(isSelected && deselectLock)
            return;
        
        base.Deselect(selectEvent);
    }

    public override void SelectEvent()
    {
        selectEvent.Invoke();
    }

    public override void DeselectEvent()
    {
        deselectEvent.Invoke();
    }

    public override void UICloseEvent(){}
}
