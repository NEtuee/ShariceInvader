using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUIControll : MonoBehaviour
{
    public UISortManager manager;


    public SpriteFontTextMesh dialog_title;
    public TextMesh dialog_text;
    public GameObject dialog;
    public SpriteRenderer dialog_sprite;


    public UIProgressBar bgm;
    public UIProgressBar fx;
    public UISelectMenu resol;
    public UISelectMenu lang;

    public UIButtonToggleGroup reGroup;
    public UIButtonToggleGroup laGroup;

    private string[] dialogCodes = {
        "dialog_stting_0",
        "dialog_stting_1"
    };

    public void Awake()
    {
        manager.activeEvent.AddListener(delegate{
            DialogSetup();
        });

        manager.deactiveEvent.AddListener(delegate{
            DialogManager.instance.DialogEnd();
            GameMain.instance.SetInGameDialog();
            OptionManager.instance.SaveSettings();
        });
    }

    public void Start()
    {
        bgm.SetValue(OptionManager.instance.volume_Bgm);
        fx.SetValue(OptionManager.instance.volume_Fx);

        int re = OptionManager.instance.resolutionPos;
        int la = OptionManager.instance.language;

        resol.MenuSelect(re,false);
        lang.MenuSelect(la,false);

        reGroup.SelectUI(re);
        laGroup.SelectUI(la);
    }

    public void DialogSetup()
    {
        DialogManager.instance.SetRightSideObjects(dialog,dialog_sprite,dialog_title,dialog_text);
        DialogManager.instance.ShowDialog(dialogCodes[Random.Range(0,dialogCodes.Length)],false,true);
    }

    public void BGMValueChange()
    {
        OptionManager.instance.SetBGMVolume(bgm.progress);
    }
    public void FXValueChange()
    {
        OptionManager.instance.SetFXVolume(fx.progress);
    }
    public void ResolutionValueChange(int i)
    {
        OptionManager.instance.SetResolution(i);
    }
    public void LanguageValueChange(int i)
    {
        OptionManager.instance.SetLanguage(i);
    }

}
