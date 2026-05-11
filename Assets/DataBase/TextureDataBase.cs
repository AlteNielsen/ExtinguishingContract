using System.IO;
using UnityEngine;
using Ray.FileIO;
using System.Linq;
using System;

public static class TextureDataBase
{
    private static Texture2D[] textures;
    private static int[] offsets;

    private static readonly string folderPath = Path.Combine("Contents", "Texture");
    private static readonly string texFolderPath = "Contents" + "/" + "Texture";
    private static readonly string unitNotPaths = "files-unit-not.csv";
    private static readonly string unitSelectedPaths = "files-unit-selected.csv";
    private static readonly string mapNormalPaths = "files-map-normal.csv";
    private static readonly string mapBurningPaths = "files-map-burning.csv";
    private static readonly string indicatorPaths = "files-indicator.csv";

    public static void Setup()
    {
        offsets = new int[6];
        offsets[0] = 0;
        offsets[1] = RayFileLoader.LoadCSVAll(Path.Combine(folderPath, unitNotPaths)).Length;
        offsets[2] = RayFileLoader.LoadCSVAll(Path.Combine(folderPath, unitSelectedPaths)).Length + offsets.Sum();
        offsets[3] = RayFileLoader.LoadCSVAll(Path.Combine(folderPath, mapNormalPaths)).Length + offsets.Sum();
        offsets[4] = RayFileLoader.LoadCSVAll(Path.Combine(folderPath, mapBurningPaths)).Length + offsets.Sum();
        offsets[5] = RayFileLoader.LoadCSVAll(Path.Combine(folderPath, indicatorPaths)).Length + offsets.Sum();
        textures = new Texture2D[offsets.Sum()];
    }

    public static void Load()
    {
        RayFileLoader.LoadTextures((texFolderPath + "/" + "Unit"), RayFileLoader.LoadCSVAll(Path.Combine(folderPath, unitNotPaths)), textures.AsMemory(offsets[0], offsets[1] - offsets[0]));
        RayFileLoader.LoadTextures((texFolderPath + "/" + "Unit"), RayFileLoader.LoadCSVAll(Path.Combine(folderPath, unitSelectedPaths)), textures.AsMemory(offsets[1], offsets[2] - offsets[1]));
        RayFileLoader.LoadTextures((texFolderPath + "/" + "Map"),  RayFileLoader.LoadCSVAll(Path.Combine(folderPath, mapNormalPaths)), textures.AsMemory(offsets[2], offsets[3] - offsets[2]));
        RayFileLoader.LoadTextures((texFolderPath + "/" + "Map"), RayFileLoader.LoadCSVAll(Path.Combine(folderPath, mapBurningPaths)), textures.AsMemory(offsets[3], offsets[4] - offsets[3]));
        RayFileLoader.LoadTextures((texFolderPath + "/" + "Indicator"), RayFileLoader.LoadCSVAll(Path.Combine(folderPath, indicatorPaths)), textures.AsMemory(offsets[4], offsets[5] - offsets[4]));
    }

    public static ReadOnlySpan<Texture2D> GetTextures(GameTextures index)
    {
        return textures.AsSpan(offsets[(int)index], offsets[(int)index + 1] - offsets[(int)index]);
    }
}

public enum GameTextures
{
    UnitNot,
    UnitSelected,
    MapNormal,
    MapBurning,
    Indicator
}
