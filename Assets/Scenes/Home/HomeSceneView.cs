using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class HomeSceneView
{
    private UIDocument document;

    public HomeSceneView(UIDocument doc)
    {
        document = doc;
        WriteText();
        SetupValues();
    }

    private void WriteText()
    {
        WriteBlockSelector();
        WriteSceneText();
    }

    private void WriteSceneText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Home)[i];
        }
    }

    private void WriteBlockSelector()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("BlockSelectorText").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[i];
        }
    }

    private void SetupValues()
    {
        SetupBlockSelectorDisplay();
        SetupContractMemoDisplay();
        SetupIndicatorsDisplay();
    }

    private void SetupContractMemoDisplay()
    {
        Label id = document.rootVisualElement.Q<Label>("ID");
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        id.text = "" + now.Span[0] + now.Span[1] + now.Span[2] + "-" + now.Span[3] + now.Span[4] + now.Span[5] + "-" + now.Span[6] + now.Span[7] + now.Span[8];

        Label grade = document.rootVisualElement.Q<Label>("Grade");
        grade.text = "" + CulculateLibrary.ContractGrade(now.Span);
    }

    private void SetupBlockSelectorDisplay()
    {
        List<VisualElement> ves = document.rootVisualElement.Query<VisualElement>("BlockSelectorBG").ToList();
        for(int i = 0; i < ves.Count; i++)
        {
            if(SaveDataManager.Instance.Access<BurningSituationChunk>(((int)SaveDataManager.SaveDataChunk.BurningSituation)).data.Span[i] > 0.5f)
            {
                ves[i].RemoveFromClassList("bg-darkgray");
                ves[i].AddToClassList("bg-red");
                ves[i].AddToClassList("semi-transparent");
            }
        }
    }

    private void SetupIndicatorsDisplay()
    {
        List<Label> chances = document.rootVisualElement.Query<Label>("IndicatorChance").ToList();
        List<Label> names = document.rootVisualElement.Query<Label>("IndicatorName").ToList();
        List<Label> types = document.rootVisualElement.Query<Label>("IndicatorType").ToList();
        List<Label> values = document.rootVisualElement.Query<Label>("IndicatorValue").ToList();
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<BlockIndicatorChunk>(((int)SaveDataManager.SaveDataChunk.BlockIndicator)).data.Span;
        float[] baseValue = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        int buttonCounter = 0;
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] > 0.5f)
            {
                int type = i / Config.Data.IndicatorMaxLv;
                int level = i % Config.Data.IndicatorMaxLv + 1;
                
                chances[buttonCounter].text = "+" + (Config.Data.IndicatorBaseChance * level) * 100 + "%";
                names[buttonCounter].text = WordDataBase.Word(WordDataBase.WordSelector.EIndicatorTitle)[type];
                types[buttonCounter].text = WordDataBase.Word(WordDataBase.WordSelector.EIndicatorEffect)[type];
                if (baseValue[type] > 0)
                {
                    values[buttonCounter].text = "+" + (baseValue[type] * level);
                }
                else
                {
                    values[buttonCounter].text = "" + (baseValue[type] * level);
                }
                buttonCounter++;
            }
        }
    }
}
