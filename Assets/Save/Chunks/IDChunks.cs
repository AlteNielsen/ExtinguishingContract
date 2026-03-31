using Ray.Data;
using Ray.FileIO;
using System;

public class NowIDChunk : RayDataChunk
{
    private const string path = "now_id.json";

    public override void Save()
    {
        IDData id = new IDData();
        id.deployment = (int)data.Span[0];
        id.unit_base_pressure = (int)data.Span[1];
        id.lv_pressure_ratio = (int)data.Span[2];
        id.usable_pressure = (int)data.Span[3];
        id.spread_speed = (int)data.Span[4];
        id.start_turn = (int)data.Span[5];
        id.initial_chance = (int)data.Span[6];
        id.start_burning_block = (int)data.Span[7];
        id.burning_speed = (int)data.Span[8];
        RaySaveDataIO.SaveJson(path, id);
    }

    public override float[] Load()
    {
        IDData id = RaySaveDataIO.LoadSaveData<IDData>(path);
        float[] values = new float[] {
            id.deployment, 
            id.unit_base_pressure, 
            id.lv_pressure_ratio,
            id.usable_pressure,
            id.spread_speed,
            id.start_turn,
            id.initial_chance,
            id.start_burning_block,
            id.burning_speed};
        return values;
    }

    public override void Initialize()
    {
        RaySaveDataIO.Initialize<IDData>(path);
    }
}

[Serializable]
public class IDData
{
    public int deployment;
    public int unit_base_pressure;
    public int lv_pressure_ratio;
    public int usable_pressure;
    public int spread_speed;
    public int start_turn;
    public int initial_chance;
    public int start_burning_block;
    public int burning_speed;
}

public class MaxIDChunk : RayDataChunk
{
    private const string path = "max_id.json";

    public override void Save()
    {
        IDData id = new IDData();
        id.deployment = (int)data.Span[0];
        id.unit_base_pressure = (int)data.Span[1];
        id.lv_pressure_ratio = (int)data.Span[2];
        id.usable_pressure = (int)data.Span[3];
        id.spread_speed = (int)data.Span[4];
        id.start_turn = (int)data.Span[5];
        id.initial_chance = (int)data.Span[6];
        id.start_burning_block = (int)data.Span[7];
        id.burning_speed = (int)data.Span[8];
        RaySaveDataIO.SaveJson(path, id);
    }

    public override float[] Load()
    {
        IDData id = RaySaveDataIO.LoadSaveData<IDData>(path);
        float[] values = new float[] {
            id.deployment,
            id.unit_base_pressure,
            id.lv_pressure_ratio,
            id.usable_pressure,
            id.spread_speed,
            id.start_turn,
            id.initial_chance,
            id.start_burning_block,
            id.burning_speed};
        return values;
    }

    public override void Initialize()
    {

    }
}