using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageSceneManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private VisualTreeAsset tile;
    private StageSceneView sceneView;
    private StageBoardView boardView;
    private WaterCalculator waterCalc;
    private FireCalculator fireCalc;
    private int spreadSpeed;

    private bool[] fireMapA;
    private int[] unitMapA;
    private UnitFacing[] unitFacing;
    private bool[] fireMapB;
    private int[] unitMapB;

    private bool[] FireMap => bufferingSwitch ? fireMapA : fireMapB;
    private bool[] NextFireMap => bufferingSwitch ? fireMapB : fireMapA;
    private int[] UnitMap => bufferingSwitch ? unitMapA : unitMapB;
    private int[] NextUnitMap => bufferingSwitch ? unitMapB : unitMapA;

    private bool bufferingSwitch = true;

    private int selectedUnitID = -1;
    private int selectedUnitPos = -1;

    void Awake()
    {
        ExtinguishingContract.DevelopOnlyGameSetup();
        boardView = new StageBoardView(document, tile);
        sceneView = new StageSceneView(document);
        var (speed, start, index, width, height) = GetParameter();
        spreadSpeed = speed;
        Span<bool> layout = stackalloc bool[width * height];
        GetMapLayout(index, layout);
        DataRestore(index);
        fireCalc = new FireCalculator(layout, width, height);
        waterCalc = new WaterCalculator(layout, width, height);
        StartProcess(index ,start, width);
        Span<bool> water = stackalloc bool[FireMap.Length];
        waterCalc.WaterCalculate(water, UnitMap, unitFacing);
        boardView.DisplayBoard(FireMap, water, UnitMap, unitFacing);
        StageSceneController(width);
    }

    private void StageSceneController(int width)
    {
        List<Button> tiles = document.rootVisualElement.Query<Button>("GridTileButton").ToList();
        for(int i = 0; i < tiles.Count; i++)
        {
            int index = i;
            int wid = width;
            tiles[i].clicked += () => TileOnClicked(index, wid); 
        }
    }

    private (int speed, int start, int mapIndex, int width, int height) GetParameter()
    {
        float[] data = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        int speed = 1;
        for(int i = 0; i < Config.Data.IndicatorMaxLv; i++)
        {
            if(indicators[Config.Data.IndicatorMaxLv * 4 + i] > 0.5f)
            {
                speed += (int)(data[4] * (i + 1));
            }
        }
        int start = 0;
        for (int i = 0; i < Config.Data.IndicatorMaxLv; i++)
        {
            if (indicators[Config.Data.IndicatorMaxLv * 5 + i] > 0.5f)
            {
                start += (int)(data[5] * (i + 1));
            }
        }
        int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];

        return (speed, start, selected, MapDataBase.Datas[selected].Data.width, MapDataBase.Datas[selected].Data.height);
    }

    private void GetMapLayout(int index, Span<bool> result)
    {
        Span<int> layout = MapDataBase.Datas[index].Data.layout;
        for(int i = 0; i < layout.Length; i++)
        {
            if (layout[i] == 1)
            {
                result[i] = true;
            }
            else
            {
                result[i] = false;
            }
        }
    }

    private void DataRestore(int index)
    {
        MapLayout datas = MapDataBase.Datas[index].Data;

        fireMapA = new bool[datas.layout.Length];
        fireMapB = new bool[datas.layout.Length];
        CulculateLibrary.SwitchFloatToBool(fireMapA, SaveDataManager.Instance.Access<FireMapChunk>((int)SaveDataManager.SaveDataChunk.FireMap).data.Span);
        Array.Copy(fireMapA, fireMapB, fireMapA.Length);

        unitMapA = new int[datas.layout.Length];
        unitMapB = new int[datas.layout.Length];
        CulculateLibrary.SwitchFloatToInt(unitMapA, SaveDataManager.Instance.Access<UnitMapChunk>((int)SaveDataManager.SaveDataChunk.UnitMap).data.Span);
        Array.Copy(unitMapA, unitMapB, unitMapA.Length);

        unitFacing = new UnitFacing[UnitDataBase.Datas.Length];
        Span<int> facingMemo = stackalloc int[UnitDataBase.Datas.Length];
        CulculateLibrary.SwitchFloatToInt(facingMemo, SaveDataManager.Instance.Access<UnitFacingChunk>((int)SaveDataManager.SaveDataChunk.UnitFacing).data.Span);
        MemoryMarshal.Cast<int, UnitFacing>(facingMemo).CopyTo(unitFacing);
    }

    private void StartProcess(int mapIndex, int count, int width)
    {
        bool isInitial = true;
        for(int i = 0; i < FireMap.Length; i++)
        {
            if (FireMap[i])
            {
                isInitial = false;
                break;
            }
        }

        if (isInitial)
        {
            ReadOnlySpan<float> unitData = SaveDataManager.Instance.Access<UnitSelectChunk>((int)SaveDataManager.SaveDataChunk.UnitSelect).data.Span;
            int counter = 0;
            for(int i = 0; i < unitData.Length; i++)
            {
                unitFacing[i] = UnitFacing.East;
                if(unitData[i] > 0.5f)
                {
                    UnitMap[counter * width] = i;
                    counter++;
                }
            }
            int firePoint = MapDataBase.Datas[mapIndex].Data.fire_point;
            fireMapA[firePoint] = true;
            fireMapB[firePoint] = true;
            for(int i = 0; i < count; i++)
            {
                TurnProcess();
            }
            SynchronizeSituation();
        }
    }

    private void TileOnClicked(int index, int width)
    {
        if(selectedUnitID < 0)
        {
            if (UnitMap[index] >= 0)
            {
                selectedUnitID = UnitMap[index];
                selectedUnitPos = index;
                Span<bool> water = stackalloc bool[UnitMap.Length];
                water.Clear();
                ReadOnlySpan<float> levels = SaveDataManager.Instance.Access<UnitLevelChunk>((int)SaveDataManager.SaveDataChunk.UnitLevel).data.Span;
                waterCalc.UnitWaterCalc(water, index % width, index / width, selectedUnitID, (int)levels[selectedUnitID], unitFacing[selectedUnitID]);
                boardView.UnitSelect(index, water);
            }
        }
        else
        {
            if(selectedUnitPos == index)
            {
                Span<bool> water = stackalloc bool[UnitMap.Length];
                water.Clear();
                ReadOnlySpan<float> levels = SaveDataManager.Instance.Access<UnitLevelChunk>((int)SaveDataManager.SaveDataChunk.UnitLevel).data.Span;
                waterCalc.UnitWaterCalc(water, selectedUnitPos % width, selectedUnitPos / width, selectedUnitID, (int)levels[selectedUnitID], unitFacing[selectedUnitID]);
                boardView.UnitSelectCancel(selectedUnitPos, water);
                selectedUnitID = -1;
                selectedUnitPos = -1;
            }
        }
    }

    private void TurnProcess()
    {
        Span<bool> water = stackalloc bool[FireMap.Length];
        Span<bool> nextFire = NextFireMap;
        FireMap.CopyTo(nextFire);
        Span<int> unitMap = NextUnitMap;
        UnitMap.CopyTo(unitMap);
        StageCalculate(nextFire, unitMap, water, unitFacing, spreadSpeed);
        SwitchBuffer();
        boardView.DisplayBoard(FireMap, water, UnitMap, unitFacing);
    }

    private void UndoProcess()
    {
        SwitchBuffer();
        Span<bool> water = stackalloc bool[FireMap.Length];
        water.Clear();
        waterCalc.WaterCalculate(water, UnitMap, unitFacing);
        boardView.DisplayBoard(FireMap, water, UnitMap, unitFacing);
    }

    private void StageCalculate(Span<bool> fireMapResult, Span<int> unitMapResult, Span<bool> waterResult, Span<UnitFacing> facing, int speed)
    {
        Span<bool> fireA = stackalloc bool[fireMapResult.Length];
        Span<bool> fireB = stackalloc bool[fireMapResult.Length];
        fireMapResult.CopyTo(fireA);
        fireB.Clear();
        Span<int> unit = stackalloc int[unitMapResult.Length];
        Span<bool> water = stackalloc bool[fireMapResult.Length];
        unitMapResult.CopyTo(unit);
        bool switcher = true;
        for(int i = 0; i < speed; i++)
        {
            water.Clear();
            if(switcher)
            {
                waterCalc.WaterCalculate(water, unit, facing);
                fireCalc.FireSpread(fireB, fireA, water);
                UnitRemove(fireB, unit);
            }
            else
            {
                waterCalc.WaterCalculate(water, unit, facing);
                fireCalc.FireSpread(fireA, fireB, water);
                UnitRemove(fireA, unit);
            }
            switcher = !switcher;
        }

        if(switcher)
        {
            fireA.CopyTo(fireMapResult);
        }
        else
        {
            fireB.CopyTo(fireMapResult);
        }
        unit.CopyTo(unitMapResult);

        waterCalc.WaterCalculate(water, unit, facing);
        water.CopyTo(waterResult);
    }

    private void UnitRemove(Span<bool> fire, Span<int> unitMapResult)
    {
        for(int i = 0; i < fire.Length; i++)
        {
            if (fire[i])
            {
                unitMapResult[i] = -1;
            }
        }
    }

    private void SwitchBuffer()
    {
        bufferingSwitch = !bufferingSwitch;
    }

    private void SynchronizeSituation()
    {
        if(bufferingSwitch)
        {
            Array.Copy(fireMapA, fireMapB, fireMapA.Length);
            Array.Copy(unitMapA, unitMapB, unitMapA.Length);
        }
        else
        {
            Array.Copy(fireMapB, fireMapA, fireMapA.Length);
            Array.Copy(unitMapB, unitMapA, unitMapA.Length);
        }
    }
}