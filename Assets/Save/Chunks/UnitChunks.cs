using System;
using Ray.Data;
using Ray.FileIO;

public class UnitSelectChunk : RayDataChunk
{
    private const string path = "unit_select.json";

    public override void Save()
    {
        UnitSelectData value = new();
        value.selected_units = CulculateLibrary.SwitchFloatToInt(data);
        RaySaveDataIO.SaveJson<UnitSelectData>(path, value);
    }

    public override float[] Load()
    {
        UnitSelectData value = RaySaveDataIO.LoadSaveData<UnitSelectData>(path);
        int maxHeight = 0;
        for(int i = 0; i < MapDataBase.Datas.Length; i++)
        {
            if(maxHeight < MapDataBase.Datas[i].Data.height)
            {
                maxHeight = MapDataBase.Datas[i].Data.height;
            }
        }
        float[] result = new float[maxHeight];
        Array.Fill<float>(result, -1);
        if(value.selected_units.Length > maxHeight)
        {
            Array.Copy(value.selected_units, result, result.Length);
        }
        else
        {
            Array.Copy(value.selected_units, result, value.selected_units.Length);
        }
        return result;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<UnitSelectData>(path);
    }
}

[Serializable]
public class UnitSelectData
{
    public int[] selected_units;
}

public class UnitLevelChunk : RayDataChunk
{
    private const string path = "unit_level.json";

    public override void Save()
    {
        UnitLevelData value = new();
        value.unit_lvs = CulculateLibrary.SwitchFloatToInt(data);
        RaySaveDataIO.SaveJson<UnitLevelData>(path, value);
    }

    public override float[] Load()
    {
        UnitLevelData value = RaySaveDataIO.LoadSaveData<UnitLevelData>(path);
        int maxHeight = 0;
        for (int i = 0; i < MapDataBase.Datas.Length; i++)
        {
            if (maxHeight < MapDataBase.Datas[i].Data.height)
            {
                maxHeight = MapDataBase.Datas[i].Data.height;
            }
        }
        float[] result = new float[maxHeight];
        if (value.unit_lvs.Length > maxHeight)
        {
            Array.Copy(value.unit_lvs, result, result.Length);
        }
        else
        {
            Array.Copy(value.unit_lvs, result, value.unit_lvs.Length);
        }
        return result;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<UnitLevelData>(path);
    }
}

[Serializable]
public class UnitLevelData
{
    public int[] unit_lvs;
}