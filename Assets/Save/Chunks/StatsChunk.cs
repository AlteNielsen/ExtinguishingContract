using System;
using Ray.FileIO;
using Ray.Data;

public class ResultStatsChunk : RayDataChunk
{
    private const string path = "result_stats.json";

    public override void Save()
    {
        ResultStatsData value = new();
        value.go_times = (int)data.Span[0];
        value.success_times = (int)data.Span[1];
        value.average_extinguish_chance = data.Span[2];
        value.extinguish_times = (int)data.Span[3];
        value.sum_other_progress = (int)data.Span[4];
        value.deployment_count = (int)data.Span[5];
        value.base_press_count = (int)data.Span[6];
        value.press_mult_count = (int)data.Span[7];
        value.max_press_count = (int)data.Span[8];
        value.burn_speed_count = (int)data.Span[9];
        value.start_turn_count = (int)data.Span[10];
        RaySaveDataIO.SaveJson<ResultStatsData>(path, value);
    }

    public override float[] Load()
    {
        ResultStatsData value = RaySaveDataIO.LoadSaveData<ResultStatsData>(path);
        float[] result = new float[] {
            value.go_times,
            value.success_times,
            value.average_extinguish_chance,
            value.extinguish_times,
            value.sum_other_progress,
            value.deployment_count,
            value.base_press_count,
            value.press_mult_count,
            value.max_press_count,
            value.burn_speed_count,
            value.start_turn_count
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
    public int go_times;
    public int success_times;
    public float average_extinguish_chance;
    public int extinguish_times;
    public int sum_other_progress;

    public int deployment_count;
    public int base_press_count;
    public int press_mult_count;
    public int max_press_count;
    public int burn_speed_count;
    public int start_turn_count;
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