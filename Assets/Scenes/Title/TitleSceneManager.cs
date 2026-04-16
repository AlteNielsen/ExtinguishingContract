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

        methods = new Action[] {StartGame, NewGame, Continue, Help, Exit, Setting};
        buttons = document.rootVisualElement.Query<Button>().ToList();
        for(int i = 0; i < buttons.Count; i++)
        {
            buttons[i].clicked += methods[i];
        }

        sceneView = new TitleSceneView(document);
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

    }

    private void Continue()
    {

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
