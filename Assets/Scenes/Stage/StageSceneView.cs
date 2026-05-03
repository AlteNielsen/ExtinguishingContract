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
    private VisualElement[] unitTiles;
    public StageBoardView(UIDocument document, VisualTreeAsset temp)
    {
        VisualElement board = document.rootVisualElement.Q<VisualElement>("Board");
        int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        board.contentContainer.Clear();
        gridPanels = new VisualElement[MapDataBase.Datas[selected].Data.layout.Length];
        unitTiles = new VisualElement[MapDataBase.Datas[selected].Data.layout.Length];
        for (int i = 0; i < MapDataBase.Datas[selected].Data.layout.Length; i++)
        {
            VisualElement ve = temp.Instantiate();
            gridPanels[i] = ve.Q<VisualElement>("GridPanel");
            unitTiles[i] = ve.Q<VisualElement>("UnitTile");
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
        DisplaySituation(fire, water);
        DisplayUnit(unit, facing);
    }

    private void DisplaySituation(Span<bool> fire, Span<bool> water)
    {
        for (int i = 0; i < fire.Length; i++)
        {
            gridPanels[i].RemoveFromClassList("bg-darkgray");
            gridPanels[i].RemoveFromClassList("water-not-selected");
            gridPanels[i].RemoveFromClassList("bg-red");
            if (fire[i])
            {
                gridPanels[i].AddToClassList("bg-red");
            }
            else if (water[i])
            {
                gridPanels[i].AddToClassList("water-not-selected");
            }
            else
            {
                gridPanels[i].AddToClassList("bg-darkgray");
            }
        }
    }

    private void DisplayUnit(Span<int> data, Span<UnitFacing> facing)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > -1)
            {
                unitTiles[i].RemoveFromClassList("transparent");
                switch (facing[data[i]])
                {
                    case UnitFacing.North:
                        unitTiles[i].style.rotate = new Rotate(Angle.Degrees(0));
                        break;
                    case UnitFacing.East:
                        unitTiles[i].style.rotate = new Rotate(Angle.Degrees(90));
                        break;
                    case UnitFacing.South:
                        unitTiles[i].style.rotate = new Rotate(Angle.Degrees(180));
                        break;
                    case UnitFacing.West:
                        unitTiles[i].style.rotate = new Rotate(Angle.Degrees(270));
                        break;
                }
            }
            else
            {
                unitTiles[i].AddToClassList("transparent");
            }
        }
    }

    public void UnitSelect(int unitPos, Span<bool> unitWater)
    {
        if(unitPos >= 0)
        {
            unitTiles[unitPos].RemoveFromClassList("bg-gray");
            unitTiles[unitPos].AddToClassList("bg-white");
            for(int i = 0; i < unitWater.Length; i++)
            {
                if(unitWater[i])
                {
                    gridPanels[i].RemoveFromClassList("water-not-selected");
                    gridPanels[i].AddToClassList("water-selected");
                }
            }
        }
    }
}
