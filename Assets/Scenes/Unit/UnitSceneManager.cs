using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UnitSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument baseDocument;
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitCard;
    [SerializeField] private VisualTreeAsset selectCard;
    private UnitSceneView sceneView;
    private float[] unitLevels;
    private bool[] unitIsSelected;

    private int basePressure;
    private float basePressureIncrease;
    private int deployLimit;
    private int pressureLimit;

    private VisualElement fader;

    private InputAction rightClick;
    private InputAction esc;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        ScrollViewSetup();
        sceneView = new UnitSceneView(document, baseDocument);
        unitLevels = new float[UnitDataBase.Datas.Length];
        unitIsSelected = new bool[UnitDataBase.Datas.Length];
        UnitSceneController();
        Load();
        CulculateControllValues();
        sceneView.WriteUnitPressure(basePressure, (int)basePressureIncrease);
        var (deployCounter, totalPressure, canGo) = JudgeCanGoStage();
        sceneView.DisplayDeployPanel(deployCounter, deployLimit);
        sceneView.DisplayTotalPressurePanel(totalPressure, pressureLimit);
        sceneView.DisplayGoButton(canGo);

        fader = document.rootVisualElement.Q<VisualElement>("Fader");
        CulculateLibrary.SceneFadeIn(fader);
    }

    private void ScrollViewSetup()
    {
        ScrollView unitScrollView = document.rootVisualElement.Q<ScrollView>("UnitScrollView");
        unitScrollView.contentContainer.Clear();
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            VisualElement card = unitCard.Instantiate();
            unitScrollView.contentContainer.Add(card);
        }

        ScrollView selectedScrollView = document.rootVisualElement.Q<ScrollView>("SelectedScrollView");
        selectedScrollView.contentContainer.Clear();
        for(int i = 0; i < UnitDataBase.Datas.Length;i++)
        {
            VisualElement card = selectCard.Instantiate();
            selectedScrollView.contentContainer.Add(card);
        }
    }

    private void UnitSceneController()
    {
        List<Button> levelSelectors = document.rootVisualElement.Query<Button>("LevelSelector").ToList();
        int maxLv = levelSelectors.Count / UnitDataBase.Datas.Length;
        for(int i = 0; i < levelSelectors.Count; i++)
        {
            int index = i / maxLv;
            int level = i % maxLv;
            levelSelectors[i].clicked += () =>
            {
                SEManager.PlaySelectSE();
                UnitLevelSelect(index, level);
            };
        }
        Button back = document.rootVisualElement.Q<Button>("BackButton");
        back.clicked += SEManager.PlayCancelSE;
        back.clicked += BackToHome;

        List<Button> unitSelectors = document.rootVisualElement.Query<Button>("UnitSelector").ToList();
        for(int i = 0; i < unitSelectors.Count;i++)
        {
            int index = i;
            unitSelectors[i].clicked += () => 
            { 
                SEManager.PlaySelectSE(); 
                UnitSelect(index); 
            };
        }
        Button go = document.rootVisualElement.Q<Button>("GoButton");
        go.clicked += SEManager.PlayDecideSE;
        go.clicked += GoToStage;

        rightClick = new InputAction(binding: "<Mouse>/rightButton");
        rightClick.performed += ctx =>
        {
            SEManager.PlayCancelSE();
            BackToHome();
        };
        rightClick.Enable();

        esc = new InputAction(binding: "<Keyboard>/escape");
        esc.performed += ctx =>
        {
            SEManager.PlayCancelSE();
            BackToHome();
        };
        esc.Enable();
    }

    private void UnitSelect(int index)
    {
        unitIsSelected[index] = !unitIsSelected[index];
        sceneView.UnitSelect(index, unitLevels.AsSpan(), unitIsSelected.AsSpan());
        var(deployCounter, totalPressure, canGo) = JudgeCanGoStage();
        sceneView.DisplayDeployPanel(deployCounter, deployLimit);
        sceneView.DisplayTotalPressurePanel(totalPressure, pressureLimit);
        sceneView.DisplayGoButton(canGo);
    }

    private void UnitLevelSelect(int index, int level)
    {
        unitLevels[index] = level;
        sceneView.UnitLevelChange(index, level);
        sceneView.UnitSelect(index, unitLevels.AsSpan(), unitIsSelected.AsSpan());
        var (deployCounter, totalPressure, canGo) = JudgeCanGoStage();
        sceneView.DisplayDeployPanel(deployCounter, deployLimit);
        sceneView.DisplayTotalPressurePanel(totalPressure, pressureLimit);
        sceneView.DisplayGoButton(canGo);
    }

    private void BackToHome()
    {
        Save();
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToHome);
    }

    private void GoToStage()
    {
        var (_, _, canGo) = JudgeCanGoStage();
        if (!canGo) return;
        Save(); 
        CulculateLibrary.SceneFadeOut(fader, GameSceneManager.ToStageLoading);
    }

    private void Load()
    {
        SaveDataManager.Instance.GetData((int)SaveDataManager.SaveDataChunk.UnitLevel, unitLevels);
        float[] isSelected = new float[UnitDataBase.Datas.Length];
        SaveDataManager.Instance.GetData((int)SaveDataManager.SaveDataChunk.UnitSelect, isSelected);
        for(int i = 0; i < isSelected.Length; i++)
        {
            if (isSelected[i] > 0.5f)
            {
                unitIsSelected[i] = false;
            }
            else
            {
                unitIsSelected[i] = true;
            }
        }
        for(int i = 0; i < unitIsSelected.Length; i++)
        {
            UnitSelect(i);
        }
        for(int i = 0; i < unitLevels.Length; i++)
        {
            UnitLevelSelect(i, (int)unitLevels[i]);
        }
    }

    private void Save()
    {
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.UnitSelect, CulculateLibrary.SwitchBoolToFloat(unitIsSelected));
        SaveDataManager.Instance.SetData((int)SaveDataManager.SaveDataChunk.UnitLevel, unitLevels);
    }

    private void CulculateControllValues()
    {

        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        float[] baseValue = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        int maxIndicatorLv = indicators.Length / ExtinguishingContract.EIndicatorNum;
        var (bs, increase) = CulculateLibrary.CulculatePressures();
        basePressure = bs;
        basePressureIncrease = increase;
        
        ReadOnlySpan<float> map = SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span;
        deployLimit = MapDataBase.Datas[(int)map[0]].Data.height;
        for(int i = 0; i < maxIndicatorLv; i++)
        {
            if(indicators[i] > 0.5f)
            {
                deployLimit += (int)baseValue[0] + CulculateLibrary.GetIndicatorLevelIncrease(0, i);
            }
        }

        pressureLimit = Config.Data.UsablePressure;
        for (int i = 0; i < maxIndicatorLv; i++)
        {
            if (indicators[(maxIndicatorLv * 3) + i] > 0.5f)
            {
                pressureLimit += (int)baseValue[3] + CulculateLibrary.GetIndicatorLevelIncrease(3, i);
            }
        }
    }

    private (int deploy, int prssure, bool canGo) JudgeCanGoStage()
    {
        int deployCounter = 0;
        bool isDeploy = true;
        for(int i = 0; i < unitIsSelected.Length; i++)
        {
            if(unitIsSelected[i])
            {
                deployCounter++;
            }
        }
        if(deployCounter > deployLimit)
        {
            isDeploy = false;
        }

        int totalPressure = 0;
        bool isPress = true;
        for (int i = 0; i < unitIsSelected.Length; i++)
        {
            if (unitIsSelected[i])
            {
                totalPressure += (int)(basePressure + (basePressureIncrease * unitLevels[i]));
            }
        }
        if(totalPressure > pressureLimit)
        {
            isPress = false;
        }

        return (deployCounter, totalPressure, isDeploy &&  isPress);
    }
}
