using Ray.FileIO;
using Ray.Data;
using System;

public class SettingChunk : RayDataChunk
{
    private const string path = "setting.json";

    public override void Save()
    {
        SettingData setting = new SettingData();
        setting.master_volume = data.Span[0];
        setting.bgm_volume = data.Span[1];
        setting.se_volume = data.Span[2];
        setting.display_mode = (int)data.Span[3];
        setting.display_size = (int)data.Span[4];
        setting.lang = (int)data.Span[5];
        RaySaveDataIO.SaveJson<SettingData>(path, setting);
    }

    public override float[] Load()
    {
        SettingData setting = RaySaveDataIO.LoadSaveData<SettingData>(path);
        float[] values = new float[] {
            setting.master_volume,
            setting.bgm_volume,
            setting.se_volume,
            setting.display_mode,
            setting.display_size,
            setting.lang};
        return values;
    }

    public override void Initialize()
    {
        
    }

    public int GetLang()
    {
        return (int)data.Span[5];
    }
}

[Serializable]
public class SettingData
{
    public float master_volume;
    public float bgm_volume;
    public float se_volume;
    public int display_mode;
    public int display_size;
    public int lang;
}

