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
        LangSetup();
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
        exit.clicked += () => CulculateLibrary.SceneFadeOut(fader, GameSceneManager.BackFromHelp);
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
}
