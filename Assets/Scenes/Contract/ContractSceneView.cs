using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ContractSceneView
{
    private UIDocument document;
    private List<VisualElement> indicatorButtonBG;
    private List<Label> indicatorButtonText;
    private Label id;
    private Label grade;

    public ContractSceneView(UIDocument doc) 
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        document = doc;
        indicatorButtonBG = document.rootVisualElement.Query<VisualElement>("IndicatorButtonBG").ToList();
        indicatorButtonText = document.rootVisualElement.Query<Label>("IndicatorButtonText").ToList();
        id = document.rootVisualElement.Q<Label>("ID");
        grade = document.rootVisualElement.Q<Label>("Grade");
        WriteText();
    }

    private void WriteText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Contract)[i];
        }
    }

    public void IndicatorButtonChange(int indicator, int lv)
    {
        int maxLv = 10;
        for(int i = 0; i < maxLv; i++)
        {
            indicatorButtonBG[indicator * maxLv + i].RemoveFromClassList("bg-white");
            indicatorButtonBG[indicator * maxLv + i].AddToClassList("bg-darkgray");
            indicatorButtonText[indicator * maxLv + i].RemoveFromClassList("color-black");
            indicatorButtonText[indicator * maxLv + i].AddToClassList("color-white");
        }
        indicatorButtonBG[indicator * maxLv + lv].RemoveFromClassList("bg-darkgray");
        indicatorButtonBG[indicator * maxLv + lv].AddToClassList("bg-white");
        indicatorButtonText[indicator * maxLv + lv].RemoveFromClassList("color-white");
        indicatorButtonText[indicator * maxLv + lv].AddToClassList("color-black");
    }
    
    public void UpdateContractView(ReadOnlySpan<float> indicatorLevels)
    {
        id.text = "" + indicatorLevels[0] + indicatorLevels[1] + indicatorLevels[2] + "-" + indicatorLevels[3] + indicatorLevels[4] + indicatorLevels[5] + "-" + indicatorLevels[6] + indicatorLevels[7] + indicatorLevels[8];
        grade.text = "" + CulculateLibrary.ContractGrade(indicatorLevels);
    }
}
