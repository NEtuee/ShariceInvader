using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : SingletonMono<PlayerFollower>
{
    void Start()
    {
        SetSingleton(this);
    }
    public void CC(float deltaTime)
    {
        if(GameManager.instance.player != null)
        {
            transform.position = Vector3.Lerp(GameManager.instance.player.transform.position,transform.position,0.65f * deltaTime);
        }
        else
            this.gameObject.SetActive(false);
    }
}
