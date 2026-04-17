using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class TitleSceneView
{
    private UIDocument document;

    public TitleSceneView(UIDocument doc)
    {
        document = doc;
        WriteText();
        WriteValue();
    }

    public void WriteText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Title)[i];
        }
        Label continueText = document.rootVisualElement.Q<Label>(className: "continue");
        VisualElement continueUnderbar = document.rootVisualElement.Q<VisualElement>("ContinueUnderbar");
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        if (now.Span[0] < 0)
        {
            continueText.RemoveFromClassList("button-text");
            continueUnderbar.RemoveFromClassList("button-underbar");
            continueText.AddToClassList("color-darkgray");
        }
    }

    public void WriteValue()
    {
        List<Label> values = document.rootVisualElement.Query<Label>("Value").ToList();
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        if (now.Span[0] < 0)
        {
            values[0].text = "-----------";
            values[1].text = "---";
        }
        else
        {
            values[0].text = "" + now.Span[0] + now.Span[1] + now.Span[2] + "-" + now.Span[3] + now.Span[4] + now.Span[5] + "-" + now.Span[6] + now.Span[7] + now.Span[8];
            values[1].text = "" + CulculateLibrary.ContractGrade(now.Span);
        }

        ReadOnlyMemory<float> max = SaveDataManager.Instance.Access<MaxIDChunk>((int)SaveDataManager.SaveDataChunk.MaxID).data;
        if (max.Span[0] < 0)
        {
            values[3].text = "-----------";
            values[2].text = "---";
        }
        else
        {
            values[3].text = "" + max.Span[0] + max.Span[1] + max.Span[2] + "-" + max.Span[3] + max.Span[4] + max.Span[5] + "-" + max.Span[6] + max.Span[7] + max.Span[8];
            values[2].text = "" + CulculateLibrary.ContractGrade(max.Span);
        }
    }

    public void SwitchScreen()
    {
        document.rootVisualElement.Q<VisualElement>("StartScreen").style.display = DisplayStyle.None;
        document.rootVisualElement.Q<VisualElement>("TitleScreen").style.display = DisplayStyle.Flex;
    }
}
