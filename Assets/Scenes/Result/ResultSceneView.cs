using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ResultSceneView
{
    private UIDocument document;

    public ResultSceneView(UIDocument doc)
    {
        document = doc;
        WriteSelectedUnit();
        WriteLabels();
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
        WriteBlockNames();
        WriteSceneTexts();
        WriteEnvPreview();
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
}
