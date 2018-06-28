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
    Color32[] edgeSample;

    public TileEdge(Sprite sprite, ETileEdge edge, ETileAttribute eTileFlags = ETileAttribute.NONE)
    {
        tileEdge = edge;

        if (this.tileEdge == ETileEdge.LEFT && sprite.name.Contains("WaterGrass_"))
        {
            Debug.Log(eTileFlags);
        }

        Vector2Int vEdgeStart, vEdgeEnd;
        GetEdgePoints(sprite, edge, eTileFlags, out vEdgeStart, out vEdgeEnd);
        SetEdgeSample(sprite, vEdgeStart, vEdgeEnd);       

        //if(sprite.name.Contains("Tile1"))
        //{
        //    Color32 debugCol = edgeSample[1];
        //    string colName = "RED";
        //    if (debugCol.r > 200 && debugCol.b > 200)
        //        colName = "MAGENTA";
        //    else if (debugCol.r > 200 && debugCol.g > 200)
        //        colName = "YELLOW";
        //    else if (debugCol.b > 200)
        //        colName = "CYAN";
        //    Debug.Log(eTileFlags + " " + edge + " " + colName);
        //}
    }

    private static void GetEdgePoints(Sprite sprite, ETileEdge edge, ETileAttribute eTileFlags, out Vector2Int vStart, out Vector2Int vEnd)
    {
        Rect spriteRect = sprite.sourceRect();
        RectInt sourceRect = new RectInt((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);

        Vector2Int vTL, vTR, vBR, vBL;
        GetCorners(sourceRect, eTileFlags, out vTL, out vTR, out vBR, out vBL);

        switch (edge)
        {
            case ETileEdge.TOP:
                vStart = vTL;
                vEnd = vTR;
                break;
            case ETileEdge.RIGHT:
                vStart = vTR;
                vEnd = vBR;
                break;
            case ETileEdge.BOTTOM:
                vStart = vBL;
                vEnd = vBR;
                break;
            case ETileEdge.LEFT:
                vStart = vTL;
                vEnd = vBL;
                break;
            default:
                vStart = vTL;
                vEnd = vTR;
                break;
        }
    }

    private static void GetCorners(RectInt sourceRect, ETileAttribute eTileFlags, out Vector2Int vTL, out Vector2Int vTR, out Vector2Int vBR, out Vector2Int vBL)
    {
        // Unity textures start in the bottom left so TOP = y + height
        vTL = new Vector2Int(sourceRect.x, sourceRect.yMax-1);
        vTR = new Vector2Int(sourceRect.xMax-1, sourceRect.yMax-1);
        vBR = new Vector2Int(sourceRect.xMax-1, sourceRect.y);
        vBL = new Vector2Int(sourceRect.x, sourceRect.y);

        if (eTileFlags == ETileAttribute.NONE)
            return;

        if (FlagUtil.IsSet(eTileFlags, ETileAttribute.FLIPX))
        {
            Vector2Int vTLCopy = vTL;
            Vector2Int vTRCopy = vTR;
            Vector2Int vBRCopy = vBR;
            Vector2Int vBLCopy = vBL;

            vTL.Set(vTRCopy.x, vTRCopy.y);
            vTR.Set(vTLCopy.x, vTLCopy.y);
            vBR.Set(vBLCopy.x, vBLCopy.y);
            vBL.Set(vBRCopy.x, vBRCopy.y);
        }

        if (FlagUtil.IsSet(eTileFlags, ETileAttribute.FLIPY))
        {
            Vector2Int vTLCopy = vTL;
            Vector2Int vTRCopy = vTR;
            Vector2Int vBRCopy = vBR;
            Vector2Int vBLCopy = vBL;

            vTL.Set(vBLCopy.x, vBLCopy.y);
            vTR.Set(vBRCopy.x, vBRCopy.y);
            vBR.Set(vTRCopy.x, vTRCopy.y);
            vBL.Set(vTLCopy.x, vTLCopy.y);
        }

        if (FlagUtil.IsSet(eTileFlags, ETileAttribute.ROTATEANY))
        {
            Vector2Int[] vCorners = { vTL, vTR, vBR, vBL };

            ETileAttribute eRotate = (eTileFlags & ETileAttribute.ROTATEANY);
            int iRotateOffset = 3;
            if (eRotate == ETileAttribute.ROTATE90)
                iRotateOffset = 3;
            else if (eRotate == ETileAttribute.ROTATE180)
                iRotateOffset = 2;
            else if (eRotate == (ETileAttribute.ROTATE180 | ETileAttribute.ROTATE90))
                iRotateOffset = 1;

            int iTL = iRotateOffset % 4;
            int iTR = (iRotateOffset + 1) % 4;
            int iBR = (iRotateOffset + 2) % 4;
            int iBL = (iRotateOffset + 3) % 4;

            vTL.Set(vCorners[iTL].x, vCorners[iTL].y);
            vTR.Set(vCorners[iTR].x, vCorners[iTR].y);
            vBR.Set(vCorners[iBR].x, vCorners[iBR].y);
            vBL.Set(vCorners[iBL].x, vCorners[iBL].y);
        }
    }

    public ETileEdge GetETileEdge()
    {
        return tileEdge;
    }

    public string GetHash()
    {
        return edgeSample.ToBase64String();
    }

    private void SetEdgeSample(Sprite sprite, Vector2Int vStart, Vector2Int vEnd)
    {
        bool bHorizontal = true;
        if (Mathf.Abs(vEnd.y - vStart.y) > Mathf.Abs(vEnd.x - vStart.x))
            bHorizontal = false;

        if (bHorizontal)
        {
            int iW = vEnd.x - vStart.x;
            iW += iW < 0 ? -1 : 1;
            GetXEdgeSample(sprite, vStart.x, vStart.y, iW);
        }
        else
        {
            int iH = vEnd.y - vStart.y;
            iH += iH < 0 ? -1 : 1;
            GetYEdgeSample(sprite, vStart.x, vStart.y, iH);
        }
    }

    private void GetXEdgeSample(Sprite sprite, int iX, int iY, int iW)
    {
        int leftOffset = 0;
        int rightOffset = iW < 0 ? SAMPLESIZE : -SAMPLESIZE;
        int midOffset = iW < 0 ? Mathf.CeilToInt(SAMPLESIZE / 2.0f) : Mathf.FloorToInt(-SAMPLESIZE / 2.0f);

        Color32[] tLeft = sprite.GetColorsX(iX + leftOffset, iY, SAMPLESIZE);
        Color32[] tMid = sprite.GetColorsX(iX + iW / 2 + midOffset, iY, SAMPLESIZE);
        Color32[] tRight = sprite.GetColorsX(iX + iW + rightOffset, iY, SAMPLESIZE);

        edgeSample = tLeft.Concat(tMid).Concat(tRight).ToArray();

        if (this.tileEdge == ETileEdge.LEFT && sprite.name.Contains("WaterGrass_"))
        {
            Color32 debugCol = tMid[0];
            string colName = "BLUE";
            if (debugCol.b < 200)
                colName = "GREEN";
            Debug.Log(this.tileEdge + " " + colName);
        }
    }

    private void GetYEdgeSample(Sprite sprite, int iX, int iY, int iH)
    {
        int sampleLength = iH < 0 ? -SAMPLESIZE : SAMPLESIZE;

        int topOffset = 0;
        int bottomOffset = iH < 0 ? SAMPLESIZE : -SAMPLESIZE;
        int midOffset = iH < 0 ? Mathf.CeilToInt(SAMPLESIZE/2.0f) : Mathf.FloorToInt(-SAMPLESIZE /2.0f);

        Color32[] tTop = sprite.GetColorsY(iX, iY + topOffset, sampleLength);
        Color32[] tMid = sprite.GetColorsY(iX, iY + iH / 2 + midOffset, sampleLength);
        Color32[] tBottom = sprite.GetColorsY(iX, iY + iH + bottomOffset, sampleLength);

        edgeSample = tTop.Concat(tMid).Concat(tBottom).ToArray();

        if (this.tileEdge == ETileEdge.LEFT && sprite.name.Contains("WaterGrass_"))
        {
            Color32 debugCol = tMid[0];
            string colName = "BLUE";
            if (debugCol.b < 200)
                colName = "GREEN";
            Debug.Log(this.tileEdge + " " + colName);
        }
    }

    public static bool CompareEdges(TileSprite tileSprite, TileSprite neighborTileSprite, ETileEdge tileEdge)
    {
        int pixelThreshold = 0;

        Sprite sprite = tileSprite.GetSprite();
        Sprite neighborSprite = neighborTileSprite.GetSprite();
        ETileAttribute spriteFlags = tileSprite.GetFlags();
        ETileAttribute neighborFlags = neighborTileSprite.GetFlags();

        Vector2Int vTileEdgeStart, vTileEdgeEnd, vNeighborEdgeStart, vNeighborEdgeEnd;
        GetEdgePoints(sprite, tileEdge, spriteFlags, out vTileEdgeStart, out vTileEdgeEnd);
        GetEdgePoints(neighborSprite, tileEdge.Opposite(), neighborFlags, out vNeighborEdgeStart, out vNeighborEdgeEnd);

        int tileWidth = vTileEdgeEnd.x - vTileEdgeStart.x;
        int tileHeight = vTileEdgeEnd.y - vTileEdgeStart.y;
        int neighborWidth = vNeighborEdgeEnd.x - vNeighborEdgeStart.x;
        int neighborHeight = vNeighborEdgeEnd.y - vNeighborEdgeStart.y;

        int absWidth = Math.Abs(tileWidth);
        int absHeight = Math.Abs(tileHeight);

        int totalPixelDiff = 0;
        int totalPixelCount = 0;

        if (absWidth > absHeight)
        {
            int tileY = vTileEdgeStart.y;
            int neighborY = vNeighborEdgeStart.y;
            for (int pixel = 0; pixel < absWidth; ++pixel)
            {
                int tileOffset = tileWidth < 0 ? -1 : 1;
                int neighborOffset = neighborWidth < 0 ? -1 : 1;
                int tileX = vTileEdgeStart.x + pixel * tileOffset;
                int neighborX = vNeighborEdgeStart.x + pixel * neighborOffset;

                Color32 spriteCol = sprite.GetColor(tileX, tileY);
                Color32 neighborCol = neighborSprite.GetColor(neighborX, neighborY);
                int pixelDiff = ComparePixel(spriteCol, neighborCol);
                totalPixelDiff += pixelDiff;
                ++totalPixelCount;
            }
        }
        else
        {
            int tileX = vTileEdgeStart.x;
            int neighborX = vNeighborEdgeStart.x;
            for (int pixel = 0; pixel < absHeight; ++pixel)
            {
                int tileOffset = tileHeight < 0 ? -1 : 1;
                int neighborOffset = neighborHeight < 0 ? -1 : 1;
                int tileY = vTileEdgeStart.y + pixel * tileOffset;
                int neighborY = vNeighborEdgeStart.y + pixel * neighborOffset;

                Color32 spriteCol = sprite.GetColor(tileX, tileY);
                Color32 neighborCol = neighborSprite.GetColor(neighborX, neighborY);
                int pixelDiff = ComparePixel(spriteCol, neighborCol);
                totalPixelDiff += pixelDiff;
                ++totalPixelCount;
            }
        }

        int averageDiff = Mathf.CeilToInt(totalPixelDiff / totalPixelCount);
        if (averageDiff <= pixelThreshold)
            return true;

        return false;
    }

    public static int ComparePixel(Color32 color, Color32 compare)
    {
        int diff = Math.Abs(color.r - compare.r);
        diff += Math.Abs(color.g - compare.g);
        diff += Math.Abs(color.b - compare.b);
        diff += Math.Abs(color.a - compare.a);

        int averageDiff = Mathf.CeilToInt((float)diff / 4.0f);

        return averageDiff;
    }
}
