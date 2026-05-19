using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

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
        if(GameSceneManager.helpIsSetting)
        {
            SwitchScreen(3);
        }
        else
        {
            SwitchScreen(0);
        }
    }

    private void SetupScreens()
    {
        screens = new VisualElement[5];
        int lang = (int)SaveDataManager.Instance.Access<SettingChunk>((int)SaveDataManager.SaveDataChunk.Setting).GetLang();
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
        exit.clicked += GameSceneManager.BackFromHelp;
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
}
