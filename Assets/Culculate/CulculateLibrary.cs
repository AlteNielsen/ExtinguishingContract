using System;

public static class CulculateLibrary
{
    public static float[] SwitchBoolToFloat(bool[] value)
    {
        float[] result = new float[value.Length];
        for (int i = 0; i < result.Length; i++)
        {
            if (value[i])
            {
                result[i] = 1;
            }
            else
            {
                result[i] = 0;
            }
        }
        return result;
    }

    public static bool[] SwitchFloatToBool(ReadOnlyMemory<float> value)
    {
        bool[] result = new bool[value.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = value.Span[i] > 0.5f;
        }
        return result;
    }

    public static int[] SwitchFloatToInt(ReadOnlyMemory<float> value)
    {
        int[] result = new int[value.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (int)value.Span[i];
        }
        return result;
    }
}
