using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public float dist = 1f;
    public float time = 0.4f;
    public Vector3 direction = new Vector3();
    
    private Vector3 origin;
    private float timer = 0f;
    public void Start()
    {
        direction = direction.normalized;
        origin = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position = MathEx.easeOutCubicVector2(origin,origin + direction * dist,timer / time);

        if(timer >= time)
        {
            transform.position = origin + direction * dist;
            enabled = false;
        }
    }
}
