using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Editor_AnimationKeyBase : MonoBehaviour
{
    public delegate void keyModityEvent();
    public static Editor_AnimationKeyBase selected;
    public static keyModityEvent keyModity = new keyModityEvent(()=>{});

    public RectTransform rectTp;
    public Image image;
    public int frame;
    public int maxFrame;
    public float stayTime;

    public string aniName;
    public Sprite sprite;

    public void Set(Sprite s, float t)
    {
        sprite = s;
        stayTime = t;
    }

    public Editor_AnimationKeyBase Select()
    {
        if(selected != null)
        {
            selected.image.color = Color.white;
        }

        image.color = Color.green;
        selected = this;
        return this;
    }
}
