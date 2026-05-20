using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ContractSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private ContractSceneView sceneView;
    private List<Button> indicatorButtons;
    private float[] indicatorLevels = new float[9];
    private Button sign;
    private Button reconsider;

    private VisualElement fader;

    private const int randomLowMaxLv = 5;

    private InputAction rightClick;
    private InputAction esc;

    void Awake()
    {
        sceneView = new ContractSceneView(document);
        ContractSceneController();
        for(int i = 0; i < ExtinguishingContract.CIndicatorNum; i++)
        {
            indicatorLevels[i] = 1;
            IndicatorButtonClicked(i, 0);
        }

        fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
    }

    private void ContractSceneController()
    {
        indicatorButtons = document.rootVisualElement.Query<Button>("IndicatorButton").ToList();
        for(int i = 0; i < indicatorButtons.Count; i++)
        {
            int j = i;
            indicatorButtons[i].clicked += () =>
            {
                SEManager.PlaySelectSE();
                IndicatorButtonClicked(j / ExtinguishingContract.CIndicatorMaxLv, j % ExtinguishingContract.CIndicatorMaxLv);
            };
        }
        sign = document.rootVisualElement.Q<Button>("Sign");
        sign.clicked += SignClicked;
        reconsider = document.rootVisualElement.Q<Button>("Reconsider");
        reconsider.clicked += ReconsiderClicked;
        
        document.rootVisualElement.Q<Button>("RandomLow").clicked += () => IndicatorRandomSelect(randomLowMaxLv);
        document.rootVisualElement.Q<Button>("RandomHigh").clicked += () => IndicatorRandomSelect(ExtinguishingContract.CIndicatorMaxLv);

        rightClick = new InputAction(binding: "<Mouse>/rightButton");
        rightClick.performed += ctx =>
        {
            ReconsiderClicked();
        };
        rightClick.Enable();

        esc = new InputAction(binding: "<Keyboard>/escape");
        esc.performed += ctx =>
        {
            ReconsiderClicked();
        };
        esc.Enable();
    }

    private void IndicatorRandomSelect(int maxLv)
    {
        SEManager.PlaySelectSE();
        for (int i = 0; i < ExtinguishingContract.CIndicatorNum; i++)
        {
            int dice = UnityEngine.Random.Range(0, maxLv);
            IndicatorButtonClicked(i, dice);
        }
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
        SEManager.PlayDecideSE();
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.NowID, indicatorLevels);
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToGameLoading);
    }

    private void ReconsiderClicked()
    {
        SEManager.PlayCancelSE();
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToTitle);
    }
}
