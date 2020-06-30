using UnityEngine;
using UnityEngine.UI;


public class FadeManager : SingletonMono<FadeManager>
{
    public Image fadeSprite;

    private bool _fading = false;
    private float _timer = 0f;
    private float _fadeSpeed = 1f;
    private float _alphaTarget = 0f;

    private Color _originColor = Color.black;
    private Color _targetColor = Color.black;

    public void Awake()
    {
        if(instance != null)
		{
			Destroy(this);
			return;
		}

        SetSingleton(this);
        fadeSprite.gameObject.SetActive(false);

        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if(_fading)
        {
            _timer += Time.deltaTime;
            fadeSprite.color = Color.Lerp(_originColor, _targetColor, _timer);
            if(_timer >= 1f)
            {
                _timer = 0f;
                _fading = false;
                fadeSprite.color = _targetColor;
            }
        }
    }

    public bool IsFading() {return _fading;}

    public void FadeIn()
    {
        SetFade(1f,0f,1f);
    }

    public void FadeOut()
    {
        SetFade(0f,1f,1f);
    }

    public void SetFade(float originAlpha, float targetAlpha, float speed)
    {
        _originColor.a = originAlpha;
        _targetColor.a = targetAlpha;
        _fadeSpeed = speed;

        _fading = true;
        _timer = 0f;

        fadeSprite.color = _originColor;
        fadeSprite.gameObject.SetActive(true);
    }
}
