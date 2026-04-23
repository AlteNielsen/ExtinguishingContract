using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitSceneView
{
    private UIDocument document;
    private List<VisualElement> unitRangeDisplays;
    private TileType[][][] unitRanges;//unitIndex, Level, slotNum
    private VisualElement[][] unitRangeTiles;
    private List<VisualElement> levelSelectorBGs;
    private List<Label> levelSelectorLabels;

    public UnitSceneView(UIDocument doc)
    {
        document = doc;
        unitRangeDisplays = document.rootVisualElement.Query<VisualElement>("UnitRange").ToList();
        levelSelectorBGs = document.rootVisualElement.Query<VisualElement>("LevelSelectorBG").ToList();
        levelSelectorLabels = document.rootVisualElement.Query<Label>("LevelSelectorLabel").ToList();
        UnitRangeDataSetup();
        UnitRangeTileSetup();
        WriteText();
    }

    private void WriteText()
    {
        WriteSceneText();
        WriteUnitCard();
    }

    private void WriteSceneText()
    {
        List<Label> labels = document.rootVisualElement.Query<Label>("Text").ToList();
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Unit)[i];
        }
    }

    private void WriteUnitCard()
    {
        List<Label> category = document.rootVisualElement.Query<Label>("UnitCategory").ToList();
        for(int i = 0; i < category.Count; i++)
        {
            category[i].text = WordDataBase.Word(WordDataBase.WordSelector.UnitCategory)[i];
        }
        List<Label> names = document.rootVisualElement.Query<Label>("UnitName").ToList();
        for (int i = 0; i < names.Count; i++)
        {
            names[i].text = WordDataBase.Word(WordDataBase.WordSelector.UnitName)[i];
        }
        List<Label> texts = document.rootVisualElement.Query<Label>("UnitText").ToList();
        for (int i = 0; i < names.Count; i++)
        {
            texts[i].text = TextDataBase.GetTexts(TextDataBase.TextDictionary.Unit)[5];
        }
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            UnitRangeDisplay(i, 0);
        }
    }

    public void UnitLevelChange(int index, int level)
    {
        int maxLv = levelSelectorBGs.Count / UnitDataBase.Datas.Length;
        for(int i = 0; i < maxLv; i++)
        {
            levelSelectorBGs[maxLv * index + i].RemoveFromClassList("bg-white");
            levelSelectorBGs[maxLv * index + i].AddToClassList("bg-darkgray");
            levelSelectorLabels[maxLv * index + i].RemoveFromClassList("color-black");
            levelSelectorLabels[maxLv * index + i].AddToClassList("color-white");
        }
        levelSelectorBGs[maxLv * index + level].RemoveFromClassList("bg-darkgray");
        levelSelectorBGs[maxLv * index + level].AddToClassList("bg-white");
        levelSelectorLabels[maxLv * index + level].RemoveFromClassList("color-white");
        levelSelectorLabels[maxLv * index + level].AddToClassList("color-black");
        UnitRangeDisplay(index, level);
    }

    private void UnitRangeDisplay(int index, int level)
    {
        for(int i = 0; i < unitRangeTiles[index].Length; i++)
        {
            switch (unitRanges[index][level][i])
            {
                case TileType.None:
                    UnitRangeDisplayReset(index, i);
                    break;
                case TileType.Off:
                    UnitRangeDisplayReset(index, i);
                    unitRangeTiles[index][i].AddToClassList("grid-tile-off");
                    break;
                case TileType.On:
                    UnitRangeDisplayReset(index, i);
                    unitRangeTiles[index][i].AddToClassList("grid-tile-on");
                    break;
                case TileType.Unit:
                    UnitRangeDisplayReset(index, i);
                    unitRangeTiles[index][i].AddToClassList("grid-tile-unit");
                    break;
            }
        }
    }

    private void UnitRangeDisplayReset(int index, int num)
    {
        unitRangeTiles[index][num].RemoveFromClassList("grid-tile-off");
        unitRangeTiles[index][num].RemoveFromClassList("grid-tile-on");
        unitRangeTiles[index][num].RemoveFromClassList("grid-tile-unit");
    }

    private void UnitRangeTileSetup()
    {
        unitRangeTiles = new VisualElement[UnitDataBase.Datas.Length][];
        for (int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            int size = GetUnitRangeDisplaySize(i);
            unitRangeTiles[i] = new VisualElement[size * size];
        }

        for(int i = 0; i < unitRangeTiles.Length; i++)
        {
            unitRangeDisplays[i].contentContainer.Clear();
            float displaySize = 425;
            int size = GetUnitRangeDisplaySize(i);
            float tileSize = (displaySize / size) - 1;
            for (int j = 0; j < unitRangeTiles[i].Length; j++)
            {
                unitRangeTiles[i][j] = new VisualElement();
                unitRangeTiles[i][j].AddToClassList("grid-tile");
                unitRangeTiles[i][j].style.width = tileSize - 10;
                unitRangeTiles[i][j].style.height = tileSize - 10;
                unitRangeDisplays[i].Add(unitRangeTiles[i][j]);
            }
        }
    }

    private void UnitRangeDataSetup()
    {
        unitRanges = new TileType[UnitDataBase.Datas.Length][][];
        for (int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            unitRanges[i] = new TileType[UnitDataBase.Datas[i].RangeData.Length][];
        }

        for (int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            for(int j = 0; j < UnitDataBase.Datas[i].RangeData.Length; j++)
            {
                int size = GetUnitRangeDisplaySize(i);
                unitRanges[i][j] = new TileType[size * size];
            }
        }

        for (int i = 0; i < unitRanges.Length; i++)
        {
            var (centerX, centerY) = GetCenterPos(i);
            int size = GetUnitRangeDisplaySize(i);
            for (int j = 0; j < unitRanges[i].Length; j++)
            {
                for(int k = 0; k < unitRanges[i][j].Length; k++)
                {
                    int x = k % size;
                    int y = k / size;
                    unitRanges[i][j][k] = DecideTileType(i, j, x, y, centerX, centerY);
                }
            }
        }
    }

    private int GetUnitRangeDisplaySize(int index)
    {
        var (maxX, maxY, minX, minY) = GetUnitRangeInfo(index);
        int wid = maxX + Mathf.Abs(minX) + 1;
        int hei = maxY + Mathf.Abs(minY) + 1;
        if(wid > hei)
        {
            return wid;
        }
        else
        {
            return hei;
        }
    }

    private TileType DecideTileType(int index, int level, int x, int y, int centerX, int centerY)
    {
        if (centerX == x && centerY == y)
        {
            return TileType.Unit;
        }

        for (int i = 0; i <= level; i++)
        {
            RangeData data = UnitDataBase.Datas[index].RangeData[i];

            for (int j = 0; j < data.range.Length; j++)
            {
                int rangeX = centerX + data.range[j].relativeX;
                int rangeY = centerY + data.range[j].relativeY;
                if (x == rangeX && y == rangeY)
                {
                    return TileType.On;
                }
            }
        }

        for(int i = level + 1; i < UnitDataBase.Datas[index].RangeData.Length; i++)
        {
            RangeData data = UnitDataBase.Datas[index].RangeData[i];

            for (int j = 0; j < data.range.Length; j++)
            {
                int rangeX = centerX + data.range[j].relativeX;
                int rangeY = centerY + data.range[j].relativeY;
                if (x == rangeX && y == rangeY)
                {
                    return TileType.Off;
                }
            }
        }

        return TileType.None;
    }

    private (int centerX, int cetnerY) GetCenterPos(int index)
    {
        var (maxX, maxY, minX, minY) = GetUnitRangeInfo(index);
        int wid = maxX + Mathf.Abs(minX) + 1;
        int hei = maxY + Mathf.Abs(minY) + 1;
        int offsetX = 0;
        int offsetY = 0;
        if(wid - hei > 0)
        {
            offsetY = (wid - hei) / 2;
        }
        if(hei - wid > 0)
        {
            offsetX = (hei - wid) / 2;
        }
        return (Mathf.Abs(minX) + offsetX, Mathf.Abs(minY) + offsetY);
    }

    private (int frameMaxX, int frameMaxY, int frameMinX, int frameMinY) GetUnitRangeInfo(int index)
    {
        UnitData target = UnitDataBase.Datas[index];
        int maxX = 0;
        int minX = 0;
        int maxY = 0;
        int minY = 0;
        for (int i = 0; i < target.RangeData.Length; i++)
        {
            for (int j = 0; j < target.RangeData[i].range.Length; j++)
            {
                int x = target.RangeData[i].range[j].relativeX;
                if (maxX < x)
                {
                    maxX = x;
                }
                if (x < minX)
                {
                    minX = x;
                }

                int y = target.RangeData[i].range[j].relativeY;
                if (maxY < y)
                {
                    maxY = y;
                }
                if (y < minY)
                {
                    minY = y;
                }
            }
        }
        return (maxX, maxY, minX, minY);
    }

    private enum TileType
    { 
        None,
        Off,
        On,
        Unit
    }
}
