using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleControll : MonoBehaviour
{
    public float timer = 2f;

    public GameObject[] items;

    public Animation ani1;
    public Line ani2;

    public bool check = false;

    private bool _fade = false;

    private float _movementTimer = 0f;

    void Start()
    {
        foreach(var item in items)
            item.SetActive(false);
        
        SoundManager.instance.PlayBGM("BGM/MainTheme",true,0.8f);
    }

    void Update()
    {
        if(!check)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                foreach(var item in items)
                    item.SetActive(true);

                ani1.Play();
                ani2.enabled = true;

                check = true;
                timer = 2f;
            }
        }
        else
        {
            
            if(timer != 0f)
            {
                timer -= Time.deltaTime;
                if(timer <= 0f)
                {
                    timer = 0f;


                }
            }
            else
            {
                if(Input.anyKey && !_fade)
                {
                    _fade = true;
                    SoundManager.instance.Play("SE/MainHit_",false,2,.5f,false);
                    FadeManager.instance.FadeOut(0f);
                }
            }
        }

        if(_fade)
        {
            if(!FadeManager.instance.IsFading())
            {
                SceneManager.LoadScene(1);
            }
        }
        
        _movementTimer += Time.deltaTime * 0.5f;
        transform.position = MathEx.Lemniscate_Gerono(0.1f,_movementTimer);
    }
}
