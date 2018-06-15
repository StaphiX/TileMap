using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ETileEdge
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT,
    COUNT
}

static class ETileEdgeUtil
{
    public static ETileEdge Opposite(this ETileEdge e)
    {
        int tileEdge = (int)e;
        int iOpposite = tileEdge + 2;

        if (iOpposite >= (int)ETileEdge.COUNT)
            iOpposite -= (int)ETileEdge.COUNT;

        return (ETileEdge)iOpposite;
    }

    public static Vector2Int GetVector2Int(this ETileEdge e)
    {
        switch (e)
        {
            case ETileEdge.TOP:
                return new Vector2Int(0, 1);
            case ETileEdge.RIGHT:
                return new Vector2Int(1, 0);
            case ETileEdge.BOTTOM:
                return new Vector2Int(0, -1);
            case ETileEdge.LEFT:
                return new Vector2Int(-1, 0);
            case ETileEdge.COUNT:
            default:
                return new Vector2Int(0, 0);
        }
    }
}


public class TileEdge
{
    const int SAMPLESIZE = 1;

    ETileEdge tileEdge;
    byte[] edgeSample;

    public TileEdge(Sprite sprite, ETileEdge edge)
    {
        Rect sourceRect = sprite.sourceRect();
        tileEdge = edge;

        switch (edge)
        {
            case ETileEdge.TOP:
                GetXEdge(sprite, (int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.width);
                break;
            case ETileEdge.RIGHT:
                GetYEdge(sprite, (int)sourceRect.xMax, (int)sourceRect.y, (int)sourceRect.height);
                break;
            case ETileEdge.BOTTOM:
                GetXEdge(sprite, (int)sourceRect.x, (int)sourceRect.yMax, (int)sourceRect.width);
                break;
            case ETileEdge.LEFT:
                GetYEdge(sprite, (int)sourceRect.x, (int)sourceRect.y, (int)sourceRect.height);
                break;
        }
    }

    public ETileEdge GetETileEdge()
    {
        return tileEdge;
    }

    public string GetString()
    {
        return Convert.ToBase64String(edgeSample);
    }

    private void GetXEdge(Sprite sprite, int iX, int iY, int iW)
    {
        byte[] tLeft = sprite.GetXBytes(iX, iY, SAMPLESIZE);
        byte[] tMid = sprite.GetXBytes(iX + iW / 2 - SAMPLESIZE / 2, iY, SAMPLESIZE);
        byte[] tRight = sprite.GetXBytes(iX + iW - SAMPLESIZE, iY, SAMPLESIZE);

        Color cLeft = new Color(tLeft[0] / 255.0f, tLeft[1] / 255.0f, tLeft[2] / 255.0f, tLeft[3] / 255.0f);
        Color cMid = new Color(tMid[0] / 255.0f, tMid[1] / 255.0f, tMid[2] / 255.0f, tMid[3] / 255.0f);
        Color cRight = new Color(tRight[0] / 255.0f, tRight[1] / 255.0f, tRight[2] / 255.0f, tRight[3] / 255.0f);

        edgeSample = tLeft.Concat(tMid).Concat(tRight).ToArray();
    }

    private void GetYEdge(Sprite sprite, int iX, int iY, int iH)
    {
        byte[] tTop = sprite.GetYBytes(iX, iY, SAMPLESIZE);
        byte[] tMid = sprite.GetYBytes(iX, iY + iH / 2 - SAMPLESIZE / 2, SAMPLESIZE);
        byte[] tBottom = sprite.GetYBytes(iX, iY + iH - SAMPLESIZE, SAMPLESIZE);

        Color cTop = new Color(tTop[0] / 255.0f, tTop[1] / 255.0f, tTop[2] / 255.0f, tTop[3] / 255.0f);
        Color cMid = new Color(tMid[0] / 255.0f, tMid[1] / 255.0f, tMid[2] / 255.0f, tMid[3] / 255.0f);
        Color cBottom = new Color(tBottom[0] / 255.0f, tBottom[1] / 255.0f, tBottom[2] / 255.0f, tBottom[3] / 255.0f);

        edgeSample = tTop.Concat(tMid).Concat(tBottom).ToArray();
    }
}
