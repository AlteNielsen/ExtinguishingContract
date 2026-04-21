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
    private List<Label> previews;

    public ContractSceneView(UIDocument doc) 
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        document = doc;
        indicatorButtonBG = document.rootVisualElement.Query<VisualElement>("IndicatorButtonBG").ToList();
        indicatorButtonText = document.rootVisualElement.Query<Label>("IndicatorButtonText").ToList();
        id = document.rootVisualElement.Q<Label>("ID");
        grade = document.rootVisualElement.Q<Label>("Grade");
        previews = document.rootVisualElement.Query<Label>("Preview").ToList();
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
        for(int i = 0; i < ExtinguishingContract.CIndicatorMaxLv; i++)
        {
            indicatorButtonBG[indicator * ExtinguishingContract.CIndicatorMaxLv + i].RemoveFromClassList("bg-white");
            indicatorButtonBG[indicator * ExtinguishingContract.CIndicatorMaxLv + i].AddToClassList("bg-darkgray");
            indicatorButtonText[indicator * ExtinguishingContract.CIndicatorMaxLv + i].RemoveFromClassList("color-black");
            indicatorButtonText[indicator * ExtinguishingContract.CIndicatorMaxLv + i].AddToClassList("color-white");
        }
        indicatorButtonBG[indicator * ExtinguishingContract.CIndicatorMaxLv + lv].RemoveFromClassList("bg-darkgray");
        indicatorButtonBG[indicator * ExtinguishingContract.CIndicatorMaxLv + lv].AddToClassList("bg-white");
        indicatorButtonText[indicator * ExtinguishingContract.CIndicatorMaxLv + lv].RemoveFromClassList("color-white");
        indicatorButtonText[indicator * ExtinguishingContract.CIndicatorMaxLv + lv].AddToClassList("color-black");
    }
    
    public void UpdateContractPreview(ReadOnlySpan<float> indicatorLevels)
    {
        float[] values = CulculateLibrary.IndicatorBaseValues(indicatorLevels);
        previews[0].text = "" + values[0];
        previews[1].text = "+" + values[1];
        previews[2].text = "+" + values[2];
        previews[3].text = "" + values[3];
        previews[4].text = "+ " + values[4];
        previews[5].text = "+" + values[5];

        previews[6].text = "" + (int)(values[6] * 100) + "%";
        previews[7].text = "" + values[7];
        previews[8].text = "" + values[8];
    }

    public void UpdateContractInfo(ReadOnlySpan<float> indicatorLevels)
    {
        id.text = "" + indicatorLevels[0] + indicatorLevels[1] + indicatorLevels[2] + "-" + indicatorLevels[3] + indicatorLevels[4] + indicatorLevels[5] + "-" + indicatorLevels[6] + indicatorLevels[7] + indicatorLevels[8];
        grade.text = "" + CulculateLibrary.ContractGrade(indicatorLevels);
    }
}
