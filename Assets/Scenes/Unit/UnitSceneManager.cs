using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UnitSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset unitCard;
    private UnitSceneView sceneView;
    private int[] unitLevels;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        ScrollViewSetup();
        sceneView = new UnitSceneView(document);
        unitLevels = new int[UnitDataBase.Datas.Length];
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
    }

    private void UnitLevelSelect(int index, int level)
    {
        unitLevels[index] = level;
        sceneView.UnitLevelChange(index, level);
    }
}
