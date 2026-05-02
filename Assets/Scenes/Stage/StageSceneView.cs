using System;
using UnityEngine.UIElements;
using UnityEngine;

public class StageSceneView
{
    private UIDocument document;

    public StageSceneView(UIDocument doc)
    {
        document = doc;
    }
}

public class StageBoardView
{
    private VisualElement[] gridPanels;

    public StageBoardView(UIDocument document, VisualTreeAsset temp)
    {
        VisualElement board = document.rootVisualElement.Q<VisualElement>("Board");
        int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        board.contentContainer.Clear();
        gridPanels = new VisualElement[MapDataBase.Datas[selected].Data.layout.Length];
        for (int i = 0; i < MapDataBase.Datas[selected].Data.layout.Length; i++)
        {
            VisualElement ve = temp.Instantiate();
            gridPanels[i] = ve.Q<VisualElement>("GridPanel");
            board.contentContainer.Add(ve);
        }
        MapSetup(selected);
    }

    private void MapSetup(int mapNum)
    {
        MapLayout layout = MapDataBase.Datas[mapNum].Data;
        for (int i = 0; i < layout.layout.Length; i++)
        {
            if (layout.layout[i] == 1)
            {
                gridPanels[i].AddToClassList("non-display");
            }
        }
    }

    public void DisplayBoard(Span<bool> fire, Span<bool> water, Span<int> unit, Span<UnitFacing> facing)
    {
        DisplayBurning(fire);
        DisplayWater(water);
        DisplayUnit(unit);
    }

    private void DisplayBurning(Span<bool> data)
    {
        for(int i = 0; i < data.Length; i++)
        {
            if (data[i])
            {
                gridPanels[i].RemoveFromClassList("bg-darkgray");
                gridPanels[i].RemoveFromClassList("bg-blue");
                gridPanels[i].RemoveFromClassList("bg-white");
                gridPanels[i].AddToClassList("bg-red");
            }
        }
    }

    private void DisplayWater(Span<bool> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i])
            {
                gridPanels[i].RemoveFromClassList("bg-darkgray");
                gridPanels[i].RemoveFromClassList("bg-red");
                gridPanels[i].RemoveFromClassList("bg-white");
                gridPanels[i].AddToClassList("bg-blue");
            }
        }
    }

    private void DisplayUnit(Span<int> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > -1)
            {
                gridPanels[i].RemoveFromClassList("bg-darkgray");
                gridPanels[i].RemoveFromClassList("bg-blue");
                gridPanels[i].RemoveFromClassList("bg-red");
                gridPanels[i].AddToClassList("bg-white");
            }
        }
    }
}
