using System.Collections.Generic;
using System.IO;
using Ray.FileIO;

public static class TextDataBase
{
    public static Dictionary<string, string> Text {  get; private set; }

    private static readonly string filelistPath = Path.Combine("Contents", "Text", "Text", "filelist_text.csv");
    private static readonly string filesPath = Path.Combine("Contents", "Text", "Text", "Files");

    public static void Setup()
    {
        Text = new();
    }

    public static void Load(int index)
    {
        string[] filenames = RayFileLoader.LoadCSVAll(filelistPath);
        for(int i = 0; i < filenames.Length; i++)
        {
            string[] keys = RayFileLoader.LoadCSVVertical(Path.Combine(filesPath, filenames[i]), 0);
            string[] values = RayFileLoader.LoadCSVVertical(Path.Combine(filesPath, filenames[i]), index + 1);
            for(int j = 0; j < keys.Length; j++)
            {
                Text[keys[j]] = values[j];
            }
        }
    }
}
