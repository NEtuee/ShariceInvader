using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultRecorder : Singleton<ResultRecorder>
{
    public float timer = 0f;
    public int combo = 0;
    public int damage = 0;

    public int score = 0;
    public int maxScore = 1680;

    public bool clear = false;


    public void Initialize()
    {
        timer = 0f;
        combo = 0;
        damage = 0;
        score = 0;

        clear = false;
    }

    public void SetComboCount(int c)
    {
        if(combo < c)
            combo = c;
    }

    public void CalculationScore()
    {
        score += CurrentComboScore();
        score += CurrentDamageScore();
        score += CurrentTimeScore();
    }

    public int CurrentTimeScore()
    {
        var i = 1000 - (int)timer;
        i = i < 0 ? 0 : i;
        return i;
    }
    public int CurrentComboScore()
    {
        return combo * 15;
    }
    public int CurrentDamageScore()
    {
        var i = 500 - damage;
        i = i < 0 ? 0 : i;
        return i;
    }
}
