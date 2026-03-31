using UnityEngine;
using System;
using System.IO;
using Ray.FileIO;

public static class UnitDataBase
{
    public static UnitData[] Datas {  get; private set; }

    private static readonly string namefilePath = Path.Combine("Contents", "Master", "Unit", "filelist_unit.csv");
    private static readonly string unitfolderPath = Path.Combine("Contents", "Master", "Unit", "Files");

    public static void UnitDataSetup()
    {
        string[] filenames = RayFileLoader.LoadCSVAll(namefilePath);
        Datas = new UnitData[filenames.Length];
        for (int i = 0; i < filenames.Length; i++)
        {
            Datas[i] = RayFileLoader.LoadJSON<UnitData>(Path.Combine(unitfolderPath, filenames[i]));
            Datas[i].Setup();
        }
    }
}

[Serializable]
public class UnitData
{
    [SerializeField] private string codename;
    [SerializeField] private string icon_path;
    [SerializeField] private RangeData[] range_data;
    
    public string Codename { get; private set; }
    public string IconPath { get; private set; }
    public RangeData[] RangeData { get; private set; }

    public void Setup()
    {
        Codename = codename;
        IconPath = icon_path;
        RangeData = range_data;
    }
}

[Serializable]
public class RangeData
{
    public RangePos[] range;
}

[Serializable]
public class RangePos
{
    public int relativeX;
    public int relativeY;
}
