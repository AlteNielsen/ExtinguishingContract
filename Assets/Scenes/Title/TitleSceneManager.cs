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
        ExtinguishingContract.GameSetup();
        methods = new Action[] {StartGame, NewGame, Continue, Help, Exit, Setting};
        buttons = document.rootVisualElement.Query<Button>().ToList();
        for(int i = 0; i < buttons.Count; i++)
        {
            buttons[i].clicked += methods[i];
        }
        sceneView = new TitleSceneView(document);
    }

    void Update()
    {

    }

    public void OnClicked(int index)
    {
        methods[index]();
    }

    private void StartGame()
    {
        sceneView.Change(0, 1);
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
