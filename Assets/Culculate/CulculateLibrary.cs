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
        result[0] = ExtinguishingIndicatorDataBase.Data.Deployment.base_value * (ContractIndicatorDataBase.Data.Deployment.start_ratio * lvs[0] * ((float)ContractIndicatorDataBase.Data.Deployment.final_ratio / ContractIndicatorDataBase.Data.Deployment.start_ratio) / ExtinguishingContract.CIndicatorMaxLv);
        result[1] = ExtinguishingIndicatorDataBase.Data.UnitBasePressure.base_value * (ContractIndicatorDataBase.Data.UnitBasePressure.start_ratio * lvs[1] * ((float)ContractIndicatorDataBase.Data.UnitBasePressure.final_ratio / ContractIndicatorDataBase.Data.UnitBasePressure.start_ratio) / ExtinguishingContract.CIndicatorMaxLv);
        result[2] = ExtinguishingIndicatorDataBase.Data.LvPressureRatio.base_value * (ContractIndicatorDataBase.Data.LvPressureRatio.start_ratio * lvs[2] * ((float)ContractIndicatorDataBase.Data.LvPressureRatio.final_ratio / ContractIndicatorDataBase.Data.LvPressureRatio.start_ratio) / ExtinguishingContract.CIndicatorMaxLv);
        result[3] = ExtinguishingIndicatorDataBase.Data.UsablePressure.base_value * (ContractIndicatorDataBase.Data.UsablePressure.start_ratio * lvs[3] * ((float)ContractIndicatorDataBase.Data.UsablePressure.final_ratio / ContractIndicatorDataBase.Data.UsablePressure.start_ratio) / ExtinguishingContract.CIndicatorMaxLv);
        result[4] = ExtinguishingIndicatorDataBase.Data.SpreadSpeed.base_value * (ContractIndicatorDataBase.Data.SpreadSpeed.start_ratio * lvs[4] * ((float)ContractIndicatorDataBase.Data.SpreadSpeed.final_ratio / ContractIndicatorDataBase.Data.SpreadSpeed.start_ratio) / ExtinguishingContract.CIndicatorMaxLv);
        result[5] = ExtinguishingIndicatorDataBase.Data.StartTurn.base_value * (ContractIndicatorDataBase.Data.StartTurn.start_ratio * lvs[5] * ((float)ContractIndicatorDataBase.Data.StartTurn.final_ratio / ContractIndicatorDataBase.Data.StartTurn.start_ratio) / ExtinguishingContract.CIndicatorMaxLv);

        result[6] = Config.Data.InitialChance - (Config.Data.InitialChance * (lvs[6] - 1) / ExtinguishingContract.CIndicatorMaxLv);
        result[7] = ContractIndicatorDataBase.Data.StartBurningBlock.start_ratio * lvs[7] * ((float)ContractIndicatorDataBase.Data.StartBurningBlock.final_ratio / ContractIndicatorDataBase.Data.StartBurningBlock.start_ratio) / ExtinguishingContract.CIndicatorMaxLv;
        result[8] = ContractIndicatorDataBase.Data.BurningSpeed.start_ratio * lvs[8] * ((float)ContractIndicatorDataBase.Data.BurningSpeed.final_ratio / ContractIndicatorDataBase.Data.BurningSpeed.start_ratio) / ExtinguishingContract.CIndicatorMaxLv;
        return result;
    }

    public static bool[] GameSituationSetup()
    {
        int startBlockNum = (int)IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span)[7];
        return nCrProcess(MapDataBase.Datas.Length, startBlockNum);
    }

    public static bool[] IndicatorUpdate()
    {
        if(ExtinguishingContract.EIndicatorNum * Config.Data.IndicatorMaxLv < ExtinguishingContract.IndicatorChoicesNum)
        {
            return nCrProcess(ExtinguishingContract.IndicatorChoicesNum, ExtinguishingContract.IndicatorChoicesNum);
        }
        else
        {
            return nCrProcess(ExtinguishingContract.EIndicatorNum * Config.Data.IndicatorMaxLv, ExtinguishingContract.IndicatorChoicesNum);
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

    public static int FloatToPercent(float value)
    {
        float result = value * 1000;
        result += 5;
        return (int)(result / 10);
    }

    public static (int basePressure, int increasePressure) CulculatePressures()
    {
        float bs = Config.Data.UnitBasePressure;
        float increase = Config.Data.UnitLvIncreaseRatio;
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        float[] baseValue = IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        int maxIndicatorLv = indicators.Length / ExtinguishingContract.EIndicatorNum;
        for (int i = 0; i < maxIndicatorLv; i++)
        {
            if (indicators[maxIndicatorLv + i] > 0.5f)
            {
                bs += (int)baseValue[1] * (i + 1);
            }

            if (indicators[(maxIndicatorLv * 2) + i] > 0.5f)
            {
                increase += (int)baseValue[2] * (i + 1);
            }
        }
        return ((int)bs, (int)increase);
    }

    public static int CulculateFireSpreadSpeed()
    {
        int result = Config.Data.SpreadSpeed;
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        float[] baseValue = IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        int maxIndicatorLv = indicators.Length / ExtinguishingContract.EIndicatorNum;
        for (int i = 0; i < maxIndicatorLv; i++)
        {
            if (indicators[(maxIndicatorLv * 4) + i] > 0.5f)
            {
                result += (int)baseValue[4] * (i + 1);
            }
        }
        return result;
    }

    public static int CulculateStartTurn()
    {
        int result = 0;
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        float[] baseValue = IndicatorBaseValues(SaveDataManager.Instance.Access<NowIDChunk>((int)SaveDataManager.SaveDataChunk.NowID).data.Span);
        int maxIndicatorLv = indicators.Length / ExtinguishingContract.EIndicatorNum;
        for (int i = 0; i < maxIndicatorLv; i++)
        {
            if (indicators[(maxIndicatorLv * 5) + i] > 0.5f)
            {
                result += (int)baseValue[5] * (i + 1);
            }
        }
        return result;
    }

    public static float CulculateChance()
    {
        float result = 0;
        ReadOnlySpan<float> selected = SaveDataManager.Instance.Access<IndicatorSelectChunk>((int)SaveDataManager.SaveDataChunk.IndicatorSelect).data.Span;
        for (int i = 0; i < selected.Length; i++)
        {
            if (selected[i] > 0.5f)
            {
                int level = i % Config.Data.IndicatorMaxLv + 1;
                result += Config.Data.IndicatorBaseChance * level;
            }
        }
        result += Config.Data.InitialChance;
        return result;
    }

    public static float CulculateChance(Span<bool> indicatorCondition)
    {
        float result = 0;
        ReadOnlySpan<float> indicators = SaveDataManager.Instance.Access<BlockIndicatorChunk>(((int)SaveDataManager.SaveDataChunk.BlockIndicator)).data.Span;
        int counter = 0;
        for (int i = 0; i < indicators.Length; i++)
        {
            if (indicators[i] > 0.5f)
            {
                if (indicatorCondition[counter])
                {
                    int level = i % Config.Data.IndicatorMaxLv + 1;
                    result += Config.Data.IndicatorBaseChance * level;
                }
                counter++;
            }
        }
        result += Config.Data.InitialChance;
        return result;
    }
}
