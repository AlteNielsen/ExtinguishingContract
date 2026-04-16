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
        sceneView.SwitchScreen();
    }

    private void NewGame()
    {
        GameSceneManager.TitleToContract();
    }

    private void Continue()
    {
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        if (now.Span[0] < 0)
        {
            return;
        }
        GameSceneManager.TitleToHome();
    }

    private void Help()
    {

    }

    private void Exit()
    {

    }

    private void Setting()
    {

    }
}
