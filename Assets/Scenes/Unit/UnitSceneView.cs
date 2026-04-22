using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitSceneView
{
    private UIDocument document;

    public UnitSceneView(UIDocument doc)
    {
        document = doc;
        WriteText();
    }

    private void WriteText()
    {
        WriteSceneText();
        WriteUnitCard();
    }

    private void WriteSceneText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Home)[i];
        }
    }

    private void WriteUnitCard()
    {
        List<Label> category = document.rootVisualElement.Query<Label>("UnitCategory").ToList();
        for(int i = 0; i < category.Count; i++)
        {
            category[i].text = WordDataBase.Word(WordDataBase.WordSelector.UnitCategory)[i];
        }
        List<Label> names = document.rootVisualElement.Query<Label>("UnitName").ToList();
        for (int i = 0; i < names.Count; i++)
        {
            names[i].text = WordDataBase.Word(WordDataBase.WordSelector.UnitName)[i];
        }
        List<Label> texts = document.rootVisualElement.Query<Label>("UnitText").ToList();
        for (int i = 0; i < names.Count; i++)
        {
            texts[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Unit)[5];
        }
    }
}
