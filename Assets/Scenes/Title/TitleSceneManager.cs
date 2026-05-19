using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private List<Button> buttons;
    private Action[] methods;

    private TitleSceneView sceneView;
    private VisualElement fader;

    void Awake()
    {
        bool isStarted = true;
        if (SaveDataManager.Instance == null)
        {
            ExtinguishingContract.GameSetup();
            isStarted = false;
        }

        TitleSceneController();

        sceneView = new TitleSceneView(document);
        SaveDataManager.Instance.TitleSceneSaveDataInitialize();
        if(isStarted)
        {
            sceneView.SwitchScreen();
        }
        fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
    }

    private void TitleSceneController()
    {
        methods = new Action[] { StartGame, NewGame, Continue, Help, Exit, Setting };
        buttons = document.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].clicked += methods[i];
        }
    }

    public void OnClicked(int index)
    {
        methods[index]();
    }

    private void StartGame()
    {
        SEManager.PlaySelectSE();
        sceneView.SwitchScreen();
    }

    private void NewGame()
    {
        SEManager.PlayDecideSE();
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToContract);
    }

    private void Continue()
    {
        SEManager.PlayDecideSE();
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        if (now.Span[0] < 0)
        {
            return;
        }
        if(CulculateLibrary.IsGoodEnding())
        {
            CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToEnding);
        }
        else 
        {
            CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToHome);
        }
    }

    private void Help()
    {
        SEManager.PlayDecideSE();
        CulculateLibrary.SceneFadeOut(fader, () => GameSceneManager.ToHelp(GameScenes.Title, false));
    }

    private void Exit()
    {
        SEManager.PlayDecideSE();
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.QuitGame);
    }

    private void Setting()
    {
        SEManager.PlayDecideSE();
        CulculateLibrary.SceneFadeOut(fader, () => GameSceneManager.ToHelp(GameScenes.Title, true));
    }
}
