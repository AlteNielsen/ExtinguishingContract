using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ContractSceneView
{
    private UIDocument document;
    public ContractSceneView(UIDocument doc) 
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        document = doc;
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
}
