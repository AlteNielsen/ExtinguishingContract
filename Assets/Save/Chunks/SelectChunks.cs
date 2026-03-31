using Ray.Data;
using Ray.FileIO;
using System;

public class MapSelectChunk : RayDataChunk
{
    private const string path = "map_select.json";

    public override void Save()
    {
        MapSelectData value = new();
        value.selected_map_index = (int)data.Span[0];
        RaySaveDataIO.SaveJson<MapSelectData>(path, value);
    }

    public override float[] Load()
    {
        MapSelectData value = RaySaveDataIO.LoadSaveData<MapSelectData>(path);
        return new float[] { value.selected_map_index };
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<MapSelectData>(path);
    }
}

[Serializable]
public class MapSelectData
{
    public int selected_map_index;
}

public class IndicatorSelectChunk : RayDataChunk
{
    private const string path = "indicator_select.json";
    private const int EIndicatorNum = 6;

    public override void Save()
    {
        IndicatorSelectData value = new();
        value.selected_indicator_data = CulculateLibrary.SwitchFloatToBool(data);
        RaySaveDataIO.SaveJson<IndicatorSelectData>(path, value);
    }

    public override float[] Load()
    {
        IndicatorSelectData value = RaySaveDataIO.LoadSaveData<IndicatorSelectData>(path);
        bool[] result = new bool[Config.Data.IndicatorMaxLv * EIndicatorNum];
        int dataLv = value.selected_indicator_data.Length / EIndicatorNum;
        for(int i = 0; i < EIndicatorNum; i++)
        {
            if(value.selected_indicator_data.Length > result.Length)
            {
                Array.Copy(value.selected_indicator_data, dataLv * i, result, Config.Data.IndicatorMaxLv * i, Config.Data.IndicatorMaxLv);
            }
            else
            {
                Array.Copy(value.selected_indicator_data, dataLv * i, result, Config.Data.IndicatorMaxLv * i, dataLv);
            }
        }
        return CulculateLibrary.SwitchBoolToFloat(result);
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<IndicatorSelectData>(path);
    }
}

[Serializable]
public class IndicatorSelectData
{
    public bool[] selected_indicator_data;
}
