using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ContractSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    private ContractSceneView sceneView;
    private List<Button> indicatorButtons;

    void Awake()
    {
        sceneView = new ContractSceneView(document);
        ContractSceneController();
        for(int i = 0; i < 9; i++)
        {
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
    }

    private void IndicatorButtonClicked(int indicator, int lv)
    {
        sceneView.IndicatorButtonChange(indicator, lv);
    }
}
