using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public bool outline = false;
    public Color color;
    
    Material material;
    void Start()
    {
        material = new Material(Shader.Find("Custom/SpriteOutline"));
        GetComponent<SpriteRenderer>().material = material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_Outline", outline ? 1f : 0);
        material.SetColor("_OutlineColor", color);
    }
}
