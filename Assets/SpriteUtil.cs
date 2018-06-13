using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteUtil
{
    public static Rect sourceRect(this Sprite s)
    {
        Rect spriteRect = s.rect;
        Vector2[] uvs = s.uv;
        Texture2D sourceTex = s.texture;
        Vector2 tl = uvs[3];
        Vector2 br = uvs[2];

        float fX = sourceTex.width * tl.x;
        float fY = sourceTex.height * tl.y;
        float fW = sourceTex.width * br.x - fX;
        float fH = sourceTex.height * br.y - fY; ;

        return new Rect(fX, fY, fW, fH);
    }

    public static byte[] GetXBytes(this Sprite s, int iStartX, int iStartY, int iLength)
    {
        byte[] tBytes = new byte[iLength * 4];
        for (int iX = iStartX; iX < iStartX + iLength; ++iX)
        {
            int iIndex = iX - iStartX;
            Color c = s.texture.GetPixel(iX, iStartY);

            tBytes[iIndex] = (byte)(c.r * 255.0f);
            tBytes[iIndex + 1] = (byte)(c.g * 255.0f);
            tBytes[iIndex + 2] = (byte)(c.b * 255.0f);
            tBytes[iIndex + 3] = (byte)(c.a * 255.0f);
        }

        return tBytes;
    }

    public static byte[] GetYBytes(this Sprite s, int iStartX, int iStartY, int iLength)
    {
        byte[] tBytes = new byte[iLength * 4];
        for (int iY = iStartY; iY < iStartY + iLength; ++iY)
        {
            int iIndex = iY - iStartY;
            Color c = s.texture.GetPixel(iStartX, iY);

            tBytes[iIndex] = (byte)(c.r * 255.0f);
            tBytes[iIndex + 1] = (byte)(c.g * 255.0f);
            tBytes[iIndex + 2] = (byte)(c.b * 255.0f);
            tBytes[iIndex + 3] = (byte)(c.a * 255.0f);
        }

        return tBytes;
    }
}
