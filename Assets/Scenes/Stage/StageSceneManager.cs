using System;
using System.Runtime.InteropServices;
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
    private UnitFacing[] unitFacingA;
    private bool[] fireMapB;
    private int[] unitMapB;
    private UnitFacing[] unitFacingB;

    private bool[] FireMap { get { if (bufferingSwitch) { return fireMapA; } else { return fireMapB; } } set { if (bufferingSwitch) { fireMapB = value; } else { fireMapA = value; } } }
    private int[] UnitMap { get { if (bufferingSwitch) { return unitMapA; } else { return unitMapB; } } set{ if (bufferingSwitch){ unitMapB = value; } else { unitMapA = value; } } }
    private UnitFacing[] UnitFacing { get { if (bufferingSwitch) { return unitFacingA; } else { return unitFacingB; } } set { if (bufferingSwitch) { unitFacingB = value; } else { unitFacingA = value; } } }

    private bool bufferingSwitch = false;

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
    }

    private (int speed, int start, int mapIndex, int width, int height) GetParameter()
    {
        float[] data = CulculateLibrary.IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        int speed = 0;
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
                speed += (int)(data[5] * (i + 1));
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

        unitFacingA = new UnitFacing[UnitDataBase.Datas.Length];
        unitFacingB = new UnitFacing[UnitDataBase.Datas.Length];
        Span<int> facingMemo = stackalloc int[UnitDataBase.Datas.Length];
        CulculateLibrary.SwitchFloatToInt(facingMemo, SaveDataManager.Instance.Access<UnitFacingChunk>((int)SaveDataManager.SaveDataChunk.UnitFacing).data.Span);
        MemoryMarshal.Cast<int, UnitFacing>(facingMemo).CopyTo(unitFacingA);
        Array.Copy(unitFacingA, unitFacingB, unitFacingA.Length);
    }

    private void TurnProcess()
    {
        Span<bool> water = stackalloc bool[FireMap.Length];
        StageCalculate(FireMap, UnitMap, water, UnitFacing, spreadSpeed);
        SwitchBuffer();
        boardView.DisplayBoard(FireMap, water, UnitMap, UnitFacing);
    }

    private void UndoProcess()
    {
        SwitchBuffer();
        Span<bool> water = stackalloc bool[FireMap.Length];
        water.Clear();
        waterCalc.WaterCalculate(water, UnitMap, UnitFacing);
        boardView.DisplayBoard(FireMap, water, UnitMap, UnitFacing);
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
            fireB.CopyTo(fireMapResult);
        }
        else
        {
            fireA.CopyTo(fireMapResult);
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
}