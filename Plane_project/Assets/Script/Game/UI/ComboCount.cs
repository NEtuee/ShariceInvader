using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCount : SingletonMono<ComboCount>
{
    public GameObject comboUI;
    SpriteFontTextMesh text;
    int combo;
    float comboCount;
    void Start()
    {
        SetSingleton(this);
        text = GetComponent<SpriteFontTextMesh>();
        combo = 0;
        comboCount = 0f;
    }

    void Update()
    {
        if(comboCount != 0f)
        {
            comboCount -= Time.deltaTime;
            if(comboCount <= 0f)
            {
                comboCount = 0f;
                combo = 0;
                AddComboCount(0);
            }
        }

        Vector3 scale = text.gameObject.transform.localScale;
        text.gameObject.transform.localScale = Vector3.Lerp(scale,new Vector3(1f,1f,1f),.3f);
    }

    public void AddComboCount(int i)
    {
        combo += i;
        comboCount = 3f;
        text.gameObject.transform.localScale = new Vector3(1.5f,1.9f);

        if(combo == 0)
        {
            text.SetText("");
            comboUI.SetActive(false);
            comboCount = 0f;
        }
        else  
        {
            comboUI.SetActive(true);
            text.SetText(combo.ToString());
        }
    }

}
