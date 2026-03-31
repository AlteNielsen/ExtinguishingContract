using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Ray.FileIO;

public static class TextureDataBase
{
    public static Dictionary<string, Sprite> UnitIcon {  get; private set; }
    public static Dictionary<string, Sprite> MapNormalBG {  get; private set; }
    public static Dictionary<string, Sprite> MapBurningBG { get; private set; }
    public static Dictionary<string, Sprite> MapView {  get; private set; }
    public static Dictionary<string, Sprite> IndicatorIcon { get; private set; }

    private static readonly string unitIconPath = Path.Combine("Contents", "Texture", "Unit");
    private static readonly string mapNormalBGPath = Path.Combine("Contents", "Texture", "Map", "NormalBG");
    private static readonly string mapBurningBGPath = Path.Combine("Contents", "Texture", "Map", "BurningBG");
    private static readonly string mapViewPath = Path.Combine("Contents", "Texture", "Map", "View");
    private static readonly string indicatorIconPath = Path.Combine("Contents", "Texture", "Indicator");

    public static void Setup()
    {
        UnitIcon = new();
        MapNormalBG = new();
        MapBurningBG = new();
        MapView = new();
        IndicatorIcon = new();
    }

    public static void Load()
    {
        UnitLoad();
        MapLoad();
        IndicatorLoad();
    }

    private static void UnitLoad()
    {
        for(int i = 0; i < UnitDataBase.Datas.Length; i++)
        {
            UnitIcon[UnitDataBase.Datas[i].Codename] = RayFileLoader.LoadSprite(Path.Combine(unitIconPath, UnitDataBase.Datas[i].IconPath));
        }
    }

    private static void MapLoad()
    {
        for(int i = 0; i < MapDataBase.Datas.Length; i++)
        {
            MapNormalBG[MapDataBase.Datas[i].Codename] = RayFileLoader.LoadSprite(Path.Combine(mapNormalBGPath, MapDataBase.Datas[i].NormalBGPath));
            MapBurningBG[MapDataBase.Datas[i].Codename] = RayFileLoader.LoadSprite(Path.Combine(mapBurningBGPath, MapDataBase.Datas[i].BurningBGPath));
            MapView[MapDataBase.Datas[i].Codename] = RayFileLoader.LoadSprite(Path.Combine(mapViewPath, MapDataBase.Datas[i].ViewPath));
        }
    }

    private static void IndicatorLoad()
    {
        IndicatorIcon["Deployment"] = RayFileLoader.LoadSprite(Path.Combine(indicatorIconPath, ExtinguishingIndicatorDataBase.Data.Deployment.icon_path));
        IndicatorIcon["UnitBasePressure"] = RayFileLoader.LoadSprite(Path.Combine(indicatorIconPath, ExtinguishingIndicatorDataBase.Data.UnitBasePressure.icon_path));
        IndicatorIcon["LvPressureRatio"] = RayFileLoader.LoadSprite(Path.Combine(indicatorIconPath, ExtinguishingIndicatorDataBase.Data.LvPressureRatio.icon_path));
        IndicatorIcon["UsablePressure"] = RayFileLoader.LoadSprite(Path.Combine(indicatorIconPath, ExtinguishingIndicatorDataBase.Data.UsablePressure.icon_path));
        IndicatorIcon["SpreadSpeed"] = RayFileLoader.LoadSprite(Path.Combine(indicatorIconPath, ExtinguishingIndicatorDataBase.Data.SpreadSpeed.icon_path));
        IndicatorIcon["StartTurn"] = RayFileLoader.LoadSprite(Path.Combine(indicatorIconPath, ExtinguishingIndicatorDataBase.Data.StartTurn.icon_path));
    }
}
