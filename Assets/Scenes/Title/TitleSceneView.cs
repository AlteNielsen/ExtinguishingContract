using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneController
{
    private Button startButton;
    private Button newGameButton;
    private Button continueGameButton;
    private Button helpButton;
    private Button exitButton;
    private Button settingButton;

    public TitleSceneController(TitleSceneManager manager, UIDocument doc)
    {
        startButton = doc.rootVisualElement.Q<Button>("StartButton");
        newGameButton = doc.rootVisualElement.Q<Button>("NewGameButton");
        continueGameButton = doc.rootVisualElement.Q<Button>("ContinueButton");
        helpButton = doc.rootVisualElement.Q<Button>("HelpButton");
        exitButton = doc.rootVisualElement.Q<Button>("ExitButton");
        settingButton = doc.rootVisualElement.Q<Button>("SettingButton");
        startButton.clicked += () => manager.OnClicked(0);
        newGameButton.clicked += () => manager.OnClicked(1);
        continueGameButton.clicked += () => manager.OnClicked(2);
        helpButton.clicked += () => manager.OnClicked(3);
        exitButton.clicked += () => manager.OnClicked(4);
        settingButton.clicked += () => manager.OnClicked(5);
    }
}

public class TitleSceneView
{
    private UIDocument document;
    private readonly string[] textKeys = new string[] { "title_press_start", "title_id_txt_left", "title_id_txt_right", "title_max_grade", "title_now_grade", "title_start", "title_continue", "title_help", "title_exit" };
    private readonly Action<float>[] methods;

    public TitleSceneView(UIDocument doc)
    {
        document = doc;
        WriteText();
        WriteValue();
        methods = new Action<float>[] { SwitchScreen };
    }

    private void WriteText()
    {
        document.rootVisualElement.Q<Label>("TextPressStart").text = TextDataBase.Text[textKeys[0]];
    }

    private void WriteValue()
    {

    }

    public void Change(int index, float info)
    {
        methods[index](info);
    }

    private void SwitchScreen(float info)
    {
        if(info < 0.5f)
        {
            document.rootVisualElement.Q<VisualElement>("StartScreen").style.display = DisplayStyle.Flex;
            document.rootVisualElement.Q<VisualElement>("TitleScreen").style.display = DisplayStyle.None;
        }
        else if(info > 0.5f)
        {
            document.rootVisualElement.Q<VisualElement>("StartScreen").style.display = DisplayStyle.None;
            document.rootVisualElement.Q<VisualElement>("TitleScreen").style.display = DisplayStyle.Flex;
        }
    }
}
