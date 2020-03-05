using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHud : SingletonMono<MainHud>
{
    public SpriteRenderer wpIcon;
    public SpriteRenderer gague;
    public SpriteRenderer mainUI;

    public Sprite nullUI;

    private Material gagueMat;

    private LineRenderer _distLine;
    private ObjectBase _distTarget;

    void Awake()
    {
        SetSingleton(this);
        gagueMat = gague.material;
        gagueMat.SetFloat("_Progress",1f);
    
        _distLine = gameObject.AddComponent<LineRenderer>();

        _distLine.startWidth = 0.02f;
        _distLine.endWidth = 0.02f;
        _distLine.material = ResourceManager.GetInstance().GetMaterial("SpriteDefault");

        _distLine.enabled = false;
    }

    public void UpdateGague(float g)
    {
        gagueMat.SetFloat("_Progress",g);
    }

    public void WeaponChange(Sprite spr, Sprite ui)
    {
        wpIcon.sprite = spr;
        mainUI.sprite = ui == null ? nullUI : ui;
        UpdateGague(1f);
    }

    public void SetNull()
    {
        wpIcon.sprite = null;
        mainUI.sprite = nullUI;
    }

    public void SetDistTarget(ObjectBase obj)
    {
        _distTarget = obj;
        _distLine.enabled = _distTarget != null;
    }

    public void UpdateDistLine(ObjectBase mainObj)
    {
        if(_distTarget != null)
        {
            _distLine.SetPosition(0,mainObj.position);
            _distLine.SetPosition(1,_distTarget.position);
        }
        else
        {
            _distLine.enabled = false;
        }
    }

}
