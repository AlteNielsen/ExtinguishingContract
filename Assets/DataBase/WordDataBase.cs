using Ray.FileIO;
using System;
using System.IO;
using UnityEngine.UIElements;

public static class WordDataBase
{
    private static string[] words;
    private static int[] offsets = new int[8];

    private static readonly string unitCategoryPath = Path.Combine("Contents", "Text", "Word", "Unit", "unit_category.csv");
    private static readonly string unitNamePath = Path.Combine("Contents", "Text", "Word", "Unit", "unit_name.csv");
    private static readonly string mapTitlePath = Path.Combine("Contents", "Text", "Word", "Map", "map_title.csv");
    private static readonly string mapNamePath = Path.Combine("Contents", "Text", "Word", "Map", "map_name.csv");
    private static readonly string extTitlePath = Path.Combine("Contents", "Text", "Word", "Indicator", "ext_title.csv");
    private static readonly string extEffectPath = Path.Combine("Contents", "Text", "Word", "Indicator", "ext_effect.csv");
    private static readonly string langPath = Path.Combine("Contents", "Text", "Word", "Language", "lang.csv");

    public static void Setup()
    {
        int lengthCounter = 0;
        offsets[0] = 0;
        lengthCounter += RayFileLoader.LoadCSVVertical(unitCategoryPath, 0).Length;
        offsets[1] = lengthCounter;
        lengthCounter += RayFileLoader.LoadCSVVertical(unitNamePath, 0).Length;
        offsets[2] = lengthCounter;
        lengthCounter += RayFileLoader.LoadCSVVertical(mapTitlePath, 0).Length;
        offsets[3] = lengthCounter;
        lengthCounter += RayFileLoader.LoadCSVVertical(mapNamePath, 0).Length;
        offsets[4] = lengthCounter;
        lengthCounter += RayFileLoader.LoadCSVVertical(extTitlePath, 0).Length;
        offsets[5] = lengthCounter;
        lengthCounter += RayFileLoader.LoadCSVVertical(extEffectPath, 0).Length;
        offsets[6] = lengthCounter;
        lengthCounter += RayFileLoader.LoadCSVAll(langPath).Length;
        offsets[7] = lengthCounter;
        words = new string[lengthCounter];
    }

    public static void Load(int index)
    {
        Array.Copy(RayFileLoader.LoadCSVVertical(unitCategoryPath, index), 0, words, offsets[0], offsets[1] - offsets[0]);
        Array.Copy(RayFileLoader.LoadCSVVertical(unitNamePath, index), 0, words, offsets[1], offsets[2] - offsets[1]);
        Array.Copy(RayFileLoader.LoadCSVVertical(mapTitlePath, index), 0, words, offsets[2], offsets[3] - offsets[2]);
        Array.Copy(RayFileLoader.LoadCSVVertical(mapNamePath, index), 0, words, offsets[3], offsets[4] - offsets[3]);
        Array.Copy(RayFileLoader.LoadCSVVertical(extTitlePath, index), 0, words, offsets[4], offsets[5] - offsets[4]);
        Array.Copy(RayFileLoader.LoadCSVVertical(extEffectPath, index), 0, words, offsets[5], offsets[6] - offsets[5]);
        Array.Copy(RayFileLoader.LoadCSVAll(langPath), 0, words, offsets[6], offsets[7] - offsets[6]);
    }

    public static ReadOnlySpan<string> Word(WordSelector selector)
    {
        return words.AsSpan<string>().Slice(offsets[(int)selector], offsets[(int)selector + 1] - offsets[(int)selector]);
    }

    public enum WordSelector
    {
        UnitCategory,
        UnitName,
        MapTitle,
        MapName,
        EIndicatorTitle,
        EIndicatorEffect,
        Language
    }
}
