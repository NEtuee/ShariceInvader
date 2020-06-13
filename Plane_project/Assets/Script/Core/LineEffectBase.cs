using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEffectBase : ObjectBase
{
    public LineRenderer mainLine;

    private float _mainTimer = 0f;
    private float _widthLerpEnd = 0f;
    private float _widthLerpTimer = 0f;
    private float _offsetScrollSpeed = 0f;
    private float _offsetScrollValue = 0f;

    private bool _lerpWidth = false;
    private bool _lerpColor = false;
    private bool _offsetScroll = false;

    private Material pixelSnapMat;
    private Material tilingMat;

    public override void firstSetting()
    {
        mainLine = gameObject.AddComponent<LineRenderer>();
        mainLine.material = ResourceManager.GetInstance().GetPixelSnapMaterial();
        mainLine.SetPosition(0,Vector3.zero);
        mainLine.SetPosition(1,Vector3.zero);

        mainLine.sortingOrder = -1;

        pixelSnapMat = ResourceManager.GetInstance().GetMaterial("PixelSnap");
        tilingMat = new Material(Shader.Find("Custom/TilingShader"));
    }

    public LineEffectBase Active(Vector3 start, Vector3 end, float width, float timer)
    {
        mainLine.SetPosition(0,start);
        mainLine.SetPosition(1,end);

        _mainTimer = timer;

        mainLine.sortingOrder = -1;
        mainLine.startWidth = width;
        mainLine.endWidth = width;
        mainLine.textureMode = LineTextureMode.Stretch;

        _lerpWidth = false;
        _offsetScroll = false;

        SetMaterial(pixelSnapMat);
        SetColor(Color.white);

        SetActive(true);

        return this;
    }

    public LineEffectBase SetColor(Color color) {mainLine.startColor = color; mainLine.endColor = color; return this;}
    public LineEffectBase SetMaterial(Material mat) {mainLine.material = mat; return this;}
    public LineEffectBase SetPosition(Vector3 pos, int point) {mainLine.SetPosition(point,pos); return this;}
    public LineEffectBase SetTextureMode(LineTextureMode mode){mainLine.textureMode = mode; return this;}
    public LineEffectBase SetTiling(Sprite spr,float tiling)
    {
        tilingMat.SetTexture("_MainTex",spr.texture);
        tilingMat.SetFloat("_Tiling",tiling);
        tilingMat.SetFloat("_Offset",0f);
        SetTextureMode(LineTextureMode.Tile);

        mainLine.material = tilingMat;
        return this;
    }
    public LineEffectBase SetOffsetScrolling(float speed)
    {
        _offsetScroll = true;
        _offsetScrollSpeed = speed;
        _offsetScrollValue = 0f;

        return this;
    }
    public LineEffectBase SetLerpWidth(float target, float time)
    {
        _widthLerpEnd = target;
        _widthLerpTimer = time;
        _lerpWidth = true;

        return this;
    }

    public override void initialize()
    {

    }

    public override void progress(float deltaTime)
    {
        _mainTimer -= deltaTime;

        if(_mainTimer <= 0f)
        {
            _mainTimer = 0f;
            SetActive(false);
        }

        if(_lerpWidth)
        {
            float width = Mathf.Lerp(mainLine.startWidth,_widthLerpEnd,_widthLerpTimer);
            
            mainLine.startWidth = width;
            mainLine.endWidth = width;
        }

        if(_offsetScroll)
        {
            _offsetScrollValue += _offsetScrollSpeed * deltaTime;
            mainLine.material.SetFloat("_Offset",_offsetScrollValue);
        }
    }

    public override void release()
    {

    }


}
