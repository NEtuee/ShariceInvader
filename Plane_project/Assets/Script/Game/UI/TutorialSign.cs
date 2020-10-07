using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialSign : Drawable
{
    public SpriteFontTextMesh mainText;

    public SpriteRenderer button;
    public SpriteFontTextMesh keyText;

    private ControllerEx.ControllerType _controller = ControllerEx.ControllerType.None;
    private ObjectBase _target;
    private LineEffectBase _line;

    private bool _showKey = false;
    private string _checkKey;

    public override void firstSetting()
    {
        base.firstSetting();

        SetSprite("tutorialsign");
    }
    public override void initialize()
    {
        SetActive(false);
    }

    public override void progress(float deltaTime)
    {
        if(_target.deleted || Player.instance.deleted)
        {
            Dispose();
            Delete();
        }

        if(_showKey)
        {
            SetKeyGraphic();
        }

        Follow();
        WeaponTypeProgress();
    }

    public void Dispose()
    {
        if(_line != null)
            _line.SetActive(false);
    }

    public void WeaponTypeProgress()
    {
        var t = Player.instance.weaponInven.GetCurrentWeaponType();
        if(t == WeaponBase.WeaponList.Lancer)
        {
            LanceTutorial();
        }
        else if(t == WeaponBase.WeaponList.Pulse)
        {
            PulseTutorial();
        }
        else if(t == WeaponBase.WeaponList.PhantomStinger)
        {
            PhantomTutorial();
        }
    }

    public void LanceTutorial()
    {
        var dist = Vector3.Distance(_position,Player.instance.position);
        DrawLine(Player.instance.position,0.234f);

        if(dist <= 2.4)
        {
            SetColor(Color.green);
            _line.SetColor(Color.green);

            SetMainText("Attack");
            ShowKeyGraphic("MainAttack");
        }
        else
        {
            SetColor(Color.red);
            _line.SetColor(Color.red);

            SetMainText("too far");
            InitController();
        }
    }

    public void PulseTutorial()
    {
        var dist = Vector3.Distance(_position,Player.instance.position);
        DrawLine(Player.instance.position,0.234f);

        if(dist <= 1.1)
        {
            SetColor(Color.green);
            _line.SetColor(Color.green);

            SetMainText("Attack");
            ShowKeyGraphic("MainAttack");
        }
        else
        {
            SetColor(Color.red);
            _line.SetColor(Color.red);

            SetMainText("too far");
            InitController();
        }
    }

    public void PhantomTutorial()
    {
        var dist = Vector3.Distance(_position,Player.instance.position);
        DrawLine(Player.instance.position,0.234f);
        var weapon = Player.instance.weaponInven.GetCurrentWeapon();

        if(weapon.mainAttack)
        {
            SetColor(Color.green);
            _line.SetColor(Color.green);

            SetMainText("Up");
            ShowKeyGraphic("MainAttack");
        }
        else if(dist <= 1.5)
        {
            if(weapon.Aimed())
            {
                SetColor(Color.cyan);
                _line.SetColor(Color.green);

                SetMainText("Hold");
                ShowKeyGraphic("MainAttack");
            }
            else
            {
                SetColor(Color.magenta);
                _line.SetColor(Color.magenta);

                SetMainText("Aim");
                //ShowKeyGraphic("MainAttack");
                InitController();
            }

        }
        else
        {
            SetColor(Color.red);
            _line.SetColor(Color.red);

            SetMainText("too far");
            InitController();
        }
    }

    public void SetMainText(string s)
    {
        mainText.SetText(s);
    }

    public void DrawLine(Vector3 pos, float dist)
    {
        if(_line == null)
        {
            _line = EffectManager.GetInstance().AddLineEffect(Vector2.zero,Vector2.zero,0.03f,1f).PassiveDeactive();
        }

        var direction = (_position - pos).normalized;

        pos += direction * dist;
        _line.SetPosition(pos,0);
        _line.SetPosition(_position - direction * dist,1);
    }

    public void Active(ObjectBase target, string text)
    {
        _target = target;
        tp.position = _target.position;

        _showKey = false;
        _checkKey = "";

        _controller = ControllerEx.ControllerType.None;

        button.gameObject.SetActive(false);
        keyText.gameObject.SetActive(false);

        SetActive(true);
    }

    public void InitController()
    {
        _showKey = false;
        _checkKey = "";

        _controller = ControllerEx.ControllerType.None;

        button.gameObject.SetActive(false);
        keyText.gameObject.SetActive(false);
    }

    public TutorialSign SetColor(Color color)
    {
        _sprRenderer.color = color;
        button.color = color;
        keyText.textColor = color;
        keyText.UpdateColor();
        mainText.textColor = color;
        mainText.UpdateColor();

        return this;
    }

    public TutorialSign ShowKeyGraphic(string s)
    {
        _checkKey = s;
        _showKey = true;

        SetKeyGraphic();

        return this;
    }

    public void SetKeyGraphic()
    {
        if(_controller == ControllerEx.GetInstance().controller)
        {
            return;
        }


        if(ControllerEx.GetInstance().controller == ControllerEx.ControllerType.KeyboardMouse)
        {
            button.gameObject.SetActive(false);
            keyText.gameObject.SetActive(true);

            var key = ControllerEx.GetInstance().GetKeyboardString(_checkKey);
            keyText.SetText(key);
        }
        else if(ControllerEx.GetInstance().controller == ControllerEx.ControllerType.XboxController)
        {
            button.gameObject.SetActive(true);
            keyText.gameObject.SetActive(false);

            var key = ControllerEx.GetInstance().GetXboxGraphic(_checkKey);
            button.sprite = key;
        }
        else if(ControllerEx.GetInstance().controller == ControllerEx.ControllerType.PSController)
        {
            button.gameObject.SetActive(true);
            keyText.gameObject.SetActive(false);

            var key = ControllerEx.GetInstance().GetPSGraphic(_checkKey);
            button.sprite = key;
        }

        _controller = ControllerEx.GetInstance().controller;

    }

    public void Follow()
    {
        if(!_target.deleted)
            _position = _target.position;
    }
}
