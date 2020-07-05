using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : SingletonMono<DialogManager>
{
    public class ConversationInfo
    {   
        public CharacterInfo character;
        public int side;
        public List<TextInfo[]> scripts;
    }

    public class CharacterInfo
    {
        public string[] name;
        public Sprite graphic;
    }

    public class TextInfo
    {
        public string text = "";
        public float autoTimer = 2f;
    }

    private GameObject left;
    private SpriteRenderer leftSprite;
    private SpriteFontTextMesh leftName;
    private TextMesh leftDialog;

    private GameObject right;
    private SpriteRenderer rightSprite;
    private SpriteFontTextMesh rightName;
    private TextMesh rightDialog;

    public TextAsset dialogInfo;
    public TextAsset charInfo;

    public Dictionary<string, CharacterInfo> characters = new Dictionary<string, CharacterInfo>();
    public Dictionary<string, ConversationInfo[]> dialogs = new Dictionary<string, ConversationInfo[]>(); 

    public int langague = 0;

    public bool dialog = false;
    public bool scrollEnd = false;
    public bool skipped = false;
    public bool canSkip = true;
    public bool autoScroll = false;
    public int speachSide = -1;
    public int newLineLength = 1;
    public float textScrollTime;
    

    private Sprite[] _charPics;

    private int scriptPos;
    private int charPos;
    private int characterSpeachPos;
    private float scrollTimer = 0f;
    private float _autoScrollTimer = 0f;
    private ConversationInfo[] currDialog;
    private List<TextInfo[]> texts;
    private string scrollText;


    void Awake()
    {
        SetSingleton(this);

        ReadCharacter();
        ReadDialog();

    }

    public void Start()
    {
        if(OptionManager.instance != null)
            langague = OptionManager.instance.language;
    }

    void Update()
    {
        if(dialog)
        {
            if(!scrollEnd)
            {
                scrollTimer += Time.deltaTime;
                if(scrollTimer >= textScrollTime)
                {
                    scrollTimer = 0f;

                    if(texts[characterSpeachPos][langague].text.Length <= charPos)
                    {
                        ++characterSpeachPos;
                        scrollEnd = true;
                    }
                    else
                    {
                        SetScrollText();
                    }

                    ++charPos;
                }
            }

            if(ControllerEx.GetInstance().KeyDown("Cancel") && canSkip)
            {
                skipped = true;
                DialogEnd();
            }

            if(ControllerEx.GetInstance().KeyDown("MainAttack") && !autoScroll)
            {
                DialogClick();
            }
            
            if(autoScroll)
            {
                _autoScrollTimer -= Time.deltaTime;
                if(_autoScrollTimer <= 0f)
                {
                    DialogClick();
                }
            }
        }
    }

    public void DialogClick()
    {
        charPos = 0;
        scrollTimer = 0f;

        if(scrollEnd)
        {
            if(texts.Count <= characterSpeachPos)
            {
                characterSpeachPos = 0;

                ++scriptPos;
                if(!SetDialogInfo())
                {
                    DialogEnd();
                }
                
            }
            else
            {
                _autoScrollTimer = texts[characterSpeachPos][langague].autoTimer;
                if(speachSide == 0)
                {
                    leftDialog.text = "";
                }
                else if(speachSide == 1)
                {
                    rightDialog.text = "";
                }
            }

            scrollEnd = false;
        }
        else
        {
            if(speachSide == 0)
            {
                leftDialog.text = texts[characterSpeachPos][langague].text;
            }
            else if(speachSide == 1)
            {
                rightDialog.text = texts[characterSpeachPos][langague].text;
            }
            ++characterSpeachPos;
            scrollEnd = true;
        }
    }

    public void SetLeftSideObjects(GameObject obj, SpriteRenderer spr, SpriteFontTextMesh n, TextMesh t)
    {
        left = obj;
        leftSprite = spr;
        leftName = n;
        leftDialog = t;
    }

    public void SetRightSideObjects(GameObject obj, SpriteRenderer spr, SpriteFontTextMesh n, TextMesh t)
    {
        right = obj;
        rightSprite = spr;
        rightName = n;
        rightDialog = t;
    }

    public void DialogEnd()
    {
        left.SetActive(false);
        right.SetActive(false);

        dialog = false;
    }

    public void SetScrollText()
    {
        if(speachSide == 0)
        {
            leftDialog.text += texts[characterSpeachPos][langague].text[charPos];
        }
        else if(speachSide == 1)
        {
            rightDialog.text += texts[characterSpeachPos][langague].text[charPos];
        }
    }

    public bool SetDialogInfo()
    {
        if(speachSide == 0)
            ChangeLeftColor(Color.gray);
        else if(speachSide == 1)
            ChangeRightColor(Color.gray);

        if(currDialog.Length <= scriptPos)
            return false;

        var curr = currDialog[scriptPos];

        texts = curr.scripts;
        speachSide = curr.side;
        characterSpeachPos = 0;
        _autoScrollTimer = texts[characterSpeachPos][langague].autoTimer;

        if(speachSide == 0)
        {
            if(curr.character != null)
            {
                leftName.SetText(curr.character.name[langague]);
                leftSprite.sprite = curr.character.graphic;
            }

            left.SetActive(true);
            ChangeLeftColor(Color.white);

            leftDialog.text = "";
        }
        else
        {
            if(curr.character != null)
            {
                rightName.SetText(curr.character.name[langague]);
                rightSprite.sprite = curr.character.graphic;
            }

            right.SetActive(true);
            ChangeRightColor(Color.white);

            rightDialog.text = "";
        }
        
        return true;
    }

    public void ShowDialog(string id, bool skip, bool auto = false)
    {
        dialog = true;
        scrollEnd = false;
        skipped = false;
        canSkip = skip;
        autoScroll = auto;
        currDialog = dialogs[id];
        charPos = 0;
        scriptPos = 0;
        scrollTimer = 0f;
        _autoScrollTimer = 0f;

        SetDialogInfo();
    }

    public void ChangeRightColor(Color color)
    {
        rightSprite.color = color;
        rightName.textColor = color;
        rightName.UpdateColor();
        rightDialog.color = color;
    }

    public void ChangeLeftColor(Color color)
    {
        leftSprite.color = color;
        leftName.textColor = color;
        leftName.UpdateColor();
        leftDialog.color = color;
    }

    public void ReadCharacter()
    {
        _charPics = ResourceManager.GetInstance().GetSpriteSet("UI/Characters");
        List<string> names = new List<string>();
        var strings = SplitReturn(charInfo.text);
        int len = strings.Length;

        for(int i = 0; i < len; ++i)
        {

            var line = strings[i].Split('/');
            string title = line[0];
            int graphic = int.Parse(line[1]);

            for(int j = 2; j < line.Length; ++j)
            {
                names.Add(line[j]);
            }

            CharacterInfo info = new CharacterInfo();
            info.name = names.ToArray();
            info.graphic = graphic == - 1 ? null : _charPics[graphic];

            characters.Add(title,info);
            names.Clear();
        }
    }

    public string SetNewLine(string s)
    {
        for(int i = newLineLength - 1; i < s.Length; i += newLineLength)
        {
            bool cut = false;
            for(int j = i; j >= 0; --j)
            {
                if(s[j] == ' ')
                {
                    s = s.Insert(j + 1,"\n");
                    cut = true;

                    break;
                }
            }

            if(!cut)
            {
                try
                {
                    var l = i + 2;
                    l = l > s.Length ? s.Length : l;
                    s = s.Insert(l,"\n");
                }
                catch
                {
                    Debug.Log(s);
                }
                
            }
        }

        return s;
    }

    public void ReadDialog()
    {
        List<ConversationInfo> info = new List<ConversationInfo>();
        var strings = SplitReturn(dialogInfo.text);
        int len = strings.Length;
        var line = strings[0].Split('/');

        for(int i = 0; i < len;)
        {
            string title = line[0];

            for(int j = i; j < len;)
            {
                List<TextInfo[]> scripts = new List<TextInfo[]>();
                List<TextInfo> langague = new List<TextInfo>();
                ConversationInfo con = new ConversationInfo();

                con.character = characters[line[1]];
                con.side = line[2] == string.Empty ? -1 : int.Parse(line[2]);

                for(int k = j; k < len;)
                {
                    int scriptLen = line.Length;
                    for(int l = 3; l < scriptLen; ++l)
                    {
                        var t = new TextInfo();
                        var ti = line[l].Split('%');//SetNewLine(line[l]).Split('%');
                        t.text = ti[0];

                        if(ti.Length > 1 && ti[1] != "")
                            t.autoTimer = float.Parse(ti[1]);

                        langague.Add(t);
                    }

                    scripts.Add(langague.ToArray());

                    j = ++k;

                    if(k >= len)
                        break;
                    
                    line = strings[k].Split('/');

                    if(line[1] != string.Empty || line[0] != string.Empty)
                    {
                        break;
                    }

                    langague.Clear();
                }

                con.scripts = scripts;

                info.Add(con);
                
                i = j;

                if(line[0] != string.Empty)
                {
                    break;
                }
            }

            dialogs.Add(title,info.ToArray());
            info.Clear();
        }
    }

    public string[] SplitReturn(string s)
    {
        return s.Replace("\r",string.Empty).Split('\n');
    }
}
