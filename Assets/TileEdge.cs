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

    public TileEdge(Sprite sprite, ETileEdge edge, ETileAttribute eTileFlags = ETileAttribute.NONE)
    {
        Rect spriteRect = sprite.sourceRect();
        RectInt sourceRect = new RectInt((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
        
        tileEdge = edge;

        Vector2Int vTL, vTR, vBR, vBL;
        GetCorners(sourceRect, eTileFlags, out vTL, out vTR, out vBR, out vBL);

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

        if(sprite.name.Contains("Tile1"))
        {
            Debug.Log(eTileFlags + " " + edge + " " + 
                ColorUtility.ToHtmlStringRGBA(new Color32(edgeSample[4], edgeSample[5], edgeSample[6], edgeSample[7])));
        }
    }

    private void GetCorners(RectInt sourceRect, ETileAttribute eTileFlags, out Vector2Int vTL, out Vector2Int vTR, out Vector2Int vBR, out Vector2Int vBL)
    {
        vTL = new Vector2Int(sourceRect.x, sourceRect.y);
        vTR = new Vector2Int(sourceRect.xMax, sourceRect.y);
        vBR = new Vector2Int(sourceRect.xMax, sourceRect.yMax);
        vBL = new Vector2Int(sourceRect.x, sourceRect.yMax);

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
            int iRotateOffset = 1;
            if (eRotate == ETileAttribute.ROTATE90)
                iRotateOffset = 1;
            else if (eRotate == ETileAttribute.ROTATE180)
                iRotateOffset = 2;
            else if (eRotate == (ETileAttribute.ROTATE180 | ETileAttribute.ROTATE90))
                iRotateOffset = 3;

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

    public string GetString()
    {
        return Convert.ToBase64String(edgeSample);
    }

    private void GetEdge(Sprite sprite, Vector2Int vStart, Vector2Int vEnd)
    {
        bool bHorizontal = true;
        if (Mathf.Abs(vEnd.y - vStart.y) > Mathf.Abs(vEnd.x - vStart.x))
            bHorizontal = false;

        if (bHorizontal)
            GetXEdge(sprite, vStart.x, vStart.y, vEnd.x - vStart.x);
        else
            GetYEdge(sprite, vStart.x, vStart.y, vEnd.y - vStart.y);
    }

    private void GetXEdge(Sprite sprite, int iX, int iY, int iW)
    {
        byte[] tLeft = sprite.GetXBytes(iX, iY, SAMPLESIZE);
        byte[] tMid = sprite.GetXBytes(iX + iW / 2 - SAMPLESIZE / 2, iY, SAMPLESIZE);
        byte[] tRight = sprite.GetXBytes(iX + iW - SAMPLESIZE, iY, SAMPLESIZE);

        Color cLeft = new Color(tLeft[0] / 255.0f, tLeft[1] / 255.0f, tLeft[2] / 255.0f, tLeft[3] / 255.0f);
        Color cMid = new Color(tMid[0] / 255.0f, tMid[1] / 255.0f, tMid[2] / 255.0f, tMid[3] / 255.0f);
        Color cRight = new Color(tRight[0] / 255.0f, tRight[1] / 255.0f, tRight[2] / 255.0f, tRight[3] / 255.0f);

        //Debug.Log(sprite.name + " " + tileEdge + " " + 
        //    ColorUtility.ToHtmlStringRGB(cLeft) + "," + 
        //    ColorUtility.ToHtmlStringRGB(cMid) + "," + 
        //    ColorUtility.ToHtmlStringRGB(cRight));

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

        //Debug.Log(sprite.name + " " + tileEdge + " " +
        //    ColorUtility.ToHtmlStringRGB(cTop) + "," +
        //    ColorUtility.ToHtmlStringRGB(cMid) + "," +
        //    ColorUtility.ToHtmlStringRGB(cBottom));

        edgeSample = tTop.Concat(tMid).Concat(tBottom).ToArray();
    }
}
