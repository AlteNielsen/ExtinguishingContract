using System;
using Ray.FileIO;
using Ray.Data;

public class ResultStatsChunk : RayDataChunk
{
    private const string path = "result_stats.json";

    public override void Save()
    {
        ResultStatsData value = new();
        value.success_times = (int)data.Span[0];
        value.average_extinguish_chance = data.Span[1];
        value.extinguish_times = (int)data.Span[2];
        value.average_extinguish_rate = data.Span[3];
        RaySaveDataIO.SaveJson<ResultStatsData>(path, value);
    }

    public override float[] Load()
    {
        ResultStatsData value = RaySaveDataIO.LoadSaveData<ResultStatsData>(path);
        float[] result = new float[] {
            value.success_times,
            value.average_extinguish_chance,
            value.extinguish_times,
            value.average_extinguish_rate
        };
        return result;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<ResultStatsData>(path);
    }
}

[Serializable]
public class ResultStatsData
{
    public int success_times;
    public float average_extinguish_chance;
    public int extinguish_times;
    public float average_extinguish_rate;
}


public class TurnStatsChunk : RayDataChunk
{
    private const string path = "turn_stats.json";

    public override void Save()
    {
        TurnStatsData value = new();
        value.burning_block_nums = CulculateLibrary.SwitchFloatToInt(data);
        RaySaveDataIO.SaveJson<TurnStatsData>(path, value);
    }

    public override float[] Load()
    {
        TurnStatsData value = RaySaveDataIO.LoadSaveData<TurnStatsData>(path);
        float[] result = new float[100];
        if(result.Length < value.burning_block_nums.Length)
        {
            Array.Copy(value.burning_block_nums, result, result.Length);
        }
        else
        {
            Array.Copy(value.burning_block_nums, result, value.burning_block_nums.Length);
        }
        return result;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<TurnStatsData>(path);
    }
}

[Serializable]
public class TurnStatsData
{
    public int[] burning_block_nums;
}