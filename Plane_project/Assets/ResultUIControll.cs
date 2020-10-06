using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUIControll : MonoBehaviour
{
    public DialogHelper dialog;
    // public SpriteFontTextMesh dialog_title;
    // public TextMesh dialog_text;
    // public GameObject dialog;
    // public SpriteRenderer dialog_sprite;


    public SpriteFontTextMesh mainScore;
    public SpriteFontTextMesh time;
    public SpriteFontTextMesh combo;
    public SpriteFontTextMesh hitCount;

    public SpriteRenderer progressBar;


    private string[] dialogCode_clear = {
        "dialog_result_clear_0"
    };
    private string[] dialogCode_fail = {
        "dialog_result_fail_0",
        "dialog_result_fail_1",
        "dialog_result_fail_2",
    };
    

    private Material _progressMat;

    private int _combo;
    private int _hitCount;
    private int _score;
    private int _maxScore;
    private int _gameTime;

    private int _prevScore;


    private int _progressCount = 0;

    private float _timer;
    private float _progress = 0f;
    private float _progressTarget = 0f;
    private float _prevProgressTarget = 0f;

    private bool _act = false;
    private bool _progressBar = false;
    private bool _clear = false;

    public void Awake()
    {
        _progressMat = new Material(Shader.Find("Custom/Default_ProgressBar"));
        progressBar.material = _progressMat;
    }

    public void Active()
    {
        time.SetText("");
        combo.SetText("");
        hitCount.SetText("");

        GameMain.instance.update = false;
        _progressBar = true;

        _timer = 1f;
        _progressCount = 1;

        _gameTime = (int)ResultRecorder.GetInstance().timer;
        _combo = ResultRecorder.GetInstance().combo;
        _hitCount = ResultRecorder.GetInstance().damage;
        _maxScore = ResultRecorder.GetInstance().maxScore;

        _prevScore = 0;
        _score = ResultRecorder.GetInstance().CurrentTimeScore();
        _clear = ResultRecorder.GetInstance().clear;

        string dialogCode = _clear ? dialogCode_clear[Random.Range(0,dialogCode_clear.Length)] : 
                            dialogCode_fail[Random.Range(0,dialogCode_fail.Length)];
        
        //DialogManager.instance.SetRightSideObjects(dialog,dialog_sprite,dialog_title,dialog_text);
        DialogManager.instance.SetRightSideObjects(dialog);
        DialogManager.instance.ShowDialog(dialogCode,false,true);

        mainScore.SetText("0");

        UpdateProgressTarget();

        this.gameObject.SetActive(true);

        FadeManager.instance.FadeIn(0f);
    }

    void Update()
    {
        if(!_act)
        {
            _timer -= Time.deltaTime;
            if(_timer <= 0f)
            {
                _timer = 0f;
                _act = true;
            }

            return;
        }

        if(_progressCount <= 3)
        {
            UpdateProgressBar();
        }

        if(!_progressBar)
        {
            _timer += Time.deltaTime;
            if(_timer >= 1.5f)
            {
                _timer = 0f;
                _prevScore = _score;

                if(_progressCount == 1)
                {
                    _score += ResultRecorder.GetInstance().CurrentComboScore();
                }
                else if(_progressCount == 2)
                {
                    _score += ResultRecorder.GetInstance().CurrentDamageScore();
                }

                ++_progressCount;
                UpdateProgressTarget();
            }
        }
    }

    public void UpdateProgressTarget()
    {
        _prevProgressTarget = _progressTarget;
        _progressTarget = (float)_score / (float)_maxScore;

        _progressBar = true;
    }

    public bool UpdateProgressBar()
    {
        if(_progressBar)
        {
            float a = _timer / .3f;
            _progress = MathEx.easeOutCubic(_prevProgressTarget,_progressTarget,a);

            if(_progressCount == 1)
            {
                time.SetText(((int)Mathf.Lerp(0f,(float)_gameTime,a)).ToString());
            }
            else if(_progressCount == 2)
            {
                combo.SetText(((int)Mathf.Lerp(0f,(float)_combo,a)).ToString());
            }
            else if(_progressCount == 3)
            {
                hitCount.SetText(((int)Mathf.Lerp(0f,(float)_hitCount,a)).ToString());
            }

            if(ResultRecorder.GetInstance().clear)
                mainScore.SetText(((int)Mathf.Lerp((float)_prevScore,(float)_score,a)).ToString());

            _timer += Time.deltaTime;

            if(_timer >= .3f)
            {
                _timer = 0f;
                _progressBar = false;

                _progress = _progressTarget;
                if(ResultRecorder.GetInstance().clear)
                    mainScore.SetText(_score.ToString());
            }

            if(ResultRecorder.GetInstance().clear)
                _progressMat.SetFloat("_Progress",_progress);
        }

        return _progressBar;

    }
}
