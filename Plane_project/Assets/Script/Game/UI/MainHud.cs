using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHud : SingletonMono<MainHud>
{
    public SpriteRenderer wpIcon;
    public SpriteRenderer[] weaponGagues;
    public SpriteRenderer shieldDamange;
    public SpriteRenderer hpBar;
    public SpriteRenderer mainUI;
    public GameObject waveText;

    public GameObject hpContainer;
    public SpriteRenderer[] weaponGagueContainer;

    public Transform mainUITransform;
    public Transform minimapScreenIndicator;
    public Transform groundLine;

    public Material scaleBarMat;

    public Sprite nullUI;


    public GlitchEffect uiGlitch;

    private Material[] gagueMats;
    private Material hpMat;

    private LineRenderer _distLine;
    private ObjectBase _distTarget;

    private PlaneBase _followTarget;

    protected AnimationControllEx _uiAni;
    protected AnimationControllEx _shieldAni;

    private float _hpBarDisapear = 0f;

    private int _currWeapon;


    void Awake()
    {
        SetSingleton(this);

        gagueMats = new Material[weaponGagues.Length];
        for(int i = 0; i < weaponGagues.Length; ++i)
        {
            gagueMats[i] = weaponGagues[i].material;
            gagueMats[i].SetFloat("_Progress",1f);
        }

        hpMat = hpBar.material;
        hpMat.SetFloat("_Progress",1f);

        _distLine = gameObject.AddComponent<LineRenderer>();
        _uiAni = new AnimationControllEx(mainUI);

        _distLine.startWidth = 0.02f;
        _distLine.endWidth = 0.02f;
        _distLine.material = ResourceManager.GetInstance().GetPixelSnapMaterial();

        _distLine.enabled = false;


        _uiAni.AddEmptyAnimation("MainAttack");
        _uiAni.AddEmptyAnimation("Boost");
        _uiAni.AddEmptyAnimation("DriveOn");
        _uiAni.AddEmptyAnimation("DriveAttack");
        _uiAni.AddEmptyAnimation("Change");

        _uiAni.Stop();

        _shieldAni = new AnimationControllEx(shieldDamange);
        _shieldAni.AddAnimation("Active","UI/MainHud/ShieldDamage");

        mainUITransform.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Initiailize()
    {
        _followTarget = Player.instance;
        _followTarget.hpChangeEvent += UpdateHpBar;

        UpdateMinimapScreenIndicator();

        mainUITransform.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Progress(float deltaTime)
    {
        if(_followTarget == null || _followTarget.deleted)
            return;
        Vector3 velo = _followTarget.velocity;
        Vector3 dir = -(velo.magnitude > 1f ? velo.normalized : velo);

        _uiAni.AnimationProgress(Timer.noneScaledDeltaTime);
        _shieldAni.AnimationProgress(Timer.noneScaledDeltaTime);
        
        transform.position = _followTarget.position + dir * 0.1f;
        mainUITransform.transform.position = _followTarget.position + dir * 0.12f;

        if(_hpBarDisapear != 0f)
        {
            _hpBarDisapear -= deltaTime;
            if(_hpBarDisapear <= 0f)
            {
                _hpBarDisapear = 0f;
                hpContainer.SetActive(false);
            }
        }

        var pos = CameraControll.instance.position;
        pos.y = 0f;
        pos.z = 0f;
        groundLine.transform.position = pos;

        WeaponGaguePositionUpdate(deltaTime);
    }

    public void ActiveUIGlitch(float time)
    {
        ActiveUIGlitch(.474f,.694f,.562f,.0453f,time);
    }

    public void ActiveUIGlitch(float inten, float flip, float col, float flic, float time)
    {
        uiGlitch.Active(inten,flip,col,flic,time);
    }

    public void UpdateMinimapScreenIndicator()
    {
        var scale = minimapScreenIndicator.localScale;
        scale.x = .8f * (.8f / (ObjectManager.GetInstance()._place._mapWidth * 0.1f));

        minimapScreenIndicator.localScale = scale;
    }

    public void WeaponGaguePositionUpdate(float deltaTime)
    {
        for(int i = 0; i < weaponGagueContainer.Length; ++i)
        {
            if(i != _currWeapon)
            {
                var scale = weaponGagueContainer[i].transform.localScale;
                weaponGagueContainer[i].transform.localScale = Vector3.Lerp(scale,new Vector3(1f,0.15f,1f),0.2f);
            }
            else
            {
                var scale = weaponGagueContainer[i].transform.localScale;
                weaponGagueContainer[i].transform.localScale = Vector3.Lerp(scale,new Vector3(1f,1f,1f),0.2f);
            }

            if(i > 0)
            {
                float pos = weaponGagueContainer[i - 1].bounds.min.y;
                pos -= 0.03f;// * weaponGagueContainer[i - 1].transform.localScale.y;
                weaponGagueContainer[i].transform.position = new Vector3(weaponGagueContainer[i - 1].transform.position.x,pos,0f);
            }
        }
    }

    public void SetCurrWeapon(int val) {_currWeapon = val;}

    public void UpdateGague(float g,int target)
    {
        gagueMats[target].SetFloat("_Progress",g);
    }

    public void UpdateHpBar()
    {
        hpMat.SetFloat("_Progress",(float)_followTarget._hp / (float)_followTarget.maxHp);
        if(_followTarget._hp == _followTarget.maxHp)
        {
            if(_hpBarDisapear == 0f && hpContainer.activeInHierarchy)
                _hpBarDisapear = 1f;
        }
        else
        {
            _hpBarDisapear = 0f;
            if(!hpContainer.activeInHierarchy)
            {
                hpContainer.SetActive(true);
                _shieldAni.ChangeAni("Active",false);
            }
            
        }
    }

    public void UpdateScaleBar(float val)
    {
        scaleBarMat.SetFloat("_Offset",val + 0.5f);
    }

    public void MainUIAni(string name,bool loop = false)
    {
        if(_uiAni.AnimationExist(name))
        {
            _uiAni.ChangeAni(name,loop);
        }
        else
        {
            _uiAni.SetSpriteNull();
        }
    }

    public void MainUIAniSwap(string name, string path)
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
        _uiAni.Stop();
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
