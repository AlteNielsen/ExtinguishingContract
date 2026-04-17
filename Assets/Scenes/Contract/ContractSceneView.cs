using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ContractSceneView
{
    private UIDocument document;
    private List<VisualElement> indicatorButtonBG;
    private List<Label> indicatorButtonText;

    public ContractSceneView(UIDocument doc) 
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        document = doc;
        indicatorButtonBG = document.rootVisualElement.Query<VisualElement>("IndicatorButtonBG").ToList();
        indicatorButtonText = document.rootVisualElement.Query<Label>("IndicatorButtonText").ToList();
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
}
