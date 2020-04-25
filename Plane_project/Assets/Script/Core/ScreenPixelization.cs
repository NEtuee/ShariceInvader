using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPixelization : MonoBehaviour
{
    public Material pixelationMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {  
        Graphics.Blit(source, destination, pixelationMaterial);

    }
}
