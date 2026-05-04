using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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
    private UnitFacing[] unitFacingA;
    private UnitFacing[] unitFacingB;
    private bool[] fireMapB;
    private int[] unitMapB;

    private bool[] FireMap => bufferingSwitch ? fireMapA : fireMapB;
    private bool[] NextFireMap => bufferingSwitch ? fireMapB : fireMapA;
    private int[] UnitMap => bufferingSwitch ? unitMapA : unitMapB;
    private int[] NextUnitMap => bufferingSwitch ? unitMapB : unitMapA;
    private UnitFacing[] NowUnitFacing => bufferingSwitch ? unitFacingA : unitFacingB;
    private UnitFacing[] NextUnitFacing => bufferingSwitch ? unitFacingB : unitFacingA;


    private bool bufferingSwitch = true;

    private int selectedUnitID = -1;
    private int selectedUnitPos = -1;

    private InputAction clockwiseRotate;
    private InputAction counterClockwiseRotate;
    private InputAction rightClick;

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
        calcManager.WaterCalculate(water, UnitMap, NowUnitFacing);
        boardView.DisplayBoard(FireMap, water, UnitMap, NowUnitFacing);
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
        clockwiseRotate = new InputAction(binding: "<Keyboard>/r");
        clockwiseRotate.performed += ctx => RotateUnit(width, true);
        clockwiseRotate.Enable();
        counterClockwiseRotate = new InputAction(binding: "<Keyboard>/e");
        counterClockwiseRotate.performed += ctx => RotateUnit(width, false);
        counterClockwiseRotate.Enable();
        rightClick = new InputAction(binding: "<Mouse>/rightButton");
        rightClick.performed += ctx => RightClicked(width);
        rightClick.Enable();
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

        unitFacingA = new UnitFacing[UnitDataBase.Datas.Length];
        unitFacingB = new UnitFacing[UnitDataBase.Datas.Length];
        Span<int> facingMemo = stackalloc int[UnitDataBase.Datas.Length];
        CulculateLibrary.SwitchFloatToInt(facingMemo, SaveDataManager.Instance.Access<UnitFacingChunk>((int)SaveDataManager.SaveDataChunk.UnitFacing).data.Span);
        MemoryMarshal.Cast<int, UnitFacing>(facingMemo).CopyTo(unitFacingA);
        Array.Copy(unitFacingA, unitFacingB, unitFacingA.Length);
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
                NowUnitFacing[i] = UnitFacing.East;
                NextUnitFacing[i] = UnitFacing.East;
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
                TurnProcess(UnitMap, NowUnitFacing);
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
                SelectUnit(index, width);
                return;
            }
        }
        else
        {
            Span<bool> water = stackalloc bool[UnitMap.Length];
            water.Clear();
            calcManager.UnitWaterCalc(water, selectedUnitPos, width, selectedUnitID, NowUnitFacing[selectedUnitID]);
            if (selectedUnitPos == index)
            {
                CancelUnitSelect(water);
                return;
            }

            if (water[index] && UnitMap[index] < 0)
            {
                UnitMove(index, water);
            }
        }
    }

    private void CancelUnitSelect(Span<bool> water)
    {
        boardView.UnitSelectCancel(selectedUnitPos, water);
        UnitSelectReset();
        sceneView.LightUpSelectUnitRange(-1);
        sceneView.LightUpSelectUnitIcon(-1);
    }

    private void SelectUnit(int index, int width)
    {
        selectedUnitID = UnitMap[index];
        selectedUnitPos = index;
        Span<bool> water = stackalloc bool[UnitMap.Length];
        water.Clear();
        calcManager.UnitWaterCalc(water, index, width, selectedUnitID, NowUnitFacing[selectedUnitID]);
        boardView.UnitSelect(index, water);
        sceneView.LightUpSelectUnitRange(UnitMap[index]);
        sceneView.LightUpSelectUnitIcon(UnitMap[index]);
    }

    private void UnitMove(int index, Span<bool> water)
    {
        Span<int> unit = stackalloc int[UnitMap.Length];
        UnitMap.CopyTo(unit);
        unit[selectedUnitPos] = -1;
        unit[index] = selectedUnitID;
        boardView.UnitSelectCancel(selectedUnitPos, water);
        UnitSelectReset();
        sceneView.LightUpSelectUnitRange(-1);
        sceneView.LightUpSelectUnitIcon(-1);
        TurnProcess(unit, NowUnitFacing);
    }

    private void RotateUnit(int width, bool isClockwise)
    {
        if (selectedUnitID < 0)
        {
            return;
        }
        Span<UnitFacing> unitFacing = stackalloc UnitFacing[NowUnitFacing.Length];
        NowUnitFacing.CopyTo(unitFacing);
        if(isClockwise)
        {
            unitFacing[selectedUnitID] = GetUnitFacingClockwise(NowUnitFacing[selectedUnitID]);
        }
        else
        {
            unitFacing[selectedUnitID] = GetUnitFacingCounterClockwise(NowUnitFacing[selectedUnitID]);
        }
        Span<bool> water = stackalloc bool[UnitMap.Length];
        water.Clear();
        calcManager.UnitWaterCalc(water, selectedUnitPos, width, selectedUnitID, NowUnitFacing[selectedUnitID]);
        boardView.UnitSelectCancel(selectedUnitPos, water);
        TurnProcess(UnitMap, unitFacing);
        if (IsSelectedUnitAlive())
        {
            water.Clear();
            calcManager.UnitWaterCalc(water, selectedUnitPos, width, selectedUnitID, NowUnitFacing[selectedUnitID]);
            boardView.UnitSelect(selectedUnitPos, water);
        }
        else
        {
            UnitSelectReset();
        }
    }

    private UnitFacing GetUnitFacingClockwise(UnitFacing facing)
    {
        switch (facing)
        {
            case UnitFacing.North:
                return UnitFacing.East;
            case UnitFacing.East:
                return UnitFacing.South;
            case UnitFacing.South:
                return UnitFacing.West;
            case UnitFacing.West:
                return UnitFacing.North;
        }
        return UnitFacing.North;
    }

    private UnitFacing GetUnitFacingCounterClockwise(UnitFacing facing)
    {
        switch (facing)
        {
            case UnitFacing.North:
                return UnitFacing.West;
            case UnitFacing.East:
                return UnitFacing.North;
            case UnitFacing.South:
                return UnitFacing.East;
            case UnitFacing.West:
                return UnitFacing.South;
        }
        return UnitFacing.North;
    }


    private void RightClicked(int width)
    {
        if(selectedUnitID >= 0)
        {
            TileOnClicked(selectedUnitPos, width);
        }
        else
        {
            UndoProcess();
        }
    }

    private void TurnProcess(Span<int> unit, Span<UnitFacing> facing)
    {
        Span<bool> water = stackalloc bool[FireMap.Length];
        Span<bool> nextFire = NextFireMap;
        FireMap.CopyTo(nextFire);
        Span<int> unitMap = NextUnitMap;
        unit.CopyTo(unitMap);
        Span<UnitFacing> unitFacing = NextUnitFacing;
        facing.CopyTo(unitFacing);
        calcManager.StageCalculate(nextFire, unitMap, water, unitFacing, spreadSpeed);
        SwitchBuffer();
        boardView.DisplayBoard(FireMap, water, UnitMap, NowUnitFacing);
    }

    private void UndoProcess()
    {
        SwitchBuffer();
        Span<bool> water = stackalloc bool[FireMap.Length];
        water.Clear();
        calcManager.WaterCalculate(water, UnitMap, NowUnitFacing);
        boardView.DisplayBoard(FireMap, water, UnitMap, NowUnitFacing);
        SynchronizeSituation();
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
            Array.Copy(unitFacingA, unitFacingB, unitFacingA.Length);
        }
        else
        {
            Array.Copy(fireMapB, fireMapA, fireMapA.Length);
            Array.Copy(unitMapB, unitMapA, unitMapA.Length);
            Array.Copy(unitFacingB, unitFacingA, unitFacingA.Length);
        }
    }

    private void UnitSelectReset()
    {
        selectedUnitID = -1;
        selectedUnitPos = -1;
    }

    private bool IsSelectedUnitAlive()
    {
        return UnitMap[selectedUnitPos] == selectedUnitID;
    }
}