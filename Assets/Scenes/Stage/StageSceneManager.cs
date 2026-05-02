using System;
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

    private int width;
    private int height;
    private bool[] mapLayout;

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
        var (speed, start) = GetParameter();
        spreadSpeed = speed;
        DataSetup();
    }

    private (int speed, int start) GetParameter()
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
        return (speed, start);
    }

    private void DataSetup()
    {
        /*int selected = (int)SaveDataManager.Instance.Access<MapSelectChunk>((int)SaveDataManager.SaveDataChunk.MapSelect).data.Span[0];
        MapLayout datas = MapDataBase.Datas[selected].Data;
        width = datas.width;
        height = datas.height;
        fireMap = new bool[datas.layout.Length];
        CulculateLibrary.SwitchFloatToBool(fireMap, SaveDataManager.Instance.Access<FireMapChunk>((int)SaveDataManager.SaveDataChunk.FireMap).data.Span);
        mapLayout = new bool[datas.layout.Length];
        for(int i = 0; i < mapLayout.Length; i++)
        {
            if (datas.layout[i] == 1)
            {
                mapLayout[i] = true;
            }
            else
            {
                mapLayout[i] = false;
            }
        }
        unitMap = new int[datas.layout.Length];
        CulculateLibrary.SwitchFloatToInt(unitMap, SaveDataManager.Instance.Access<UnitMapChunk>((int)SaveDataManager.SaveDataChunk.UnitMap).data.Span);
        *///unitFacing = new int[height];
        //CulculateLibrary.SwitchFloatToInt(unitFacing, SaveDataManager.Instance.Access<UnitFacingChunk>((int)SaveDataManager.SaveDataChunk.UnitFacing).data.Span);
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

public class FireCalculator
{
    private bool[] mapLayout;
    private int width;
    private int height;

    public FireCalculator(bool[] map, int wid, int hei)
    {
        mapLayout = map;
        width = wid;
        height = hei;
    }

    public void FireSpread(Span<bool> result, Span<bool> original, Span<bool> water)
    {
        FireSpreadUnit(result, original);
        BlockFireByMap(result);
        BlockFireByWater(result, water);
    }

    private void FireSpreadUnit(Span<bool> result, Span<bool> original)
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (original[i])
            {
                int posX = i % width;
                int posY = i / width;
                if (0 < posX)
                {
                    result[i - 1] = true;
                }
                if ((posX + 1) < width)
                {
                    result[i + 1] = true;
                }
                if (0 < posY)
                {
                    result[i - width] = true;
                }
                if ((posY + 1) < height)
                {
                    result[i + width] = true;
                }
            }
        }
    }

    private void BlockFireByMap(Span<bool> result)
    {
        for (int i = 0; i < result.Length; ++i)
        {
            if (mapLayout[i])
            {
                result[i] = false;
            }
        }
    }

    private void BlockFireByWater(Span<bool> result, Span<bool> water)
    {
        for (int i = 0; i < result.Length; ++i)
        {
            if (water[i])
            {
                result[i] = false;
            }
        }
    }
}

public class WaterCalculator
{
    private bool[] mapLayout;
    private int width;
    private int height;

    public WaterCalculator(bool[] map, int wid, int hei)
    {
        mapLayout = map;
        width = wid;
        height = hei;
    }

    public void WaterCalculate(Span<bool> result, Span<int> unitMap, Span<UnitFacing> unitFacing)
    {
        ReadOnlySpan<float> levels = SaveDataManager.Instance.Access<UnitLevelChunk>((int)SaveDataManager.SaveDataChunk.UnitLevel).data.Span;
        for(int i = 0; i < unitMap.Length; i++)
        {
            if (unitMap[i] >= 0)
            {
                UnitWaterCalc(result, i % width, i / width, unitMap[i], (int)levels[unitMap[i]], unitFacing[unitMap[i]]);

            }
        }
    }

    private void UnitWaterCalc(Span<bool> result, int unitPosX, int unitPosY, int unitID, int unitLevel, UnitFacing facing)
    {
        RangeData[] range = UnitDataBase.Datas[unitID].RangeData;
        for(int i = 0; i < unitLevel; i++)
        {
            for(int j = 0; j < range[i].range.Length; j++)
            {
                var (relTargetX, relTargetY) = RotatePos(range[i].range[j].relativeX, range[i].range[j].relativeY, facing);
                bool isValid = !CheckIsBlocked(unitPosX, unitPosY, unitPosX + relTargetX, unitPosY + relTargetY);
                if (isValid)
                {
                    result[(unitPosY + relTargetY) * width + unitPosX + relTargetX] = true;
                }
            }
        }
    }

    private (int relRangePosX, int relRangePosY) RotatePos(int relPosX, int relPosY, UnitFacing facing)
    {
        switch(facing)
        {
            case UnitFacing.North:
                return(relPosX, relPosY);
            case UnitFacing.East:
                return(relPosY, -relPosX);
            case UnitFacing.South:
                return(-relPosX, -relPosY);
            case UnitFacing.West:
                return(-relPosY, relPosX);
        }
        return (relPosX, relPosY);
    }

    private bool CheckIsBlocked(int unitPosX, int unitPosY, int targetPosX, int targetPosY)
    {
        var (wid, hei, startX, startY)= GetCheckRange(unitPosX, unitPosY, targetPosX, targetPosY);
        for(int i = 0; i < hei; i++)
        {
            for(int j = 0; j < wid; j++)
            {
                if(CheckIsThrough(startX + j, startY + i, unitPosX, unitPosY, targetPosX, targetPosY) && mapLayout[(startX + j) + (width * (i + startY))])
                {
                    return true;
                }
            }
        }
        return false;
    }

    private (int wid, int hei, int startX, int startY) GetCheckRange(int unitPosX, int unitPosY, int targetPosX, int targetPosY)
    {
        int wid = Math.Abs(targetPosX - unitPosX) + 1;
        int hei = Math.Abs(targetPosY - unitPosY) + 1;
        int startX = 0;
        if(unitPosX > targetPosX)
        {
            startX = targetPosX;
        }
        else
        {
            startX = unitPosX;
        }
        int startY = 0;
        if(unitPosY > targetPosY)
        {
            startY = targetPosY;
        }
        else
        {
            startY = unitPosY;
        }
        return (wid, hei, startX, startY);
    }

    private bool CheckIsThrough(int checkPosX, int checkPosY, int unitPosX, int unitPosY, int targetPosX, int targetPosY)
    {
        int vecX = targetPosX - unitPosX;
        int vecY = targetPosY - unitPosY;
        int posX = unitPosX * 2 + 1;
        int posY = unitPosY * 2 + 1;
        Span<int> result = stackalloc int[4];
        result[0] = CalcCrossProduct(vecX * 2, vecY * 2, posX, posY, checkPosX * 2, checkPosY * 2);
        result[1] = CalcCrossProduct(vecX * 2, vecY * 2, posX, posY, (checkPosX + 1) * 2, checkPosY * 2);
        result[2] = CalcCrossProduct(vecX * 2, vecY * 2, posX, posY, checkPosX * 2, (checkPosY + 1) * 2);
        result[3] = CalcCrossProduct(vecX * 2, vecY * 2, posX, posY, (checkPosX + 1) * 2, (checkPosY + 1) * 2);
        int max = -1000;
        int min = 1000;
        if (result[0] * result[1] * result[2] * result[3]  == 0)
        {
            return true;
        }
        for(int i = 0; i < 4; i++)
        {
            if(max < result[i])
            {
                max = result[i];
            }
            if(min > result[i])
            {
                min = result[i];
            }
        }
        if(max > 0 && min < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int CalcCrossProduct(int vecX, int vecY, int posX, int posY, int checkPosX, int checkPosY)
    {
        long result = vecX * (checkPosY - posY) - vecY * (checkPosX - posX);
        if(result < 0)
        {
            return -1;
        }
        if(result == 0)
        {
            return 0;
        }
        if(result > 0)
        {
            return 1;
        }
        return 0;
    }
}

public enum UnitFacing
{
    North,
    East,
    South,
    West
}
