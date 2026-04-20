using System;

public static class CulculateLibrary
{
    public static float[] SwitchBoolToFloat(bool[] value)
    {
        float[] result = new float[value.Length];
        for (int i = 0; i < result.Length; i++)
        {
            if (value[i])
            {
                result[i] = 1;
            }
            else
            {
                result[i] = 0;
            }
        }
        return result;
    }

    public static bool[] SwitchFloatToBool(ReadOnlyMemory<float> value)
    {
        bool[] result = new bool[value.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = value.Span[i] > 0.5f;
        }
        return result;
    }

    public static int[] SwitchFloatToInt(ReadOnlyMemory<float> value)
    {
        int[] result = new int[value.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (int)value.Span[i];
        }
        return result;
    }

    public static int ContractGrade(ReadOnlySpan<float> data)
    {
        int primaryValue = 11;
        int counter = 0;
        for(int i = 0; i < data.Length; i++)
        {
            if ((int)data[i] == 0)
            {
                counter += 10;
            }
            else
            {
                counter += (int)data[i];
            }
        }
        return primaryValue * counter;
    }

    public static float[] IndicatorBaseValues(ReadOnlySpan<float> indicatorLevels)
    {
        float[] result = new float[9];
        int[] lvs = new int[9];
        for(int i = 0; i < 9;i++)
        {
            if(indicatorLevels[i] == 0)
            {
                lvs[i] = 10;
            }
            else
            {
                lvs[i] = (int)indicatorLevels[i];
            }
        }
        result[0] = ExtinguishingIndicatorDataBase.Data.Deployment.base_value * (ContractIndicatorDataBase.Data.Deployment.start_ratio * lvs[0] * ((float)ContractIndicatorDataBase.Data.Deployment.final_ratio / ContractIndicatorDataBase.Data.Deployment.start_ratio) / 10);
        result[1] = ExtinguishingIndicatorDataBase.Data.UnitBasePressure.base_value * (ContractIndicatorDataBase.Data.UnitBasePressure.start_ratio * lvs[1] * ((float)ContractIndicatorDataBase.Data.UnitBasePressure.final_ratio / ContractIndicatorDataBase.Data.UnitBasePressure.start_ratio) / 10);
        result[2] = ExtinguishingIndicatorDataBase.Data.LvPressureRatio.base_value * (ContractIndicatorDataBase.Data.LvPressureRatio.start_ratio * lvs[2] * ((float)ContractIndicatorDataBase.Data.LvPressureRatio.final_ratio / ContractIndicatorDataBase.Data.LvPressureRatio.start_ratio) / 10);
        result[3] = ExtinguishingIndicatorDataBase.Data.UsablePressure.base_value * (ContractIndicatorDataBase.Data.UsablePressure.start_ratio * lvs[3] * ((float)ContractIndicatorDataBase.Data.UsablePressure.final_ratio / ContractIndicatorDataBase.Data.UsablePressure.start_ratio) / 10);
        result[4] = ExtinguishingIndicatorDataBase.Data.SpreadSpeed.base_value * (ContractIndicatorDataBase.Data.SpreadSpeed.start_ratio * lvs[4] * ((float)ContractIndicatorDataBase.Data.SpreadSpeed.final_ratio / ContractIndicatorDataBase.Data.SpreadSpeed.start_ratio) / 10);
        result[5] = ExtinguishingIndicatorDataBase.Data.StartTurn.base_value * (ContractIndicatorDataBase.Data.StartTurn.start_ratio * lvs[5] * ((float)ContractIndicatorDataBase.Data.StartTurn.final_ratio / ContractIndicatorDataBase.Data.StartTurn.start_ratio) / 10);

        result[6] = Config.Data.InitialChance - (Config.Data.InitialChance * (lvs[6] - 1) / 10);
        result[7] = ContractIndicatorDataBase.Data.StartBurningBlock.start_ratio * lvs[7] * ((float)ContractIndicatorDataBase.Data.StartBurningBlock.final_ratio / ContractIndicatorDataBase.Data.StartBurningBlock.start_ratio) / 10;
        result[8] = ContractIndicatorDataBase.Data.BurningSpeed.start_ratio * lvs[8] * ((float)ContractIndicatorDataBase.Data.BurningSpeed.final_ratio / ContractIndicatorDataBase.Data.BurningSpeed.start_ratio) / 10;
        return result;
    }

    public static bool[] GameSituationSetup()
    {
        int startBlockNum = (int)IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span)[7];
        return nCrProcess(MapDataBase.Datas.Length, startBlockNum);
    }

    public static bool[] IndicatorUpdate()
    {
        int EIndicatorNum = 6;
        int IndicatorSlotNum = 9;
        if(EIndicatorNum * Config.Data.IndicatorMaxLv < IndicatorSlotNum)
        {
            return nCrProcess(IndicatorSlotNum, IndicatorSlotNum);
        }
        else
        {
            return nCrProcess(EIndicatorNum * Config.Data.IndicatorMaxLv, IndicatorSlotNum);
        }
    }

    public static bool[] nCrProcess(int n, int r)
    {
        int[] ramdomizer = new int[n];
        for(int i = 0; i < n; i++)
        {
            ramdomizer[i] = i;
        }
        for(int i = 0; i < n * 2; i++)
        {
            int target = UnityEngine.Random.Range(1, n);
            int pool = ramdomizer[target];
            ramdomizer[target] = ramdomizer[0];
            ramdomizer[0] = pool;
        }
        bool[] result = new bool[n];
        for (int i = 0; i < r; i++)
        {
            result[ramdomizer[i]] = true;
        }
        return result;
    }
}
