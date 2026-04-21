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
        RestoreSituationFromSaveData();
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
        Button nextButton = document.rootVisualElement.Q<Button>("NextButton");
        nextButton.clicked += ToUnitScene;
        Button backButton = document.rootVisualElement.Q<Button>("BackButton");
        backButton.clicked += BackToTitle;
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
        GameSceneManager.ToTitle();
    }

    private void ToUnitScene()
    {
        if (SaveDataManager.Instance.Access<BurningSituationChunk>(((int)SaveDataManager.SaveDataChunk.BurningSituation)).data.Span[blockSelector] < 0.5f)
        {
            return;
        }
        SaveProcess();
        GameSceneManager.ToUnit();
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
