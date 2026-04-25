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
    }

    private void WriteBlockNames()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("BlockName").ToList();
        for(int i = 0; i < labels.Count; i++)
        {
            labels[i].text = WordDataBase.Word(WordDataBase.WordSelector.MapTitle)[i];
        }
    }
}
