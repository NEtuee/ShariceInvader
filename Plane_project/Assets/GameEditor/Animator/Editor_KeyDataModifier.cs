using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_KeyDataModifier : MonoBehaviour
{
    public InputField stayTimeField;
    public Image image;
    public RectTransform imageRect;
    public Text aniName;
    public Text frame;

    private Sprite _sprite;
    private Editor_AnimationKeyBase _key;

    public void Start()
    {
        Editor_AnimationKeyViewer.keySelected += GetKeyInfo;
    }

    public void InitValue()
    {
        _key = null;

        image.sprite = null;
        aniName.text = "Animation Name";
        frame.text = "Frame : 0 of 0";
        imageRect.sizeDelta = new Vector2(165f,165f);

        stayTimeField.text = "";
        stayTimeField.interactable = false;
    }

    public void GetKeyInfo(Editor_AnimationKeyBase key)
    {
        _key = key;

        stayTimeField.interactable = true;

        aniName.text = key.aniName;
        frame.text = "Frame : " + key.frame.ToString() + " of " + (key.maxFrame - 1).ToString();
        stayTimeField.text = key.stayTime.ToString();

        _sprite = key.sprite;

        SetSpriteToImage();
    }

    public void ValueChanged()
    {
        if(stayTimeField.text != "" && _key != null)
        {
            _key.stayTime = float.Parse(stayTimeField.text);
            Editor_AnimationKeyBase.keyModity();
        }
    }

    public void SetSpriteToImage()
    {
        Vector2 spriteSize = new Vector2(_sprite.rect.width,_sprite.rect.height);

        float ratio = 0f;

        if(spriteSize.x > spriteSize.y)
        {
            ratio = 165f / spriteSize.x;
        }
        else
        {
            ratio = 165f / spriteSize.y;
        }

        imageRect.sizeDelta = spriteSize * ratio;
        image.sprite = _sprite;
    }
}
