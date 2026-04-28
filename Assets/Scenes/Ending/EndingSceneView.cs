using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
        DisplayEnvValues();
        DisplayContractInfo();
        DisplayOverallStats();
        DisplayIndicatorStats();
        DisplayBurningSituation();
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

    private void DisplayEnvValues()
    {
        float[] values = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        List<Label> labels = mainDocument.rootVisualElement.Query<Label>("EnvLabel").ToList();
        labels[0].text = "" + (int)(values[6] * 100) + "%";
        labels[1].text = "" + values[7];
        labels[2].text = "" + values[8];
    }

    private void DisplayContractInfo()
    {
        Label id = mainDocument.rootVisualElement.Q<Label>("ID");
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        id.text = "" + now.Span[0] + now.Span[1] + now.Span[2] + "-" + now.Span[3] + now.Span[4] + now.Span[5] + "-" + now.Span[6] + now.Span[7] + now.Span[8];

        Label grade = mainDocument.rootVisualElement.Q<Label>("Grade");
        grade.text = "" + CulculateLibrary.ContractGrade(now.Span);
    }

    private void DisplayOverallStats()
    {
        ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<ResultStatsChunk>((int)SaveDataManager.SaveDataChunk.ResultStats).data.Span;
        List<Label> labels = mainDocument.rootVisualElement.Query<Label>("OverallStats").ToList();
        labels[0].text = "" + (int)datas[0];
        labels[1].text = "" + (int)datas[1];
        labels[2].text = CulculateLibrary.FloatToPercent(datas[2]) + "%";
        labels[3].text = "" + (int)datas[3];
        labels[4].text = CulculateLibrary.FloatToPercent(datas[3] / datas[1]) + "%";
        float zscore = CulculateLibrary.CulculateZScore((int)datas[1], (int)datas[3], datas[2]);
        labels[5].text = "" + (int)(zscore * 100) / 100f;
        labels[6].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.EndingView)[ZScoreConclude(zscore)];
        labels[7].text = CulculateLibrary.FloatToPercent(datas[4]) + "%";
    }

    private void DisplayIndicatorStats()
    {
        List<Label> labels = mainDocument.rootVisualElement.Query<Label>("IndicatorStats").ToList();
        ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<ResultStatsChunk>((int)SaveDataManager.SaveDataChunk.ResultStats).data.Span;
        int offset = 5;
        for(int i = 0;  i < labels.Count; i++)
        {
            labels[i].text = "" + (int)datas[offset + i];
        }
    }

    private void DisplayBurningSituation()
    {
        List<VisualElement> bars = mainDocument.rootVisualElement.Query<VisualElement>("GraphBar").ToList();
        int dataPerBar = DataPerGraphBar(bars.Count);
        Span<float> datas = stackalloc float[bars.Count];
        CalculateDisplayValue(dataPerBar, datas);
        for(int i = 0;i < bars.Count;i++)
        {
            bars[i].style.height = Length.Percent(datas[i] * 100);
        }
    }

    private int DataPerGraphBar(int barNum)
    {
        ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<TurnStatsChunk>((int)SaveDataManager.SaveDataChunk.TurnStats).data.Span;
        int counter = 0;
        for(int i = 0; i < datas.Length; i++)
        {
            if (datas[i] < 0.5f)
            {
                counter = i;
                break;
            }
        }
        return (counter / barNum) + 1;
    }

    private void CalculateDisplayValue(int dataPerBar, Span<float> result)
    {
        ReadOnlySpan<float> datas = SaveDataManager.Instance.Access<TurnStatsChunk>((int)SaveDataManager.SaveDataChunk.TurnStats).data.Span;
        for (int i = 0; i < result.Length; i++)
        {
            for(int j = 0; j < dataPerBar; j++)
            {
                result[i] += datas[i * dataPerBar + j];
            }
            result[i] /= dataPerBar;
            result[i] /= MapDataBase.Datas.Length;
        }
    }

    private int ZScoreConclude(float zscore)
    {
        float greatStandard = 1.65f;
        float goodStandard = 1.0f;
        float standard = 0.5f;
        int offset = 32;

        if(zscore > greatStandard)
        {
            return offset + 6;
        }
        else if(zscore > goodStandard)
        {
            return offset + 5;
        }
        else if(zscore > standard)
        {
            return offset + 4;
        }
        else if(zscore > -standard)
        {
             return offset + 3;
        }
        else if(zscore > -goodStandard)
        {
            return offset + 2;
        }
        else if(zscore > -greatStandard)
        {
            return offset + 1;
        }
        else
        {
            return offset;
        }
    }
}
