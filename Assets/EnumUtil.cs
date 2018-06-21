using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class FlagUtil
{
    public static bool IsExactly<T>(T flags, T value) where T : struct
    {
        int flagsInt = (int)(object)flags;
        int valueInt = (int)(object)value;

        return (flagsInt & valueInt) == valueInt;
    }

    public static bool IsSet<T>(T flags, T value) where T : struct
    {
        int flagsInt = (int)(object)flags;
        int valueInt = (int)(object)value;

        return (flagsInt & valueInt) != 0;
    }

    public static void Set<T>(ref T flags, T value) where T : struct
    {
        int flagsInt = (int)(object)flags;
        int valueInt = (int)(object)value;

        flags = (T)(object)(flagsInt | valueInt);
    }

    public static void Unset<T>(ref T flags, T value) where T : struct
    {
        int flagsInt = (int)(object)flags;
        int valueInt = (int)(object)value;

        flags = (T)(object)(flagsInt & (~valueInt));
    }
}

