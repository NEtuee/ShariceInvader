using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follower : SingletonMono<follower>
{
    public Transform tp;
    public float gague;

    void Start()
    {
        SetSingleton(this);
    }

    public void CC(float deltaTime)
    {
        if(GameManager.instance.player != null)
        {
            transform.position = Vector3.Lerp(tp.position,transform.position,gague);
        }
        else
            this.gameObject.SetActive(false);
    }
}
