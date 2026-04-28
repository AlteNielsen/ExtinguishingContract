using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class EndingSceneView
{
    private UIDocument mainDocument;
    private UIDocument goodDocument;
    private UIDocument badDocument;

    public EndingSceneView(UIDocument doc, UIDocument good, UIDocument bad)
    {
        mainDocument = doc;
        goodDocument = good;
        badDocument = bad;
        WriteTexts();
        DisplayDatas();
        DisplayEndingScreen();
    }

    private void WriteTexts()
    {
        WriteGoodTexts();
        WriteBadTexts();
        WriteMainTexts();
    }

    private void WriteGoodTexts()
    {
        List<Label> labels = goodDocument.rootVisualElement.Query<Label>("Text").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.EndingScreen)[i];
        }
    }

    private void WriteBadTexts()
    {
        List<Label> labels = badDocument.rootVisualElement.Query<Label>("Text").ToList();
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.EndingScreen)[3 + i];
        }
    }

    private void WriteMainTexts()
    {
        List<Label> labels = mainDocument.rootVisualElement.Query<Label>("Text").ToList();
        for(int i = 0; i < labels.Count;i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.EndingView)[i];
        }
    }

    private void DisplayEndingScreen()
    {
        ReadOnlySpan<float> blockSituation = SaveDataManager.Instance.Access<BurningSituationChunk>((int)SaveDataManager.SaveDataChunk.BurningSituation).data.Span;
        bool isGoodEnding = true;
        for(int i = 0; i < blockSituation.Length; i++)
        {
            if(blockSituation[i] > 0.5f)
            {
                isGoodEnding = false;
                break;
            }
        }

        if(isGoodEnding)
        {
            goodDocument.sortingOrder = 10;
            badDocument.sortingOrder = -10;
        }
        else
        {
            goodDocument.sortingOrder = -10;
            badDocument.sortingOrder = 10;
        }
    }

    public void SwitchMainScreen()
    {
        goodDocument.sortingOrder = -10;
        badDocument.sortingOrder = -11;
        mainDocument.sortingOrder = 10;
    }

    private void DisplayDatas()
    {
        DisplayIndicatorValues();
    }

    private void DisplayIndicatorValues()
    {
        float[] values = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        List<Label> labels = mainDocument.rootVisualElement.Query<Label>("IndicatorLabel").ToList();
        for(int i = 0; i < labels.Count;i++)
        {
            if (values[i] > 0)
            {
                labels[i].text = "+" + values[i];
            }
            else
            {
                labels[i].text = "" + values[i];
            }
        }
    }
}
