using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogHelper : MonoBehaviour
{
    public enum ProgressType
    {
        Normal,
        Auto
    };

    public SpriteFontTextMesh title;
    public TextMeshPro mainText;
    public SpriteRenderer portrait;
    public SpriteRenderer background;
    public SpriteRenderer indicator;

    public ProgressType progressType;

    private AnimationControllEx _indicatorAnimation = null;

    public void Initialize()
    {
        if(_indicatorAnimation == null)
        {
            _indicatorAnimation = new AnimationControllEx(indicator);
            _indicatorAnimation.AddAnimation("Auto","UI/Dialog/Auto");
            _indicatorAnimation.AddAnimation("End","UI/Dialog/End");
            _indicatorAnimation.AddAnimation("Talking","UI/Dialog/Talking");
        }
        
        
    }

    public void SetInfo(string t, Sprite p, Sprite b, string m = "")
    {
        title.SetText(t);
        portrait.sprite = p;
        background.sprite = b;

        mainText.SetText(m);
    }

    public void SetMainText(string s)
    {
        mainText.SetText(s);
    }

    public void AddMainText(string s)
    {
        mainText.SetText(mainText.text + s);
    }

    public void SetColor(Color c)
    {
        title.textColor = c;
        title.UpdateColor();
        mainText.color = c;
        portrait.color = c;
        background.color = c;
        indicator.color = c;
    }

    public void AddMainText(char c)
    {
        mainText.SetText(mainText.text + c);
    }

    public void SetType(ProgressType type)
    {
        progressType = type;
    }

    public void AnimationProgress(float deltaTime)
    {
        _indicatorAnimation.AnimationProgress(deltaTime);
    }

    public void AnimationTalkingStart()
    {
        if(progressType == ProgressType.Auto)
        {
            _indicatorAnimation.ChangeAni("Auto",true);
        }
        else
        {
            _indicatorAnimation.ChangeAni("Talking",true);
        }
    }
    public void AnimationTalkingEnd()
    {
        if(progressType == ProgressType.Auto)
        {
            _indicatorAnimation.ChangeAni("Auto",true);
        }
        else
        {
            _indicatorAnimation.ChangeAni("End",false);
        }
    }
}
