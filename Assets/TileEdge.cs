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
        Rect spriteRect = sprite.sourceRect();
        RectInt sourceRect = new RectInt((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
        
        tileEdge = edge;

        Vector2Int vTL, vTR, vBR, vBL;
        GetCorners(sourceRect, eTileFlags, out vTL, out vTR, out vBR, out vBL);

        if (this.tileEdge == ETileEdge.LEFT && sprite.name.Contains("WaterGrass_"))
        {
            Debug.Log(eTileFlags);
        }

        switch (edge)
        {
            case ETileEdge.TOP:
                GetEdge(sprite, vTL, vTR);
                break;
            case ETileEdge.RIGHT:
                GetEdge(sprite, vTR, vBR);
                break;
            case ETileEdge.BOTTOM:
                GetEdge(sprite, vBL, vBR);
                break;
            case ETileEdge.LEFT:
                GetEdge(sprite, vTL, vBL);
                break;
        }

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

    private void GetCorners(RectInt sourceRect, ETileAttribute eTileFlags, out Vector2Int vTL, out Vector2Int vTR, out Vector2Int vBR, out Vector2Int vBL)
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

    private void GetEdge(Sprite sprite, Vector2Int vStart, Vector2Int vEnd)
    {
        bool bHorizontal = true;
        if (Mathf.Abs(vEnd.y - vStart.y) > Mathf.Abs(vEnd.x - vStart.x))
            bHorizontal = false;

        if (bHorizontal)
        {
            int iW = vEnd.x - vStart.x;
            iW += iW < 0 ? -1 : 1;
            GetXEdge(sprite, vStart.x, vStart.y, iW);
        }
        else
        {
            int iH = vEnd.y - vStart.y;
            iH += iH < 0 ? -1 : 1;
            GetYEdge(sprite, vStart.x, vStart.y, iH);
        }
    }

    private void GetXEdge(Sprite sprite, int iX, int iY, int iW)
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

    private void GetYEdge(Sprite sprite, int iX, int iY, int iH)
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
}
