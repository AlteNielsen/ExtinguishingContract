using Ray.Data;
using Ray.FileIO;
using System;

public class BurningSituationChunk : RayDataChunk
{
    private const string path = "burning_situation.json";

    public override void Save()
    {
        BurningSituationData value = new BurningSituationData();
        value.burning_situation = CulculateLibrary.SwitchFloatToBool(data);
        RaySaveDataIO.SaveJson<BurningSituationData>(path, value);
    }

    public override float[] Load()
    {
        BurningSituationData value = RaySaveDataIO.LoadSaveData<BurningSituationData>(path);
        bool[] result = new bool[MapDataBase.Datas.Length];
        if (value.burning_situation.Length > MapDataBase.Datas.Length)
        {
            Array.Copy(value.burning_situation, result, MapDataBase.Datas.Length);
        }
        else
        {
            Array.Copy(value.burning_situation, result, value.burning_situation.Length);
        }
        return CulculateLibrary.SwitchBoolToFloat(result);
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<BurningSituationData>(path);
    }
}

[Serializable]
public class BurningSituationData
{
    public bool[] burning_situation;
}


public class BlockIndicatorChunk : RayDataChunk
{
    private const string path = "block_indicator.json";

    public override void Save()
    {
        BlockIndicatorData value = new BlockIndicatorData();
        value.block_indicators = CulculateLibrary.SwitchFloatToBool(data);
        RaySaveDataIO.SaveJson<BlockIndicatorData>(path, value);
    }

    public override float[] Load()
    {
        BlockIndicatorData value = RaySaveDataIO.LoadSaveData<BlockIndicatorData>(path);
        bool[] result = new bool[ExtinguishingContract.EIndicatorNum * Config.Data.IndicatorMaxLv];
        int beforeMaxLevel = value.block_indicators.Length / ExtinguishingContract.EIndicatorNum;
        for(int i = 0; i < ExtinguishingContract.EIndicatorNum; i++)
        {
            if(beforeMaxLevel > Config.Data.IndicatorMaxLv)
            {
                Array.Copy(value.block_indicators, beforeMaxLevel * i, result, Config.Data.IndicatorMaxLv * i, Config.Data.IndicatorMaxLv);
            }
            else
            {
                Array.Copy(value.block_indicators, beforeMaxLevel * i, result, Config.Data.IndicatorMaxLv * i, beforeMaxLevel);
            }
        }
        return CulculateLibrary.SwitchBoolToFloat(result);
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<BlockIndicatorData>(path);
    }
}

[Serializable]
public class BlockIndicatorData
{
    public bool[] block_indicators;
}


public class OtherProgressChunk : RayDataChunk
{
    private const string path = "other_progress.json";

    public override void Save()
    {
        OtherProgressData progress = new();
        progress.other_block_progress = data.Span[0];
        RaySaveDataIO.SaveJson<OtherProgressData>(path, progress);
    }

    public override float[] Load()
    {
        OtherProgressData progress = RaySaveDataIO.LoadSaveData<OtherProgressData>(path);
        return new float[] { progress.other_block_progress };
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<OtherProgressData>(path);
    }
}

[Serializable]
public class OtherProgressData
{
    public float other_block_progress;
}
