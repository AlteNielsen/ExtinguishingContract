using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public class ContractSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private ContractSceneView sceneView;
    private List<Button> indicatorButtons;
    private float[] indicatorLevels = new float[9];
    private Button sign;
    private Button reconsider;

    void Awake()
    {
        sceneView = new ContractSceneView(document);
        ContractSceneController();
        for(int i = 0; i < 9; i++)
        {
            indicatorLevels[i] = 1;
            IndicatorButtonClicked(i, 0);
        }
    }

    private void ContractSceneController()
    {
        indicatorButtons = document.rootVisualElement.Query<Button>("IndicatorButton").ToList();
        int maxLv = 10;
        for(int i = 0; i < indicatorButtons.Count; i++)
        {
            int j = i;
            indicatorButtons[i].clicked += () => IndicatorButtonClicked(j / maxLv, j % maxLv);
        }
        sign = document.rootVisualElement.Q<Button>("Sign");
        sign.clicked += SignClicked;
        reconsider = document.rootVisualElement.Q<Button>("Reconsider");
        reconsider.clicked += ReconsiderClicked;
    }

    private void IndicatorButtonClicked(int indicator, int lv)
    {
        if(lv + 1 == 10)
        {
            indicatorLevels[indicator] = 0;
        }
        else
        {
            indicatorLevels[indicator] = lv + 1;
        }
        sceneView.IndicatorButtonChange(indicator, lv);
        ReadOnlySpan<float> data = indicatorLevels;
        sceneView.UpdateContractPreview(data);
        sceneView.UpdateContractInfo(data);
    }

    private void SignClicked()
    {
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.NowID, indicatorLevels);
        GameSceneManager.ToGameLoading();
    }

    private void ReconsiderClicked()
    {
        GameSceneManager.ToTitle();
    }
}
