/**
This work is licensed under a Creative Commons Attribution 3.0 Unported License.
http://creativecommons.org/licenses/by/3.0/deed.en_GB

You are free:

to copy, distribute, display, and perform the work
to make derivative works
to make commercial use of the work
*/

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class GlitchEffect : MonoBehaviour
{
	public Texture2D displacementMap;
	public Shader Shader;
	[Header("Glitch Intensity")]

	[Range(0, 1)]
	public float intensity;

	[Range(0, 1)]
	public float flipIntensity;

	[Range(0, 1)]
	public float colorIntensity;

	[Range(0, 0.1f)]
	public float flickerTime = 0.05f;

	private float _time;
	private float _timer;

	private float _glitchup;
	private float _glitchdown;
	private float flicker;
	private float _glitchupTime = 0.05f;
	private float _glitchdownTime = 0.05f;
	private Material _material;


	private float _glitchIntensity;
    private float _glitchFlip;
    private float _glitchColor;
    private float _glitchFlickerTime;

	void Start()
	{
		_material = new Material(Shader);
	}

	public void Update()
	{
		_timer += Time.deltaTime;

		float t = _timer / _time;

		intensity = Mathf.Lerp(_glitchIntensity,0,t);
		flipIntensity = Mathf.Lerp(_glitchFlip,0,t);
		colorIntensity = Mathf.Lerp(_glitchColor,0,t);
		flickerTime = Mathf.Lerp(_glitchFlickerTime,0,t);

		if(_timer >= _time)
		{
			this.enabled = false;
		}
	}

	public void Active(float inten, float flip, float col, float flic, float time)
    {
        intensity = _glitchIntensity = inten;
        flipIntensity = _glitchFlip = flip;
        colorIntensity = _glitchColor = col;
        flickerTime = _glitchFlickerTime = flic;

		if(time > _time - _timer)
		{
			_time = time;
        	_timer = 0f;
		}

        this.enabled = true;
    }

	// Called by camera to apply image effect
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		_material.SetFloat("_Intensity", intensity);
		_material.SetFloat("_ColorIntensity", colorIntensity);
		_material.SetTexture("_DispTex", displacementMap);

		flicker += Time.deltaTime * colorIntensity;
		if (flicker > flickerTime)
		{
			_material.SetFloat("filterRadius", Random.Range(-3f, 3f) * colorIntensity);
			_material.SetVector("direction", Quaternion.AngleAxis(Random.Range(0, 360) * colorIntensity, Vector3.forward) * Vector4.one);
			flicker = 0;
		}

		if (colorIntensity == 0)
			_material.SetFloat("filterRadius", 0);

		_glitchup += Time.deltaTime * flipIntensity;
		if (_glitchup > _glitchupTime)
		{
			if (Random.value < 0.1f * flipIntensity)
				_material.SetFloat("flip_up", Random.Range(0, 1f) * flipIntensity);
			else
				_material.SetFloat("flip_up", 0);

			_glitchup = 0;
		}

		if (flipIntensity == 0)
			_material.SetFloat("flip_up", 0);

		_glitchdown += Time.deltaTime * flipIntensity;
		if (_glitchdown > _glitchdownTime)
		{
			if (Random.value < 0.1f * flipIntensity)
				_material.SetFloat("flip_down", 1 - Random.Range(0, 1f) * flipIntensity);
			else
				_material.SetFloat("flip_down", 1);

			_glitchdown = 0;
		}

		if (flipIntensity == 0)
			_material.SetFloat("flip_down", 1);

		if (Random.value < 0.05 * intensity)
		{
			_material.SetFloat("displace", Random.value * intensity);
			_material.SetFloat("scale", 1 - Random.value * intensity);
		}
		else
			_material.SetFloat("displace", 0);

		Graphics.Blit(source, destination, _material);
	}
}
