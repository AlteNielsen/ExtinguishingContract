using System;
using Ray.Data;
using Ray.FileIO;

public class FireMapChunk : RayDataChunk
{
    private const string path = "fire_map.json";

    public override void Save()
    {
        FireMapData value = new();
        value.fire_map = CulculateLibrary.SwitchFloatToBool(data);
        RaySaveDataIO.SaveJson<FireMapData>(path, value);
    }

    public override float[] Load()
    {
        FireMapData value = RaySaveDataIO.LoadSaveData<FireMapData>(path);
        int maxLayout = 0;
        for(int i = 0; i < MapDataBase.Datas.Length; i++)
        {
            if(maxLayout < MapDataBase.Datas[i].Data.height * MapDataBase.Datas[i].Data.width)
            {
                maxLayout = MapDataBase.Datas[i].Data.height * MapDataBase.Datas[i].Data.width;
            }
        }
        bool[] result = new bool[maxLayout];
        if(result.Length < value.fire_map.Length)
        {
            Array.Copy(value.fire_map, result, result.Length);
        }
        else
        {
            Array.Copy(value.fire_map, result, value.fire_map.Length);
        }
        return CulculateLibrary.SwitchBoolToFloat(result);
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<FireMapData>(path);
    }
}

[Serializable]
public class FireMapData
{
    public bool[] fire_map;
}


public class UnitMapChunk : RayDataChunk
{
    private const string path = "unit_map.json";

    public override void Save()
    {
        UnitMapData value = new();
        value.unit_map = CulculateLibrary.SwitchFloatToInt(data);
        RaySaveDataIO.SaveJson<UnitMapData>(path, value);
    }

    public override float[] Load()
    {
        UnitMapData value = RaySaveDataIO.LoadSaveData<UnitMapData>(path);
        int maxLayout = 0;
        for (int i = 0; i < MapDataBase.Datas.Length; i++)
        {
            if (maxLayout < MapDataBase.Datas[i].Data.height * MapDataBase.Datas[i].Data.width)
            {
                maxLayout = MapDataBase.Datas[i].Data.height * MapDataBase.Datas[i].Data.width;
            }
        }
        float[] result = new float[maxLayout];
        Array.Fill<float>(result, -1);
        if(result.Length < value.unit_map.Length)
        {
            Array.Copy(value.unit_map, result, result.Length);
        }
        else
        { 
            Array.Copy(value.unit_map, result, value.unit_map.Length);
        }
        return result;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<UnitMapData>(path);
    }
}

[Serializable]
public class UnitMapData
{
    public int[] unit_map;
}


public class UnitFacingChunk : RayDataChunk
{
    private const string path = "unit_facing.json";

    public override void Save()
    {
        UnitFacingData value = new();
        value.unit_facing = CulculateLibrary.SwitchFloatToInt(data);
        RaySaveDataIO.SaveJson<UnitFacingData>(path, value);
    }

    public override float[] Load()
    {
        UnitFacingData value = RaySaveDataIO.LoadSaveData<UnitFacingData>(path);
        float[] result = new float[UnitDataBase.Datas.Length];
        if (value.unit_facing.Length > UnitDataBase.Datas.Length)
        {
            Array.Copy(value.unit_facing, result, result.Length);
        }
        else
        {
            Array.Copy(value.unit_facing, result, value.unit_facing.Length);
        }
        return result;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<UnitFacingData>(path);
    }
}

[Serializable]
public class UnitFacingData
{
    public int[] unit_facing;
}