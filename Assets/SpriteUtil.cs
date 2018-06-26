using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteUtil
{
    public static Rect sourceRect(this Sprite s)
    {
        if(s.packingMode != SpritePackingMode.Tight)
        {
            return s.textureRect;
        }

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

    public static Color32[] GetColorsX(this Sprite s, int iStartX, int iStartY, int iLength)
    {
        Rect sourceRect = s.sourceRect();
        Color32[] colors = new Color32[iLength];
        for (int index = 0; index < iLength; ++index)
        {
            int pixelX = index + iStartX;
            colors[index] = s.texture.GetPixel(pixelX, iStartY);
        }

        return colors;
    }

    public static Color32[] GetColorsY(this Sprite s, int iStartX, int iStartY, int iLength)
    {
        int iAbsLength = Mathf.Abs(iLength);
        Rect sourceRect = s.sourceRect();
        Color32[] colors = new Color32[iAbsLength];

        for (int index = 0; index < iAbsLength; ++index)
        {
            int pixelOffset = iLength < 0 ? -index : index;
            int pixelY = pixelOffset + iStartY;
            colors[index] = s.texture.GetPixel(iStartX, pixelY);
        }

        return colors;
    }
}
