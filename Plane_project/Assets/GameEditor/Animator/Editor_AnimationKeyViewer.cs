using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_AnimationKeyViewer : SingletonMono<Editor_AnimationKeyViewer>
{
    public delegate void keySelectedEvent(Editor_AnimationKeyBase key);

    public static keySelectedEvent keySelected = new keySelectedEvent((Editor_AnimationKeyBase key)=>{});

    public Button saveButton;

    public GameObject keyBase;
    public RectTransform keyHolder;

    public Text aniName;
    public Text stayTimeText;
    public Text currTimeText;

    public bool createAnimationFile = true;
    public bool keySelectWhilePlay = false;

    private List<Editor_AnimationKeyBase> _keys = new List<Editor_AnimationKeyBase>();
    private Queue<Editor_AnimationKeyBase> _keyPool = new Queue<Editor_AnimationKeyBase>();

    private bool _keyExist = false;

    private bool _aniPlay = false;
    private SpriteRenderer _aniPreviewer;
    private float _timer = 0f;
    private float _stayTime = 0f;
    private int _pos = 0;
    private bool _loop = false;

    private string savePath = "";

    private bool _changed = false;

    public void Start()
    {
        Editor_EventSystem.instance.clickEvent += KeySelectEvent;
        Editor_AnimationKeyBase.keyModity += KeyModifyEvent;

        _aniPreviewer = new GameObject("AnimationPreviewer").AddComponent<SpriteRenderer>();
        _aniPreviewer.transform.position = Vector3.zero;

        SetSingleton(this);
    }

    public void Update()
    {
        HotKeyCheck();

        if(_aniPlay)
        {
            _timer += Time.deltaTime;

            if(_timer >= _stayTime)
            {
                _timer -= _stayTime;

                _keys[_pos].image.color = Color.white;

                _pos++;

                if(_pos >= _keys.Count)
                {
                    if(_loop)
                        _pos = 0;
                    else
                    {
                        PauseButton();
                        return;
                    }
                }

                _stayTime = _keys[_pos].stayTime;
                stayTimeText.text = _stayTime.ToString();

                _aniPreviewer.sprite = _keys[_pos].sprite;

                _keys[_pos].image.color = Color.blue;

                if(keySelectWhilePlay)
                    KeySelectEvent(_pos);
            }

            double t = Math.Truncate(_timer * 10000f) / 10000f;
            currTimeText.text = t.ToString();
        }
    }

    public void HotKeyCheck()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!_aniPlay)
                FirstPlayButton();
            else
                PauseButton();
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(Editor_AnimationKeyBase.selected != null)
            {
                int f = Editor_AnimationKeyBase.selected.frame;
                KeySelectEvent(f - 1 < 0 ? _keys.Count - 1 : f - 1);
            }
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(Editor_AnimationKeyBase.selected != null)
            {
                int f = Editor_AnimationKeyBase.selected.frame;
                KeySelectEvent(f + 1 >= _keys.Count ? 0 : f + 1);
            }
        }
    }

    public void FirstPlayButton()
    {
        AniInit(0);
    }

    public void PlayButton()
    {
        AniInit(Editor_AnimationKeyBase.selected == null ? 0 : Editor_AnimationKeyBase.selected.frame);
    }

    public void PauseButton()
    {
        //_keys[_pos >= _keys.Count ? _keys.Count - 1 : _pos].image.color = Color.white;
        if(Editor_AnimationKeyBase.selected != null)
            Editor_AnimationKeyBase.selected.image.color = Color.green;

        _aniPlay = false;
    }

    public void LoopToggle()
    {
        _loop = !_loop;
    }

    public void AniInit(int pos)
    {
        if(!_keyExist)
            return;

        _aniPlay = true;
        _timer = 0f;
        _pos = pos;

        _aniPreviewer.sprite = _keys[pos].sprite;
        _stayTime = _keys[pos].stayTime;

        if(Editor_AnimationKeyBase.selected != null)
            Editor_AnimationKeyBase.selected.image.color = Color.white;
        _keys[pos].image.color = Color.blue;

        if(keySelectWhilePlay)
        {
            KeySelectEvent(pos);
        }
        
    }

    public Editor_AnimationKeyBase CreateAnimationKey()
    {
        if(_keyPool.Count == 0)
            return Instantiate(keyBase).GetComponent<Editor_AnimationKeyBase>();

        var key = _keyPool.Dequeue();
        key.gameObject.SetActive(true);

        return key;
    }

    public void SetAllKeyDuration(float d)
    {
        foreach(var key in _keys)
        {
            key.stayTime = d;
        }

        Editor_AnimationKeyBase.keyModity();
    }

    public void ClearKeyList()
    {
        for(int i = 0; i < _keys.Count; ++i)
        {
            _keys[i].gameObject.SetActive(false);
            _keyPool.Enqueue(_keys[i]);
        }

        _keys.Clear();
    }

    public void CreateAnimationKeys(int count)
    {
        float placeWidth = (count * 22f) + (count * 8f) + 30f;

        Vector2 size = keyHolder.sizeDelta;
        size.x = placeWidth;

        keyHolder.sizeDelta = size;

        for(int i = 0; i < count; ++i)
        {
            Editor_AnimationKeyBase key = CreateAnimationKey();
            
            key.image.color = Color.white;
            key.rectTp.SetParent(keyHolder);
            key.rectTp.anchoredPosition = new Vector2(22f * i + (8f * (i + 1)),-50f);

            _keys.Add(key);
        }
    }

    public void KeyModifyEvent()
    {
        saveButton.interactable = _changed = true;
    }

    public void SaveKeyData()
    {
        List<string> s = new List<string>();

        for(int i = 0; i < _keys.Count; ++i)
        {
            s.Add(_keys[i].stayTime.ToString());
        }

        IOManager.WriteStringToFile_NoMark(s.ToArray(),savePath,false);

        Editor_EventSystem.instance.ActiveNotice("Save Complete");

        _changed = false;
        saveButton.interactable = _changed;
    }

    public void SetKeyData(string path, Sprite[] sprites)
    {
        string file = path.Substring(path.LastIndexOf('\\'));
        string name = path + file + "_Ani" + ".txt";

        file = file.Replace('\\',' ');
        aniName.text = file;

        string[] data = IOManager.ReadStringFromFile(name);
        savePath = name;

        if(data == null && createAnimationFile)
        {
            Debug.Log("create");
            
            CreateAnimationRef(name,0.08333f,sprites.Length);
        }

        for(int i = 0; i < _keys.Count; ++i)
        {
            float t = 0.08333f;

            if(data != null)
            {
                t = float.Parse(data[i]);
            }

            _keys[i].stayTime = t;
            _keys[i].sprite = sprites[i];
            _keys[i].frame = i;
            _keys[i].aniName = file;
            _keys[i].maxFrame = _keys.Count;
        }
    }

    public void CreateAnimationRef(string name, float time, int count)
    {
        List<string> s = new List<string>();

        string t = time.ToString();
        for(int i = 0; i < count; ++i)
        {
            s.Add(t);
        }

        IOManager.WriteStringToFile_NoMark(s.ToArray(),name,false);
    }

    public void PathSelectEvent()
    {
        Editor_FolderItemBase item = Editor_FolderItemBase.selected;

        string [] p = Directory.GetDirectories(item.filePath);
        Debug.Log(item.filePath);
        if(p.Length != 0)
        {
            Debug.Log("null");
            return;
        }

        Sprite [] spr = ResourceManager.GetInstance().GetSpriteAll(item.filePath);

        if(spr != null)
        {
            ClearKeyList();

            CreateAnimationKeys(spr.Length);

            SetKeyData(item.filePath,spr);


            _aniPreviewer.sprite = spr[0];

            _keyExist = true;

            saveButton.interactable = true;
        }
    }

    public void KeySelectEvent(RectTransform rect)
    {
        var key = rect.GetComponent<Editor_AnimationKeyBase>();

        if(key != null)
        {
            key.Select();
            _aniPreviewer.sprite = key.sprite;

            stayTimeText.text = key.stayTime.ToString();
            keySelected(key);
        }
    }

    public void KeySelectEvent(int pos)
    {
        var key = _keys[pos];

        if(key != null)
        {
            key.Select();
            _aniPreviewer.sprite = key.sprite;

            stayTimeText.text = key.stayTime.ToString();
            keySelected(key);
        }
    }

}
