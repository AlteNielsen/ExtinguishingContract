using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class HelpSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset[] contract;
    [SerializeField] private VisualTreeAsset[] refCont;
    [SerializeField] private VisualTreeAsset[] refExt;
    [SerializeField] private VisualTreeAsset[] credits;

    private VisualElement[] screens;
    private ScrollView documentScroll;
    private ScrollView settingScroll;
    private VisualElement selectorPlank;

    private Label masterVolumeLabel;
    private Label bgmVolumeLabel;
    private Label sevolumeLabel;

    private float masterVolume;
    private float bgmVolume;
    private float seVolume;

    private List<VisualElement> displayBGs;
    private List<Label> displayLabels;
    private int displayMode;

    private VisualElement resolutonLeft;
    private Label resolutionValueLabel;
    private VisualElement resolutonRight;
    private int resolutionValue;    

    private VisualElement fader;

    private void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        SetupScreens();
        SetupScroll();
        selectorPlank = document.rootVisualElement.Q<VisualElement>("SelectorPlank");
        HelpSceneController();
        WriteText();
        fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
        if (GameSceneManager.helpIsSetting)
        {
            SwitchScreen(3);
        }
        else
        {
            SwitchScreen(0);
        }
        SettingSetup();
    }

    private void SetupScreens()
    {
        screens = new VisualElement[5];
        int lang = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).GetLang();
        screens[0] = contract[lang].Instantiate();
        screens[1] = refCont[lang].Instantiate();
        screens[2] = refExt[lang].Instantiate();
        screens[4] = credits[lang].Instantiate();
    }

    private void SetupScroll()
    {
        documentScroll = document.rootVisualElement.Q<ScrollView>("DocumentScroll");
        settingScroll = document.rootVisualElement.Q<ScrollView>("SettingScroll");
    }

    private void HelpSceneController()
    {
        List<Button> sideButtons = document.rootVisualElement.Query<Button>("SideButton").ToList();
        for(int i = 0; i <  sideButtons.Count; i++)
        {
            int j = i;
            sideButtons[i].clicked += () => SwitchScreen(j);
        }
        Button exit = document.rootVisualElement.Q<Button>("ExitButton");
        exit.clicked += () =>
        {
            Save();
            CulculateLibrary.SceneFadeOut(fader, GameSceneManager.BackFromHelp);
        };
    }

    private void WriteText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Help)[i];
        }
    }

    private void SwitchScreen(int index)
    {
        if (index == 3)
        {
            documentScroll.AddToClassList("non-display");
            settingScroll.RemoveFromClassList("non-display");
        }
        else
        {
            settingScroll.AddToClassList("non-display");
            documentScroll.RemoveFromClassList("non-display");
            documentScroll.contentContainer.Clear();
            documentScroll.contentContainer.Add(screens[index]);
        }

        for(int i = 0; i < screens.Length; i++)
        {
            selectorPlank.RemoveFromClassList("selector-pos-" + i);
        }
        selectorPlank.AddToClassList("selector-pos-" + index);
    }

    private void SettingSetup()
    {
        LangSetup();
        VolumeSetup();
        DisplaySettingSetup();
        ResolutionSetup();
    }

    private void LangSetup()
    {
        int lang = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).GetLang();
        ReadOnlySpan<string> words = WordDataBase.Word(WordDataBase.WordSelector.Language);
        VisualElement leftbb = document.rootVisualElement.Q<VisualElement>("LeftLangButtonBlock");
        if(lang == 0)
        {
            leftbb.AddToClassList("transparent");
        }
        else
        {
            Button leftButton = document.rootVisualElement.Q<Button>("LeftLangButton");
            leftButton.clicked += () => LangSwitch(false, lang);
        }
        VisualElement rightbb = document.rootVisualElement.Q<VisualElement>("RightLangButtonBlock");
        if(lang == words.Length - 1)
        {
            rightbb.AddToClassList("transparent");
        }
        else
        {
            Button leftButton = document.rootVisualElement.Q<Button>("RightLangButton");
            leftButton.clicked += () => LangSwitch(true, lang);
        }
        Label label = document.rootVisualElement.Q<Label>("LangName");
        label.text = words[lang]; 
    }

    private void LangSwitch(bool facing, int now)
    {
        float[] data = new float[6];
        SaveDataManager.Instance.GetData((int)SaveDataManager.SaveDataChunk.Setting, data);
        if(facing)
        {
            data[5] = (int)data[5] + 1;
        }
        else
        {
            data[5] = (int)data[5] - 1;
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.Setting, data);
        CulculateLibrary.SceneFadeOut(fader, () =>
        {
            ExtinguishingContract.ReloadLang();
            GameSceneManager.ReloadHelp();
        });
    }

    private void VolumeSetup()
    {
        ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).data.Span;

        masterVolume = datas[0];
        bgmVolume = datas[1];
        seVolume = datas[2];

        Slider master = document.rootVisualElement.Q<Slider>("MasterSlider");
        master.value = masterVolume;
        Slider bgm = document.rootVisualElement.Q<Slider>("BGMSlider");
        bgm.value = bgmVolume;
        Slider se = document.rootVisualElement.Q<Slider>("SESlider");
        se.value = seVolume;

        masterVolumeLabel = document.rootVisualElement.Q<Label>("MasterVolumeLabel");
        masterVolumeLabel.text = "" + CulculateLibrary.FloatToPercent(masterVolume);
        bgmVolumeLabel = document.rootVisualElement.Q<Label>("BGMVolumeLabel");
        bgmVolumeLabel.text = "" + CulculateLibrary.FloatToPercent(bgmVolume);
        sevolumeLabel = document.rootVisualElement.Q<Label>("SEVolumeLabel");
        sevolumeLabel.text = "" + CulculateLibrary.FloatToPercent(seVolume);

        master.RegisterValueChangedCallback(evt =>
        {
            masterVolume = evt.newValue;
            BGMManager.Instance.SetVolume(masterVolume * bgmVolume);
            SEManager.Instance.SetVolume(masterVolume * seVolume);
            masterVolumeLabel.text = "" + CulculateLibrary.FloatToPercent(masterVolume);
        });

        bgm.RegisterValueChangedCallback(evt =>
        {
            bgmVolume = evt.newValue;
            BGMManager.Instance.SetVolume(masterVolume * bgmVolume);
            bgmVolumeLabel.text = "" + CulculateLibrary.FloatToPercent(bgmVolume);
        });

        se.RegisterValueChangedCallback(evt =>
        {
            seVolume = evt.newValue;
            SEManager.Instance.SetVolume(masterVolume * seVolume);
            sevolumeLabel.text = "" + CulculateLibrary.FloatToPercent(seVolume);
        });
    }

    private void DisplaySettingSetup()
    {
        displayMode = (int)SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).data.Span[3];
        displayBGs = document.rootVisualElement.Query<VisualElement>("DisplayBG").ToList();
        displayLabels = document.rootVisualElement.Query<Label>("DisplayLabel").ToList();
        
        displayBGs[displayMode].RemoveFromClassList("bg-darkgray");
        displayBGs[displayMode].AddToClassList("bg-white");
        displayLabels[displayMode].RemoveFromClassList("color-white");
        displayLabels[displayMode].AddToClassList("color-black");

        List<Button> buttons = document.rootVisualElement.Query<Button>("DisplayButton").ToList();
        for(int i = 0; i < buttons.Count; i++)
        {
            int j = i;
            buttons[i].clicked += () => SelectDisplaySetting(j);
        }
    }

    private void SelectDisplaySetting(int index)
    {
        for(int i = 0; i < 3; i++)
        {
            displayBGs[i].AddToClassList("bg-darkgray");
            displayBGs[i].RemoveFromClassList("bg-white");
            displayLabels[i].AddToClassList("color-white");
            displayLabels[i].RemoveFromClassList("color-black");
        }
        displayBGs[index].RemoveFromClassList("bg-darkgray");
        displayBGs[index].AddToClassList("bg-white");
        displayLabels[index].RemoveFromClassList("color-white");
        displayLabels[index].AddToClassList("color-black");

        if (displayMode == index) return;

        displayMode = index;
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        Save();
    }

    private void ResolutionSetup()
    {
        resolutonLeft = document.rootVisualElement.Q<VisualElement>("ResolutionLeft");
        resolutionValueLabel = document.rootVisualElement.Q<Label>("ResolutionValue");
        resolutonRight = document.rootVisualElement.Q<VisualElement>("ResolutionRight");
        resolutionValue = (int)SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).data.Span[4];
        ResolutionButtonDisplay(resolutionValue);
        WriteResolutionValueLabel(resolutionValue);
        Button leftb = document.rootVisualElement.Q<Button>("ResolutionLeftButton");
        leftb.clicked += () => ResolutionSwitch(false);
        Button rightb = document.rootVisualElement.Q<Button>("ResolutionRightButton");
        rightb.clicked += () => ResolutionSwitch(true);
    }

    private void ResolutionSwitch(bool facing)
    {
        if(facing)
        {
            if(resolutionValue != ExtinguishingContract.ResolutionNum - 1)
            {
                resolutionValue++;
            }
            else
            {
                return;
            }
        }
        
        if(!facing)
        {
            if(resolutionValue != 0)
            {
                resolutionValue--;
            }
            else
            {
                return;
            }
        }

        Screen.SetResolution(ExtinguishingContract.GetResolution(resolutionValue).x, ExtinguishingContract.GetResolution(resolutionValue).y, Screen.fullScreenMode);

        ResolutionButtonDisplay(resolutionValue);
        WriteResolutionValueLabel(resolutionValue);
        Save();
    }

    private void ResolutionButtonDisplay(int index)
    {
        if (index == 0)
        {
            resolutonLeft.AddToClassList("transparent");
            resolutonRight.RemoveFromClassList("transparent");
        }
        else if (index == ExtinguishingContract.ResolutionNum - 1)
        {
            resolutonLeft.RemoveFromClassList("transparent");
            resolutonRight.AddToClassList("transparent");
        }
        else
        {
            resolutonLeft.RemoveFromClassList("transparent");
            resolutonRight.RemoveFromClassList("transparent");
        }
    }

    private void WriteResolutionValueLabel(int index)
    {
        resolutionValueLabel.text = ExtinguishingContract.GetResolution(index).x + " x " + ExtinguishingContract.GetResolution(index).y;
    }

    private void Save()
    {
        ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).data.Span;
        float[] save = new float[datas.Length];
        datas.CopyTo(save);
        save[0] = masterVolume;
        save[1] = bgmVolume;
        save[2] = seVolume;
        save[3] = displayMode;
        save[4] = resolutionValue;
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.Setting, save);
    }
}
