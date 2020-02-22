using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testsss : MonoBehaviour
{
    void Start()
    {
        transform.localPosition = CanvasScript.instance.CanvasPosToWorldPos(new Vector2(0,0));
    }

    void Update()
    {
        
    }
}
