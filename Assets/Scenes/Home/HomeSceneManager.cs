using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HomeSceneManager : MonoBehaviour
{
    private const int tileCount = 96;

    [SerializeField] private UIDocument document;
    [SerializeField] private UIDocument baseDocument;
    [SerializeField] private VisualTreeAsset grid;
    private HomeSceneView sceneView;
    private int blockSelector;
    private bool[] isIndicatorSelected = new bool[ExtinguishingContract.IndicatorChoicesNum];
    private VisualElement fader;

    private InputAction rightClick;
    private InputAction esc;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        GridSstup();
        sceneView = new HomeSceneView(document, baseDocument);
        HomeSceneController();
        RestoreSituationFromSaveData();

        fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
    }

    private void GridSstup()
    {
        VisualElement panel =  document.rootVisualElement.Q<VisualElement>("MapGrids");
        panel.contentContainer.Clear();
        for (int i = 0; i < tileCount; i++)
        {
            VisualElement ve = grid.Instantiate();
            panel.contentContainer.Add(ve);
        }
    }

    private void HomeSceneController()
    {
        List<Button> blockButtons = document.rootVisualElement.Query<Button>("BlockSelectorButton").ToList();
        for(int i = 0; i < blockButtons.Count; i++)
        {
            int j = i;
            blockButtons[i].clicked += () =>
            {
                SEManager.PlaySelectSE();
                BlockSelect(j);
            };
        }
        List<Button> indicatorButtons = document.rootVisualElement.Query<Button>("IndicatorButton").ToList();
        for (int i = 0; i < indicatorButtons.Count; i++)
        {
            int j = i;
            indicatorButtons[i].clicked += () => 
            { 
                SEManager.PlaySelectSE(); 
                IndicatorSelect(j); 
            };
        }
        Button mapButton = document.rootVisualElement.Q<Button>("MapButton");
        mapButton.clicked += MapDisplaySwitch;
        mapButton.clicked += SEManager.PlaySelectSE;
        Button nextButton = document.rootVisualElement.Q<Button>("NextButton");
        nextButton.clicked += ToUnitScene;
        nextButton.clicked += SEManager.PlayDecideSE;
        Button backButton = document.rootVisualElement.Q<Button>("BackButton");
        backButton.clicked += BackToTitle;
        backButton.clicked += SEManager.PlayCancelSE;

        rightClick = new InputAction(binding: "<Mouse>/rightButton");
        rightClick.performed += ctx =>
        {
            SEManager.PlayCancelSE();
            BackToTitle();
        };
        rightClick.Enable();

        esc = new InputAction(binding: "<Keyboard>/escape");
        esc.performed += ctx =>
        {
            SEManager.PlayCancelSE();
            BackToTitle();
        };
        esc.Enable();
    }

    private void RestoreSituationFromSaveData()
    {
        BlockSelect((int)SaveDataManager.Instance.Access<MapSelectChunk>(((int)SaveDataManager.SaveDataChunk.MapSelect)).data.Span[0]);
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<BlockIndicatorChunk>(((int)SaveDataManager.SaveDataChunk.BlockIndicator)).data.Span;
        ReadOnlySpan<float> selected = SaveDataManager.Instance.Access<IndicatorSelectChunk>(((int)SaveDataManager.SaveDataChunk.IndicatorSelect)).data.Span;
        int counter = 0;
        for(int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] > 0.5f)
            {
                if (selected[i]  < 0.5f)
                {
                    isIndicatorSelected[counter] = true;
                }
                counter++;
            }
        }
        for(int i = 0; i < isIndicatorSelected.Length; i++)
        {
            IndicatorSelect(i);
        }
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

    private void BackToTitle()
    {
        SaveProcess();
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToTitle);
    }

    private void ToUnitScene()
    {
        if (SaveDataManager.Instance.Access<BurningSituationChunk>(((int)SaveDataManager.SaveDataChunk.BurningSituation)).data.Span[blockSelector] < 0.5f)
        {
            return;
        }
        SaveProcess();
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToUnit);
    }

    private void SaveProcess()
    {
        float[] result = new float[ExtinguishingContract.EIndicatorNum * Config.Data.IndicatorMaxLv];
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<BlockIndicatorChunk>(((int)SaveDataManager.SaveDataChunk.BlockIndicator)).data.Span;
        int counter = 0;
        for (int i = 0; i < result.Length; i++)
        {
            if (indicators[i] > 0.5f)
            {
                if (isIndicatorSelected[counter])
                {
                    result[i] = 1;
                }
                counter++;
            }
        }
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.IndicatorSelect, result);
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.MapSelect, new float[] { blockSelector });
    }
}
