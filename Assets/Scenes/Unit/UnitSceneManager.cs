using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class UnitSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitCard;
    [SerializeField] private VisualTreeAsset selectCard;
    private UnitSceneView sceneView;
    private int[] unitLevels;
    private bool[] unitIsSelected;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        ScrollViewSetup();
        sceneView = new UnitSceneView(document);
        unitLevels = new int[UnitDataBase.Datas.Length];
        unitIsSelected = new bool[UnitDataBase.Datas.Length];
        UnitSceneController();
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            UnitLevelSelect(i, 0);
        }
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
            levelSelectors[i].clicked += () => UnitLevelSelect(index, level);
        }
        Button back = document.rootVisualElement.Q<Button>("BackButton");
        back.clicked += BackToHome;
        List<Button> unitSelectors = document.rootVisualElement.Query<Button>("UnitSelector").ToList();
        for(int i = 0; i < unitSelectors.Count;i++)
        {
            int index = i;
            unitSelectors[i].clicked += () => UnitSelect(index);
        }
    }

    private void UnitSelect(int index)
    {
        unitIsSelected[index] = !unitIsSelected[index];
        sceneView.UnitSelect(index, unitLevels.AsSpan(), unitIsSelected.AsSpan());
    }

    private void UnitLevelSelect(int index, int level)
    {
        unitLevels[index] = level;
        sceneView.UnitLevelChange(index, level);
        sceneView.UnitSelect(index, unitLevels.AsSpan(), unitIsSelected.AsSpan());
    }

    private void BackToHome()
    {
        GameSceneManager.ToHome();
    }
}
