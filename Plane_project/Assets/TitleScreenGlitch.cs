using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenGlitch : MonoBehaviour
{
    public GlitchEffect one;
    public ScreenGlitch screenGlitch;

    float _glitchTimer = 0f;
    float _timeSaver;

    void Start()
    {
        ActiveUIGlitch(3f);
        Glitch(4f);
    }

    // Update is called once per frame
    void Update()
    {
      if(_glitchTimer != 0f)
		{
			_glitchTimer -= Time.deltaTime;

			float percentage = (_timeSaver - _glitchTimer) / _timeSaver;

			screenGlitch._colorDrift = Mathf.Lerp(0.4f,0f,percentage);
			screenGlitch._scanLineJitter = Mathf.Lerp(0.5f,0f,percentage);

			// if(percentage > 0.5f)
			// {
			// 	digitalGlitch.digitalIntensity = .2f;
			// }
			// else
			// {
			// 	digitalGlitch.digitalIntensity = .1f;
			// }

			if(_glitchTimer <= 0f)
			{
				screenGlitch._colorDrift = 0f;
				screenGlitch._scanLineJitter = 0f;

				screenGlitch.progress = false;
				screenGlitch.enabled = false;
				_glitchTimer = 0f;
			}
		}
	}

	public void Glitch(float time)
	{
		_timeSaver = _glitchTimer = time;

		screenGlitch.progress = true;
		screenGlitch.enabled = true;

		screenGlitch._colorDrift = 3f;
		screenGlitch._scanLineJitter = 3f;
	}

    public void ActiveUIGlitch(float time)
    {
        one.Active(.474f,.694f,.562f,.0453f,time);
    }
}
