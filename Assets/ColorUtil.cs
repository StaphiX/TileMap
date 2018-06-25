using System;
using UnityEngine;

public static class ColorUtil
{
    public static string ToBase64String (this Color32[] c)
    {
        if (c.Length <= 0)
            return "";

        byte[] bytes = new byte[c.Length * 4];

        for(int colorIndex = 0; colorIndex < c.Length; ++colorIndex)
        {
            int byteColorIndex = colorIndex * 4;

            bytes[byteColorIndex] = c[colorIndex].r;
            bytes[byteColorIndex+1] = c[colorIndex].g;
            bytes[byteColorIndex+2] = c[colorIndex].b;
            bytes[byteColorIndex+3] = c[colorIndex].a;
        }

        return Convert.ToBase64String(bytes);
    }
}
