using System;
using System.IO;
using UnityEngine;
using Ray.FileIO;

public static class MapDataBase
{
    public static MapData[] Datas {  get; private set; }
    
    private static readonly string namefilePath = Path.Combine("Contents", "Master", "Map", "filelist_map.csv");
    private static readonly string unitfolderPath = Path.Combine("Contents", "Master", "Map", "Files");

    public static void MapDataSetup()
    {
        string[] filenames = RayFileLoader.LoadCSVAll(namefilePath);
        Datas = new MapData[filenames.Length];
        for (int i = 0; i < filenames.Length; i++)
        {
            Datas[i] = RayFileLoader.LoadJSON<MapData>(Path.Combine(unitfolderPath, filenames[i]));
            Datas[i].Setup();
        }
    }
}

[Serializable]
public class MapData
{
    [SerializeField] private string codename;
    [SerializeField] private string normal_bg_path;
    [SerializeField] private string burning_bg_path;
    [SerializeField] private string view_path;
    [SerializeField] private MapLayout data;

    public string Codename { get; private set; }
    public string NormalBGPath { get; private set; }
    public string BurningBGPath { get; private set; }
    public string ViewPath { get; private set; }

    public MapLayout Data { get; private set; }

    
    public void Setup()
    {
        Codename = codename;
        NormalBGPath = normal_bg_path;
        BurningBGPath = burning_bg_path;
        ViewPath = view_path;
        Data = data;
    }
}

[Serializable]
public class MapLayout
{
    public int width;
    public int height;
    public int[] layout;
    public int fire_point;
}
