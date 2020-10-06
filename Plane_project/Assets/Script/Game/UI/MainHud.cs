using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHud : SingletonMono<MainHud>
{
    public SpriteRenderer wpIcon;
    public SpriteRenderer[] weaponGagues;
    public SpriteRenderer shieldDamange;
    public SpriteFontTextMesh hpBar;
    public SpriteRenderer mainUI;
    public GameObject waveIcon;

    public SpriteRenderer wave_0;
    public SpriteFontTextMesh wave_1;

    public GameObject hpContainer;
    public GameObject hpGagueContainer;
    public SpriteRenderer hpGague;
    public SpriteRenderer[] weaponGagueContainer;

    public Transform mainUITransform;
    public Transform minimapScreenIndicator;
    public Transform groundLine;

    public Material scaleBarMat;

    public Sprite nullUI;


    public GlitchEffect uiGlitch;

    private Material[] gagueMats;

    private LineRenderer _distLine;
    private ObjectBase _distTarget;

    private PlaneBase _followTarget;

    protected AnimationControllEx _uiAni;
    protected AnimationControllEx _shieldAni;
    protected AnimationKeyEvent _shieldKeyEvent;

    private float _hpBarDisapear = 0f;
    private float _waveTimer = 0f;
    private float _waveTime = 0f;

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
        _shieldAni.AddAnimation("FirstHit","UI/MainHud/HP/FirstHit");
        _shieldAni.AddAnimation("Hit","UI/MainHud/HP/Hit");
        _shieldAni.AddAnimation("Recover","UI/MainHud/HP/Recover");
        _shieldAni.AddAnimation("Closing","UI/MainHud/HP/Closing");

        _shieldKeyEvent = new AnimationKeyEvent();
        CreateShieldAnimationkeyEvent();






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
        var pos = CameraControll.instance.position;
        pos.y = 0f;
        pos.z = 0f;
        groundLine.transform.position = pos;

        if(_followTarget == null || _followTarget.deleted)
            return;
            
        Vector3 velo = _followTarget.velocity;
        Vector3 dir = -(velo.magnitude > 1f ? velo.normalized : velo);

        _uiAni.AnimationProgress(Timer.noneScaledDeltaTime);
        int frame = _shieldAni.AnimationProgress(Timer.noneScaledDeltaTime);
        _shieldKeyEvent.EventEntry(_shieldAni.currAni,frame);
        
        transform.position = _followTarget.position + dir * 0.13f;
        mainUITransform.transform.position = _followTarget.position + dir * 0.1f;

        if(_shieldAni.currAni == "Closing" && _shieldAni.isEnd && hpContainer.activeInHierarchy)
        {
            hpContainer.SetActive(false);
        }

        // if(_hpBarDisapear != 0f)
        // {
        //     _hpBarDisapear -= deltaTime;
        //     if(_hpBarDisapear <= 0f)
        //     {
        //         _hpBarDisapear = 0f;
        //         hpContainer.SetActive(false);
        //     }
        // }

        if(_waveTimer != 0f)
        {
            float a = Mathf.Lerp(0,1,_waveTimer / (_waveTime * .15f));

            var c = wave_0.color;
            c.a = a;
            wave_0.color = c;
            c = wave_1.textColor;
            c.a = a;
            wave_1.textColor = c;
            wave_1.UpdateColor();


            _waveTimer -= deltaTime;
            if(_waveTimer <= 0f)
            {
                _waveTimer = 0f;
                waveIcon.SetActive(false);
            }
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            ShowWaveIcon(3f);
        }

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
                var color = gagueMats[i].GetColor("_RestColor");
                weaponGagueContainer[i].transform.localScale = Vector3.Lerp(scale,new Vector3(1f,0.15f,1f),0.2f);
                gagueMats[i].SetColor("_RestColor",Color.Lerp(color,new Color32(217,22,70,255),0.2f));


                var restColor = weaponGagueContainer[i].material.GetColor("_RestColor");
                weaponGagueContainer[i].material.SetColor("_RestColor",Color.Lerp(restColor,new Color32(217,22,70,0),0.2f));
            }
            else
            {
                var scale = weaponGagueContainer[i].transform.localScale;
                var color = gagueMats[i].GetColor("_RestColor");
                weaponGagueContainer[i].transform.localScale = Vector3.Lerp(scale,new Vector3(1f,1f,1f),0.2f);
                gagueMats[i].SetColor("_RestColor",Color.Lerp(color,Color.white,0.2f));

                var restColor = weaponGagueContainer[i].material.GetColor("_RestColor");
                weaponGagueContainer[i].material.SetColor("_RestColor",Color.Lerp(restColor,new Color32(217,22,70,255),0.2f));
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
        //hpMat.SetFloat("_Progress",(float)_followTarget._hp / (float)_followTarget.maxHp);
        if(_followTarget._hp == _followTarget.maxHp)
        {
            if(_hpBarDisapear == 0f && hpContainer.activeInHierarchy)
            {
                _shieldAni.ChangeAni("Closing",false);
            }
        }
        else
        {
            _hpBarDisapear = 0f;
            if(!hpContainer.activeInHierarchy)
            {
                hpContainer.SetActive(true);
            }
            
        }

        var per = (float)_followTarget._hp / (float)_followTarget.maxHp;
        var hp = (int)((per) * 100f);
        
        hpBar.SetText(hp.ToString());
        hpGague.material.SetFloat("_Progress",per);
    }

    public void ShowWaveIcon(float time)
    {
        waveIcon.SetActive(true);
        SoundManager.instance.Play("SE/WaveGlitch",false,-1,1f,false);

        var c = wave_0.color;
        c.a = 1f;
        wave_0.color = c;
        c = wave_1.textColor;
        c.a = 1f;
        wave_1.textColor = c;
        wave_1.UpdateColor();

        _waveTimer = _waveTime = time;
        ActiveUIGlitch(.774f,.994f,.862f,.0253f,time + .5f);
    }

    public void ShieldDamage()
    {
        if(hpContainer.activeInHierarchy)
            _shieldAni.ChangeAni("Hit",false);
        else
            _shieldAni.ChangeAni("FirstHit",false);
    }

    public void ShieldRecover()
    {
        if(_shieldAni.currAni != "Recover")
            _shieldAni.ChangeAni("Recover",false);
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

    public void CreateShieldAnimationkeyEvent()
    {
        _shieldKeyEvent.AddActiveEvent("FirstHit",0,hpGagueContainer.transform,false);
        _shieldKeyEvent.AddActiveEvent("FirstHit",7,hpGagueContainer.transform,true);
        _shieldKeyEvent.AddTranslateEvent("FirstHit",7,hpGagueContainer.transform,new Vector3(0f,-0.37f));
        _shieldKeyEvent.AddTranslateEvent("FirstHit",9,hpGagueContainer.transform,new Vector3(0f,-0.38f));

        _shieldKeyEvent.AddActiveEvent("FirstHit",0,hpBar.transform,false);
        _shieldKeyEvent.AddActiveEvent("FirstHit",7,hpBar.transform,true);
        _shieldKeyEvent.AddTranslateEvent("FirstHit",7,hpBar.transform,new Vector3(0.67f,-0.60f));
        _shieldKeyEvent.AddTranslateEvent("FirstHit",9,hpBar.transform,new Vector3(0.67f,-0.61f));

        _shieldKeyEvent.AddActiveEvent("Hit",0,hpGagueContainer.transform,true);
        _shieldKeyEvent.AddTranslateEvent("Hit",0,hpGagueContainer.transform,new Vector3(0f,-.4f));
        _shieldKeyEvent.AddTranslateEvent("Hit",1,hpGagueContainer.transform,new Vector3(0f,-.34f));
        _shieldKeyEvent.AddTranslateEvent("Hit",2,hpGagueContainer.transform,new Vector3(0f,-.39f));
        _shieldKeyEvent.AddTranslateEvent("Hit",3,hpGagueContainer.transform,new Vector3(0f,-.36f));
        _shieldKeyEvent.AddTranslateEvent("Hit",4,hpGagueContainer.transform,new Vector3(0f,-.37f));
        _shieldKeyEvent.AddTranslateEvent("Hit",5,hpGagueContainer.transform,new Vector3(0f,-.38f));
        
        _shieldKeyEvent.AddActiveEvent("Hit",0,hpBar.transform,false);
        _shieldKeyEvent.AddActiveEvent("Hit",1,hpBar.transform,true);
        _shieldKeyEvent.AddTranslateEvent("Hit",1,hpBar.transform,new Vector3(.67f,-.57f));
        _shieldKeyEvent.AddTranslateEvent("Hit",2,hpBar.transform,new Vector3(.67f,-.59f));
        _shieldKeyEvent.AddTranslateEvent("Hit",3,hpBar.transform,new Vector3(.67f,-.60f));
        _shieldKeyEvent.AddTranslateEvent("Hit",4,hpBar.transform,new Vector3(.67f,-.61f));


        _shieldKeyEvent.AddActiveEvent("Recover",0,hpGagueContainer.transform,true);
        _shieldKeyEvent.AddTranslateEvent("Recover",0,hpGagueContainer.transform,new Vector3(0f,-.36f));
        _shieldKeyEvent.AddTranslateEvent("Recover",1,hpGagueContainer.transform,new Vector3(0f,-.37f));
        _shieldKeyEvent.AddTranslateEvent("Recover",2,hpGagueContainer.transform,new Vector3(0f,-.38f));
        
        _shieldKeyEvent.AddActiveEvent("Recover",0,hpBar.transform,true);
        _shieldKeyEvent.AddTranslateEvent("Recover",0,hpBar.transform,new Vector3(.67f,-.57f));
        _shieldKeyEvent.AddTranslateEvent("Recover",1,hpBar.transform,new Vector3(.67f,-.59f));
        _shieldKeyEvent.AddTranslateEvent("Recover",2,hpBar.transform,new Vector3(.67f,-.60f));
        _shieldKeyEvent.AddTranslateEvent("Recover",3,hpBar.transform,new Vector3(.67f,-.61f));

        _shieldKeyEvent.AddActiveEvent("Closing",0,hpGagueContainer.transform,false);
        _shieldKeyEvent.AddActiveEvent("Closing",0,hpBar.transform,false);
    }

}
