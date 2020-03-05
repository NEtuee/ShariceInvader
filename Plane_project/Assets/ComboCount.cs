using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCount : SingletonMono<ComboCount>
{
    public GameObject comboUI;
    TextMesh text;
    int combo;
    float comboCount;
    void Start()
    {
        SetSingleton(this);
        text = GetComponent<TextMesh>();
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
    }

    public void AddComboCount(int i)
    {
        combo += i;
        comboCount = 3f;
        if(combo == 0)
        {
            text.text = "";
            comboUI.SetActive(false);
            comboCount = 0f;
        }
        else  
        {
            comboUI.SetActive(true);
            text.text = combo.ToString();
        }
    }

}
