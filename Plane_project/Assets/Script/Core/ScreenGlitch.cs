using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenGlitch : MonoBehaviour
{
    public Shader analogShader;

    public float _scanLineJitter = 0;
    public float _colorDrift = 0;

    public bool progress = false;

    Material _analogMat;


    public void Start()
    {
        if (_analogMat == null)
        {
            _analogMat = new Material(analogShader);
            _analogMat.hideFlags = HideFlags.DontSave;
        }

        enabled = false;
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {  
        if(!progress)
        {
            Graphics.Blit(source, destination, _analogMat);
        }   
        
        /* #region  Analog Shader Setup */
        var sl_thresh = Mathf.Clamp01(1.0f - _scanLineJitter * 1.2f);
        var sl_disp = 0.002f + Mathf.Pow(_scanLineJitter, 3) * 0.05f;
        _analogMat.SetVector("_ScanLineJitter", new Vector2(sl_disp, sl_thresh));

        var cd = new Vector2(_colorDrift * 0.04f, Time.time * 606.11f);
        _analogMat.SetVector("_ColorDrift", cd);

        Graphics.Blit(source, destination, _analogMat);
        /* #endregion */
    }
}
