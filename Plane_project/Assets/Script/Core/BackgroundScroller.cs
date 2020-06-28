using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Vector2 screenSize;
    public int layer;

    private Vector2 _scale;
    private Vector2 _offset;

    private Vector2 _offsetLimit;
    private Material _mainMat;
    private MeshRenderer _meshRenderer;


    public void Init()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _mainMat = _meshRenderer.material;
        Vector3 textureSize = new Vector3(_mainMat.mainTexture.width,_mainMat.mainTexture.height);

        _scale = new Vector2(screenSize.x / textureSize.x,screenSize.y / textureSize.y);
        _offsetLimit = new Vector2((textureSize.x / screenSize.x) - 1f, (textureSize.y / screenSize.y) - 1f);

        _mainMat.SetFloat("_MainScaleX",_scale.x);
        _mainMat.SetFloat("_MainScaleY",_scale.y);
        _mainMat.SetFloat("PixelSnap",1f);

        _meshRenderer.sortingLayerID = 0;
        _meshRenderer.sortingOrder = layer;
    }

    public void ScreenScroll(Vector2 o)
    {
        // o.x = o.x > 1f ? 1f : o.x < 0f ? 0f : o.x;
        // o.y = o.y > 1f ? 1f : o.y < 0f ? 0f : o.y;


        _offset = _offsetLimit * o;
        _offset.y *= _offset.y < 0 ? -1f : 1f;
        _offset.x += 0.5f - (1f - o.x);

        _mainMat.SetFloat("_MainOffsetX",_offset.x);
        _mainMat.SetFloat("_MainOffsetY",_offset.y);
    }
}
