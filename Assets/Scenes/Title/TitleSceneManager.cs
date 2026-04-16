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

    void Awake()
    {
        if (SaveDataManager.Instance == null)
        {
            ExtinguishingContract.GameSetup();
        }

        TitleSceneController();

        sceneView = new TitleSceneView(document);
        SaveDataInitialize();
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

    private void SaveDataInitialize()
    {
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.MapSelect);
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.IndicatorSelect);
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.UnitSelect);
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.UnitLevel);
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.FireMap);
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.UnitMap);
        SaveDataManager.Instance.Initialize((int)SaveDataManager.SaveDataChunk.UnitFacing);
    }

    public void OnClicked(int index)
    {
        methods[index]();
    }

    private void StartGame()
    {
        sceneView.SwitchScreen();
    }

    private void NewGame()
    {
        GameSceneManager.ToContract();
    }

    private void Continue()
    {
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        if (now.Span[0] < 0)
        {
            return;
        }
        GameSceneManager.ToHome();
    }

    private void Help()
    {
        GameSceneManager.ToHelp();
    }

    private void Exit()
    {
        GameSceneManager.QuitGame();
    }

    private void Setting()
    {
        GameSceneManager.ToHelp();
    }
}
