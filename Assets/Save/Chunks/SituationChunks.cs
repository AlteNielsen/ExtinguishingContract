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
    private const int EIndicatorNum = 6;

    public override void Save()
    {
        BlockIndicatorData value = new BlockIndicatorData();
        value.indicator_max_lv = Config.Data.IndicatorMaxLv;
        value.block_num = MapDataBase.Datas.Length;
        value.block_indicators = CulculateLibrary.SwitchFloatToBool(data);
        RaySaveDataIO.SaveJson<BlockIndicatorData>(path, value);
    }

    public override float[] Load()
    {
        BlockIndicatorData value = RaySaveDataIO.LoadSaveData<BlockIndicatorData>(path);
        bool[] result = new bool[MapDataBase.Datas.Length *  EIndicatorNum * Config.Data.IndicatorMaxLv];
        for(int i = 0; i < value.block_num; i++)
        {
            if (i < MapDataBase.Datas.Length)
            {
                if (value.indicator_max_lv > Config.Data.IndicatorMaxLv)
                {
                    Array.Copy(value.block_indicators, EIndicatorNum * value.indicator_max_lv * i, result, EIndicatorNum * Config.Data.IndicatorMaxLv * i, EIndicatorNum * Config.Data.IndicatorMaxLv);
                }
                else
                {
                    Array.Copy(value.block_indicators, EIndicatorNum * value.indicator_max_lv * i, result, EIndicatorNum * Config.Data.IndicatorMaxLv * i, value.indicator_max_lv * EIndicatorNum);
                }
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
    public int indicator_max_lv;
    public int block_num;
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
