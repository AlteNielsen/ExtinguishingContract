using System;
using System.IO;
using Ray.FileIO;

public static class TextDataBase
{
    private static string[] Text;

    private static readonly string filelistPath = Path.Combine("Contents", "Text", "Text", "filelist_text.csv");
    private static readonly string filesPath = Path.Combine("Contents", "Text", "Text", "Files");

    private static int[] offsets;

    public static void Setup()
    {
        string[] filenames = RayFileLoader.LoadCSVAll(filelistPath);
        int counter = 0;
        offsets = new int[filenames.Length + 1];
        for (int i = 0; i < filenames.Length; i++)
        {
            string[] texts = RayFileLoader.LoadCSVVertical(Path.Combine(filesPath, filenames[i]), 0);
            offsets[i] = counter;
            counter += texts.Length;
        }
        Text = new string[counter];
        offsets[offsets.Length - 1] = counter;
    }

    public static void Load(int index)
    {
        string[] filenames = RayFileLoader.LoadCSVAll(filelistPath);
        int offset = 0;
        for(int i = 0; i < filenames.Length; i++)
        {
            string[] texts = RayFileLoader.LoadCSVVertical(Path.Combine(filesPath, filenames[i]), index);
            for(int j = 0; j < texts.Length; j++)
            {
                Text[offset + j] = texts[j];
            }
            offset += texts.Length;
        }
    }

    public static ReadOnlySpan<string> GetTexts(TextDictionary dic)
    {
        ReadOnlySpan<string> span = Text.AsSpan();
        return span.Slice(offsets[(int)dic], offsets[(int)dic + 1] -  offsets[(int)dic]);
    }

    public enum TextDictionary
    {
        Title,
        Contract,
        GameLoading
    }
}
