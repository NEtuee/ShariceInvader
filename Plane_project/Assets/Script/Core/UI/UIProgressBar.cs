using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProgressBar : UISelectButton
{
    public UnityEngine.Events.UnityEvent whenValueChange = new UnityEngine.Events.UnityEvent();

    public float progressStart = 0f;
    public float progressEnd = 1f;
    public float progressIncreaseValue = 0.1f;

    public float progressValue{get{return progressEnd * progress;}}
    public float progress{set
    {
        SetValue(value);
        whenValueChange.Invoke();
    } get{return _progress;}}


    public SpriteRenderer barCage;
    public SpriteRenderer progressBar;
    public Sprite baseSprite;
    public Sprite selectSprite;

    private Material _material;
    private float _progress = 0f;
    private bool _bind = false;

    public override void Initialize()
    {
        base.Initialize();
        _material = new Material(Shader.Find("Custom/Default_ProgressBar"));
        _material.SetFloat("PixelSnap",1f);
        
        progressBar.material = _material;
        progressBar.sprite = baseSprite;
        UpdateMaterialValue();
    }

    public override void Progress(float deltaTime)
    {
        if(_bind)
        {
            if(ControllerEx.GetInstance().KeyDown("Left"))
            {
                var factor = _progress - progressIncreaseValue;
                progress = factor < 0f ? 0f : factor;
            }
            else if(ControllerEx.GetInstance().KeyDown("Right"))
            {
                var factor = _progress + progressIncreaseValue;
                progress = factor >= 1f ? 1f : factor;
            }
        }
    }

    public void SetValue(float p)
    {
        _progress = p;
        UpdateMaterialValue();
    }

    public override void ColorSync(Color color)
    {
        var col = barCage.color;
        col.a = color.a;
        barCage.color = col;

        col = progressBar.color;
        col.a = color.a;
        progressBar.color = col;
    }

    public override void SelectEvent()
    {
        _bind = true;

        manager.uiSelectLock = true;
        progressBar.sprite = selectSprite;
    }

    public override void DeselectEvent()
    {
        _bind = false;

        manager.uiSelectLock = false;
        progressBar.sprite = baseSprite;
    }

    public void UpdateMaterialValue()
    {
        _material.SetFloat("_Progress",progress);
    }

}
