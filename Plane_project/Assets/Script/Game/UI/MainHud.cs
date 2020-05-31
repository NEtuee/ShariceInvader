using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHud : SingletonMono<MainHud>
{
    public SpriteRenderer wpIcon;
    public SpriteRenderer gague;
    public SpriteRenderer hpBar;
    public SpriteRenderer mainUI;

    public Transform mainUITransform;

    public Material scaleBarMat;

    public Sprite nullUI;

    private Material gagueMat;
    private Material hpMat;

    private LineRenderer _distLine;
    private ObjectBase _distTarget;

    private PlaneBase _followTarget;

    protected AnimationControllEx _uiAni;


    void Awake()
    {
        SetSingleton(this);
        gagueMat = gague.material;
        gagueMat.SetFloat("_Progress",1f);

        hpMat = hpBar.material;
        hpMat.SetFloat("_Progress",1f);

        _distLine = gameObject.AddComponent<LineRenderer>();
        _uiAni = new AnimationControllEx(mainUI);

        _distLine.startWidth = 0.02f;
        _distLine.endWidth = 0.02f;
        _distLine.material = ResourceManager.GetInstance().GetPixelSnapMaterial();

        _distLine.enabled = false;


        _uiAni.AddEmptyAnimation("MainAttack");
        _uiAni.AddEmptyAnimation("DriveOn");
        _uiAni.AddEmptyAnimation("DriveAttack");
        _uiAni.AddEmptyAnimation("Change");

        _uiAni.Stop();
    }

    public void Initiailize()
    {
        _followTarget = Player.instance;
        _followTarget.hpChangeEvent += UpdateHpBar;

    }

    public void Progress(float deltaTime)
    {
        if(_followTarget == null || _followTarget.deleted)
            return;
        Vector3 velo = _followTarget.velocity;
        Vector3 dir = -(velo.magnitude > 1f ? velo.normalized : velo);

        _uiAni.AnimationProgress(deltaTime);
        
        transform.position = _followTarget.position + dir * 0.1f;
        mainUITransform.transform.position = _followTarget.position + dir * 0.12f;
    }

    public void UpdateGague(float g)
    {
        gagueMat.SetFloat("_Progress",g);
    }

    public void UpdateHpBar()
    {
        hpMat.SetFloat("_Progress",(float)_followTarget._hp / (float)_followTarget.maxHp);
    }

    public void UpdateScaleBar(float val)
    {
        scaleBarMat.SetFloat("_Offset",val + 0.5f);
    }

    public void MainUIAni(string name,bool loop = false)
    {
        if(_uiAni.AnimationExist(name))
            _uiAni.ChangeAni(name,loop);
        else
            _uiAni.Stop();
    }

    public void MainUIAniSwap(string name, string path, int type)
    {
        _uiAni.CopyAnimation(name,path);
    }

    public void MainUIAniSwap(string name, AnimationControllEx.AnimationKey[] key)
    {
        _uiAni.CopyAnimation(name,key);
    }

    public void WeaponChange(Sprite icon, Sprite ui)
    {
        wpIcon.sprite = icon;
        //mainUI.sprite = ui == null ? nullUI : ui;

        MainUIAni("Change");
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
