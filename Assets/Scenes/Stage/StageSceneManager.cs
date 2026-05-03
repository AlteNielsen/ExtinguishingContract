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
    private CalculatorManager calcManager;
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
        var (speed, start, index, width, height) = CulculateLibrary.GetParameter();
        spreadSpeed = speed;
        Span<bool> layout = stackalloc bool[width * height];
        CulculateLibrary.GetMapLayout(index, layout);
        DataRestore(index);
        calcManager = new CalculatorManager(layout, width, height);
        StartProcess(index ,start, width);
        Span<bool> water = stackalloc bool[FireMap.Length];
        calcManager.WaterCalculate(water, UnitMap, unitFacing);
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
                TurnProcess(UnitMap);
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
                calcManager.UnitWaterCalc(water, index, width, selectedUnitID, unitFacing[selectedUnitID]);
                boardView.UnitSelect(index, water);
                return;
            }
        }
        else
        {
            Span<bool> water = stackalloc bool[UnitMap.Length];
            water.Clear();
            calcManager.UnitWaterCalc(water, selectedUnitPos, width, selectedUnitID, unitFacing[selectedUnitID]);
            if (selectedUnitPos == index)
            {
                boardView.UnitSelectCancel(selectedUnitPos, water);
                selectedUnitID = -1;
                selectedUnitPos = -1;
                return;
            }

            if (water[index] && UnitMap[index] < 0)
            {
                Span<int> unit = stackalloc int[UnitMap.Length];
                UnitMap.CopyTo(unit);
                unit[selectedUnitPos] = -1;
                unit[index] = selectedUnitID;
                boardView.UnitSelectCancel(selectedUnitPos, water);
                selectedUnitID = -1;
                selectedUnitPos = -1;
                TurnProcess(unit);
            }
        }
    }

    private void TurnProcess(Span<int> unit)
    {
        Span<bool> water = stackalloc bool[FireMap.Length];
        Span<bool> nextFire = NextFireMap;
        FireMap.CopyTo(nextFire);
        Span<int> unitMap = NextUnitMap;
        unit.CopyTo(unitMap);
        calcManager.StageCalculate(nextFire, unitMap, water, unitFacing, spreadSpeed);
        SwitchBuffer();
        boardView.DisplayBoard(FireMap, water, UnitMap, unitFacing);
    }

    private void UndoProcess()
    {
        SwitchBuffer();
        Span<bool> water = stackalloc bool[FireMap.Length];
        water.Clear();
        calcManager.WaterCalculate(water, UnitMap, unitFacing);
        boardView.DisplayBoard(FireMap, water, UnitMap, unitFacing);
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