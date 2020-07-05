using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : SingletonMono<OptionManager>
{
    private static readonly string _fileName = "Option.ini";

    public enum LanguageSet
    {
        Korean,
        Japanese,
        English,
    }

    public float volume_Bgm;
    public float volume_Fx;

    public int resolutionPos = 0;
    public int language = 0;

    public void Start()
    {
        SetSingleton(this);

        LoadSettings();
		UpdateOptions();
    }

    public void LoadSettings()
    {
        var list = IOManager.ReadiniFile(_fileName);
        if(list == null)
        {
            CreateSettingFile();
            return;
        }
        
        var op = list["option"];
        foreach(var item in op)
        {
            if(item.title == "bgmVol")
            {
                volume_Bgm = float.Parse(item.data);
            }
            else if(item.title == "fxVol")
            {
                volume_Fx = float.Parse(item.data);
            }
            else if(item.title == "resolution")
            {
                resolutionPos = int.Parse(item.data);
            }
            else if(item.title == "lang")
            {
                language = int.Parse(item.data);
            }
        }
    }

    public void UpdateOptions()
    {
        ScreenManager.GetInstance().SetScreenResolution(resolutionPos);
        SoundManager.instance.SetBGMVolume(volume_Bgm);
        SoundManager.instance.SetSEVolume(volume_Fx);

        if(DialogManager.instance != null)
            DialogManager.instance.langague = language;
    }

    public void SetResolution(int i) 
    {
        resolutionPos = i;
        ScreenManager.GetInstance().SetScreenResolution(resolutionPos);
    }
    public void SetLanguage(int i) 
    {
        language = i;
        DialogManager.instance.langague = i;
    }
    public void SetBGMVolume(float val) 
    {
        volume_Bgm = val;
        SoundManager.instance.SetBGMVolume(val);
    }
    public void SetFXVolume(float val) 
    {
        volume_Fx = val;
        SoundManager.instance.SetSEVolume(val);
    }

    public void SaveSettings()
    {
        List<string> data = new List<string>();

        data.Add("[option]");
        data.Add("bgmVol=" + volume_Bgm);
        data.Add("fxVol=" + volume_Fx);
        data.Add("resolution=" + resolutionPos);
        data.Add("lang=" + language);

        IOManager.WriteStringToFile_NoMark(data.ToArray(),_fileName);
    }

    public void CreateSettingFile()
    {
        volume_Bgm = 1f;
        volume_Fx = 1f;
        resolutionPos = 0;
        language = 0;

        SaveSettings();
    }
}
