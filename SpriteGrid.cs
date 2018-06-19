using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class Grid
{
    public Vector2Int TILESIZE = new Vector2Int(64, 64);
    public int PIXELSPERUNIT = 64;
    public int PIXELSCALE = 1;

    public int gridX;
    public int gridY;
    public Vector2 gridCenter = Vector2.zero;

    public Vector2 GetGridCenter()
    {
        return gridCenter;
    }

    public Vector2 ScreenToGrid(Vector2 vScreenPos)
    {
        Vector2 vWorldPos = Camera.main.ScreenToWorldPoint(vScreenPos);
        return WorldToGrid(vWorldPos);
    }

    public Vector2 GridToWorld(Vector2 vGridPos)
    {
        Vector2 vGridCenter = GetGridCenter();

        Vector2 vGridOffset = vGridPos - vGridCenter;
        Vector2 vWorldOffset = (vGridOffset * TILESIZE) / PIXELSPERUNIT;

        return vWorldOffset;
    }

    public Vector2 WorldToGrid(Vector2 vWorldPos)
    {
        Vector2 vGridCenter = GetGridCenter();

        Vector2 vGridOffset = (vWorldPos * PIXELSPERUNIT) / TILESIZE;
        Vector2 vGridPos = vGridOffset + vGridCenter;

        return vGridPos;
    }

    public void SetupGridSprites()
    {

    }
}



