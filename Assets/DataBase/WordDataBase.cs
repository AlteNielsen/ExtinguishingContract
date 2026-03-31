using System.Collections.Generic;
using System.IO;
using Ray.FileIO;

public static class WordDataBase
{
    public static Dictionary<string, string> UnitCategory { get; private set; }
    public static Dictionary<string, string> UnitName { get; private set; }
    public static Dictionary<string, string> MapTitle {  get; private set; }
    public static Dictionary<string, string> MapName {  get; private set; }
    public static Dictionary<string, string> ContTitle {  get; private set; }
    public static Dictionary<string, string> ExtTitle {  get; private set; }
    public static Dictionary<string, string> ExtEffect {  get; private set; }
    public static string[] Language {  get; private set; }

    private static readonly string unitCategoryPath = Path.Combine("Contents", "Text", "Word", "Unit", "unit_category.csv");
    private static readonly string unitNamePath = Path.Combine("Contents", "Text", "Word", "Unit", "unit_name.csv");
    private static readonly string mapTitlePath = Path.Combine("Contents", "Text", "Word", "Map", "map_title.csv");
    private static readonly string mapNamePath = Path.Combine("Contents", "Text", "Word", "Map", "map_name.csv");
    private static readonly string contTitlePath = Path.Combine("Contents", "Text", "Word", "Indicator", "cont_title.csv");
    private static readonly string extTitlePath = Path.Combine("Contents", "Text", "Word", "Indicator", "ext_title.csv");
    private static readonly string extEffectPath = Path.Combine("Contents", "Text", "Word", "Indicator", "ext_effect.csv");
    private static readonly string langPath = Path.Combine("Contents", "Text", "Word", "Language", "lang.csv");

    public static void Setup()
    {
        UnitCategory = new();
        UnitName = new();
        MapTitle = new();
        MapName = new();
        ContTitle = new();
        ExtTitle = new();
        ExtEffect = new();
        Language = RayFileLoader.LoadCSVAll(langPath);
    }

    public static void Load(int index)
    {
        UCLoad(index);
        UNLoad(index);
        MTLoad(index);
        MNLoad(index);
        CTLoad(index);
        ETLoad(index);
        EELoad(index);
    }

    private static void UCLoad(int index)
    {
        string[] ucategory = RayFileLoader.LoadCSVVertical(unitCategoryPath, 0);
        string[] ucValue = RayFileLoader.LoadCSVVertical(unitCategoryPath, index + 1);
        for (int i = 0; i < ucategory.Length; i++)
        {
            UnitCategory[ucategory[i]] = ucValue[i];
        }
    }

    private static void UNLoad(int index)
    {
        string[] uname = RayFileLoader.LoadCSVVertical(unitNamePath, 0);
        string[] unValue = RayFileLoader.LoadCSVVertical(unitNamePath, index + 1);
        for (int i = 0; i < uname.Length; i++)
        {
            UnitName[uname[i]] = unValue[i];
        }
    }

    private static void MTLoad(int index)
    {
        string[] mtitle = RayFileLoader.LoadCSVVertical(mapTitlePath, 0);
        string[] mtValue = RayFileLoader.LoadCSVVertical(mapTitlePath, index + 1);
        for (int i = 0; i < mtitle.Length; i++)
        {
            MapTitle[mtitle[i]] = mtValue[i];
        }
    }

    private static void MNLoad(int index)
    {
        string[] mname = RayFileLoader.LoadCSVVertical(mapNamePath, 0);
        string[] mnValue = RayFileLoader.LoadCSVVertical(mapNamePath, index + 1);
        for (int i = 0; i < mname.Length; i++)
        {
            MapName[mname[i]] = mnValue[i];
        }
    }

    private static void CTLoad(int index)
    {
        string[] ctitle = RayFileLoader.LoadCSVVertical(contTitlePath, 0);
        string[] ctValue = RayFileLoader.LoadCSVVertical(contTitlePath, index + 1);
        for (int i = 0; i < ctitle.Length; i++)
        {
            ContTitle[ctitle[i]] = ctValue[i];
        }
    }

    private static void ETLoad(int index)
    {
        string[] etitle = RayFileLoader.LoadCSVVertical(extTitlePath, 0);
        string[] etValue = RayFileLoader.LoadCSVVertical(extTitlePath, index + 1);
        for (int i = 0; i < etitle.Length; i++)
        {
            ExtTitle[etitle[i]] = etValue[i];
        }
    }

    private static void EELoad(int index)
    {
        string[] eeffect = RayFileLoader.LoadCSVVertical(extEffectPath, 0);
        string[] eeValue = RayFileLoader.LoadCSVVertical(extEffectPath, index + 1);
        for (int i = 0; i < eeffect.Length; i++)
        {
            ExtEffect[eeffect[i]] = eeValue[i];
        }
    }
}
