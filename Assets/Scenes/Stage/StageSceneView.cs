using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;

public class StageSceneView
{
    private UIDocument document;
    private List<VisualElement> rangeDisplays;
    private int[] unitIndices;

    public StageSceneView(UIDocument doc)
    {
        document = doc;
        DisplayUnitRanges();
    }

    public void LightUpSelectUnitRange(int index)
    {
        for (int i = 0; i < rangeDisplays.Count; i++)
        {
            rangeDisplays[i].AddToClassList("display-tile-range-not-selected");
        }

        if(index >= 0)
        {
            rangeDisplays[unitIndices[index]].RemoveFromClassList("display-tile-range-not-selected");
        }
    }

    private void DisplayUnitRanges()
    {
        rangeDisplays = document.rootVisualElement.Query<VisualElement>("UnitRangeDisplay").ToList();
        ReadOnlySpan<float> unit = SaveDataManager.Instance.Access<UnitSelectChunk>((int)SaveDataManager.SaveDataChunk.UnitSelect).data.Span;
        ReadOnlySpan<float> levels = SaveDataManager.Instance.Access<UnitLevelChunk>((int)SaveDataManager.SaveDataChunk.UnitLevel).data.Span;
        for(int i = 0; i < rangeDisplays.Count; i++)
        {
            rangeDisplays[i].AddToClassList("display-tile-range-not-selected");
        }

        unitIndices = new int[unit.Length];
        int counter = 0;
        for (int i = 0; i < unit.Length; i++)
        {
            if (unit[i] < 0.5f) continue;

            unitIndices[i] = counter;
            int size = CulculateLibrary.GetUnitRangeDisplaySize(i, (int)levels[i] + 1);
            var(centerX, centerY) = CulculateLibrary.GetUnitRangeCenterPos(i, (int)levels[i] + 1);
            for (int j = 0; j < size * size; j++)
            {
                VisualElement ve = new();
                ve.style.width = Length.Percent(100f / size);
                ve.style.height = Length.Percent(100f / size);
                if(IsTheTileUnitRange(i, (int)levels[i] + 1, j, centerX, centerY, size))
                {
                    ve.AddToClassList("display-tile-range");
                }
                if(j == centerY * size + centerX)
                {
                    ve.AddToClassList("display-tile-unit");
                }
                rangeDisplays[counter].contentContainer.Add(ve);
            }
            counter++;
        }
    }

    private bool IsTheTileUnitRange(int index, int level, int target, int centerX, int centerY, int size)
    {
        Span<RangeData> data = UnitDataBase.Datas[index].RangeData;
        for(int i = 0; i < level; i++)
        {
            for(int j = 0; j < data[i].range.Length; j++)
            {
                int x = data[i].range[j].relativeX + centerX;
                int y = data[i].range[j].relativeY + centerY;
                if(y * size + x == target)
                {
                    return true;
                }
            }
        }
        return false;
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
        unitTiles[unitPos].RemoveFromClassList("bg-gray");
        unitTiles[unitPos].AddToClassList("bg-white");
        for (int i = 0; i < unitWater.Length; i++)
        {
            if (unitWater[i])
            {
                gridPanels[i].RemoveFromClassList("water-not-selected");
                gridPanels[i].AddToClassList("water-selected");
            }
        }
    }

    public void UnitSelectCancel(int unitPos, Span<bool> unitWater)
    {
        unitTiles[unitPos].RemoveFromClassList("bg-white");
        unitTiles[unitPos].AddToClassList("bg-gray");
        for (int i = 0; i < unitWater.Length; i++)
        {
            if (unitWater[i])
            {
                gridPanels[i].RemoveFromClassList("water-selected");
                gridPanels[i].AddToClassList("water-not-selected");
            }
        }
    }
}
