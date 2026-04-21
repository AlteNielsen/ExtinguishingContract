using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private HomeSceneView sceneView;
    private int blockSelector;
    private bool[] isIndicatorSelected = new bool[ExtinguishingContract.IndicatorChoicesNum];

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        sceneView = new HomeSceneView(document);
        HomeSceneController();
        BlockSelect(0);
    }

    private void HomeSceneController()
    {
        List<Button> blockButtons = document.rootVisualElement.Query<Button>("BlockSelectorButton").ToList();
        for(int i = 0; i < blockButtons.Count; i++)
        {
            int j = i;
            blockButtons[i].clicked += () => BlockSelect(j);
        }
        List<Button> indicatorButtons = document.rootVisualElement.Query<Button>("IndicatorButton").ToList();
        for (int i = 0; i < indicatorButtons.Count; i++)
        {
            int j = i;
            indicatorButtons[i].clicked += () => IndicatorSelect(j);
        }
        Button mapButton = document.rootVisualElement.Q<Button>("MapButton");
        mapButton.clicked += MapDisplaySwitch;
    }

    private void BlockSelect(int index)
    {
        blockSelector = index;
        sceneView.BlockSelect(blockSelector);
    }

    private void IndicatorSelect(int index)
    {
        isIndicatorSelected[index] = !isIndicatorSelected[index];
        sceneView.IndicatorSelect(index, isIndicatorSelected.AsSpan());
    }

    private void MapDisplaySwitch()
    {
        sceneView.MapDisplay();
    }
}
