using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.UIElements;

public class ResultSceneView
{
    private UIDocument document;

    public ResultSceneView(UIDocument doc, bool isSuccess)
    {
        document = doc;
        WriteSelectedUnit();
        WriteLabels();
        Display(isSuccess);
    }

    private void WriteSelectedUnit()
    {
        WriteUnitName();
        WriteUnitLevel();
        DisplayUnit();
    }

    private void WriteUnitName()
    {
        List<Label> names = document.rootVisualElement.Query<Label>("UnitName").ToList();
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            names[i].text = WordDataBase.Word(WordDataBase.WordSelector.UnitName)[i];
        }
    }

    private void WriteUnitLevel()
    {
        List<Label> levels = document.rootVisualElement.Query<Label>("UnitLevel").ToList();
        int maxLv = levels.Count / UnitDataBase.Datas.Length;
        ReadOnlySpan<float> save = SaveDataManager.Instance.Access<UnitLevelChunk>((int)SaveDataManager.SaveDataChunk.UnitLevel).data.Span;
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            for(int j = 0; j < maxLv; j++)
            {
                if(j == (int)save[i])
                {
                    levels[i * maxLv + j].RemoveFromClassList("non-display");
                }
                else
                {
                    levels[i * maxLv + j].AddToClassList("non-display");
                }
            }
        }
    }

    private void DisplayUnit()
    {
        List<VisualElement> plates = document.rootVisualElement.Query<VisualElement>("UnitPlate").ToList();
        ReadOnlySpan<float> save = SaveDataManager.Instance.Access<UnitSelectChunk>((int)SaveDataManager.SaveDataChunk.UnitSelect).data.Span;
        for(int i = 0; i < plates.Count; i++)
        {
            if (save[i] > 0.5f)
            {
                plates[i].RemoveFromClassList("non-display");
            }
            else
            {
                plates[i].AddToClassList("non-display");
            }
        }
    }

    private void WriteLabels()
    {
        WriteContractMemo();
        WriteBlockNames();
        WriteSceneTexts();
        WriteEnvPreview();
    }

    private void WriteContractMemo()
    {
        Label id = document.rootVisualElement.Q<Label>("ID");
        ReadOnlyMemory<float> now = SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data;
        id.text = "" + now.Span[0] + now.Span[1] + now.Span[2] + "-" + now.Span[3] + now.Span[4] + now.Span[5] + "-" + now.Span[6] + now.Span[7] + now.Span[8];

        Label grade = document.rootVisualElement.Q<Label>("Grade");
        grade.text = "" + CulculateLibrary.ContractGrade(now.Span);
    }

    private void WriteBlockNames()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("BlockName").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[i];
        }
    }

    private void WriteSceneTexts()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Result)[i];
        }
    }

    private void WriteEnvPreview()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("EnvPreview").ToList();
        WriteDeployInfo(labels[0]);
        var (bs, increase) = CulculateLibrary.CulculatePressures();
        labels[1].text = "" + bs;
        labels[2].text = "+" + increase;
        WriteUsePressure(labels[3]);
        labels[4].text = "" + CulculateLibrary.CulculateFireSpreadSpeed();
        labels[5].text = "" + CulculateLibrary.CulculateStartTurn();
        int chacne = (int)(CulculateLibrary.CulculateChance() * 100);
        if(chacne > 100)
        {
            labels[6].text = "" + 100 + "%";
        }
        else
        {
            labels[6].text = "" + chacne + "%";
        }
    }

    private void WriteDeployInfo(Label target)
    {
        int deploy = 0;
        ReadOnlySpan<float> info = SaveDataManager.Instance.Access<UnitSelectChunk>((int)SaveDataManager.SaveDataChunk.UnitSelect).data.Span;
        for (int i = 0; i < info.Length; i++)
        {
            if (info[i] > 0.5f)
            {
                deploy++;
            }
        }
        target.text = "" + deploy;
    }

    private void WriteUsePressure(Label target)
    {
        int pressure = 0;
        ReadOnlySpan<float> unitInfo = SaveDataManager.Instance.Access<UnitSelectChunk>((int)SaveDataManager.SaveDataChunk.UnitSelect).data.Span;
        ReadOnlySpan<float> pressInfo = SaveDataManager.Instance.Access<UnitLevelChunk>((int)SaveDataManager.SaveDataChunk.UnitLevel).data.Span;
        var (bs, increase) = CulculateLibrary.CulculatePressures();
        for (int i = 0; i < unitInfo.Length; i++)
        {
            if (unitInfo[i] > 0.5f)
            {
                pressure += (int)(bs + (increase * pressInfo[i]));
            }
        }
        target.text = "" + pressure;
    }

    private void Display(bool isSuccess)
    {
        DisplayIsSuccess(isSuccess);
        DisplayProgress(isSuccess);
    }

    private void DisplaySelectedMap(bool isSuccess)
    {
        Label selected = document.rootVisualElement.Q<Label>("SelectedBlockName");
        ReadOnlySpan<float> map = SaveDataManager.Instance.Access<MapSelectChunk>(((int)SaveDataManager.SaveDataChunk.MapSelect)).data.Span;
        selected.text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[(int)map[0]];
        List<VisualElement> bgs = document.rootVisualElement.Query<VisualElement>("BlockDisplayBG").ToList();
        bgs[(int)map[0]].RemoveFromClassList("bg-darkgray");
        bgs[(int)map[0]].AddToClassList("semi-transparent");
        if (isSuccess)
        {
            bgs[(int)map[0]].AddToClassList("selected-map-success");
            bgs[(int)map[0]].AddToClassList("bg-blue");
        }
        else
        {
            bgs[(int)map[0]].AddToClassList("selected-map-continue");
            bgs[(int)map[0]].AddToClassList("bg-red");
        }
    }

    private void DisplayIsSuccess(bool isSuccess)
    {
        Label label = document.rootVisualElement.Q<Label>("IsSuccessText");
        if (isSuccess)
        {
            label.text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Result)[16];
            label.AddToClassList("color-waterblue");
        }
        else
        {
            label.text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Result)[17];
            label.AddToClassList("color-red");
        }
    }

    private void DisplayProgress(bool isSuccess)
    {
        Label increase = document.rootVisualElement.Q<Label>("OtherProgressIncrease");
        float baseChance = SaveDataManager.Instance.Access<OtherProgressChunk>((int)SaveDataManager.SaveDataChunk.OtherProgress).data.Span[0];
        float chance = CulculateLibrary.CulculateChance();
        if (isSuccess && chance > 1)
        {
            increase.text = "+" + CulculateLibrary.FloatToPercent(chance - 1) + "%";
            increase.AddToClassList("color-waterblue");
        }
        else
        {
            increase.text = "Å}" + 0 + "%";
            increase.AddToClassList("color-white");
        }
        Label valueLabel = document.rootVisualElement.Q<Label>("OtherProgressValue");
        float value = baseChance + (chance - 1);
        if (chance > 1)
        {
            while (value >= 1)
            {
                value -= 1;
            }
        }
        valueLabel.text = CulculateLibrary.FloatToPercent(value) + "%";
        VisualElement gaugePool = document.rootVisualElement.Q<VisualElement>("Gauge");
        gaugePool.style.height = Length.Percent(value * 100);
    }
}
