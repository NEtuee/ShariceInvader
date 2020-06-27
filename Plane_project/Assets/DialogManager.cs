using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : SingletonMono<DialogManager>
{
    public class ConversationInfo
    {   
        public CharacterInfo character;
        public int side;
        public List<string[]> scripts;
    }

    public class CharacterInfo
    {
        public string[] name;
        public Sprite graphic;
    }

    public GameObject left;
    public SpriteRenderer leftSprite;
    public TextMesh leftName;
    public TextMesh leftDialog;

    public GameObject right;
    public SpriteRenderer rightSprite;
    public TextMesh rightName;
    public TextMesh rightDialog;

    public TextAsset dialogInfo;
    public TextAsset charInfo;

    public Dictionary<string, CharacterInfo> characters = new Dictionary<string, CharacterInfo>();
    public Dictionary<string, ConversationInfo[]> dialogs = new Dictionary<string, ConversationInfo[]>(); 

    public int langague = 0;

    public bool dialog = false;
    public bool scrollEnd = false;
    public int speachSide = -1;
    public int newLineLength = 1;
    public float textScrollTime;
    

    private int scriptPos;
    private int charPos;
    private int characterSpeachPos;
    private float scrollTimer = 0f;
    private ConversationInfo[] currDialog;
    private List<string[]> texts;
    private string scrollText;


    void Awake()
    {
        SetSingleton(this);

        ReadCharacter();
        ReadDialog();
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

                    if(texts[characterSpeachPos][langague].Length <= charPos)
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

            if(ControllerEx.GetInstance().KeyUp("MainAttack"))
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
                            left.SetActive(false);
                            right.SetActive(false);

                            GameMain.instance.update = true;
                            dialog = false;
                        }
                    }
                    else
                    {
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
                        leftDialog.text = texts[characterSpeachPos][langague];
                    }
                    else if(speachSide == 1)
                    {
                        rightDialog.text = texts[characterSpeachPos][langague];
                    }
                    ++characterSpeachPos;
                    scrollEnd = true;
                }
            }
            
        }
    }

    public void SetScrollText()
    {
        if(speachSide == 0)
        {
            leftDialog.text += texts[characterSpeachPos][langague][charPos];
        }
        else if(speachSide == 1)
        {
            rightDialog.text += texts[characterSpeachPos][langague][charPos];
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

        if(speachSide == 0)
        {
            if(curr.character != null)
            {
                leftName.text = curr.character.name[langague];
            }

            left.SetActive(true);
            ChangeLeftColor(Color.white);

            leftDialog.text = "";
        }
        else
        {
            if(curr.character != null)
            {
                rightName.text = curr.character.name[langague];
            }

            right.SetActive(true);
            ChangeRightColor(Color.white);

            rightDialog.text = "";
        }
        
        return true;
    }

    public void ShowDialog(string id, bool auto = false)
    {
        dialog = true;
        GameMain.instance.update = false;
        scrollEnd = false;
        currDialog = dialogs[id];
        charPos = 0;
        scriptPos = 0;
        scrollTimer = 0f;

        SetDialogInfo();
    }

    public void ChangeRightColor(Color color)
    {
        //rightSprite.color = color;
        rightName.color = color;
        rightDialog.color = color;
    }

    public void ChangeLeftColor(Color color)
    {
        //leftSprite.color = color;
        leftName.color = color;
        leftDialog.color = color;
    }

    public void ReadCharacter()
    {
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
            //info.graphic = graphic;

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
                s = s.Insert(i + 2,"\n");
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
                List<string[]> scripts = new List<string[]>();
                List<string> langague = new List<string>();
                ConversationInfo con = new ConversationInfo();

                con.character = characters[line[1]];
                con.side = line[2] == string.Empty ? -1 : int.Parse(line[2]);

                for(int k = j; k < len;)
                {
                    int scriptLen = line.Length;
                    for(int l = 3; l < scriptLen; ++l)
                    {
                        langague.Add(SetNewLine(line[l]));
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
