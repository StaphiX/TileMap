using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties
{
    TileEdge[] edge = new TileEdge[(int)ETileEdge.COUNT];
    Sprite sprite = null;

    public TileProperties(Sprite sprite)
    {
        this.sprite = sprite;
        SetTileEdges(sprite);
    }

    public TileEdge GetEdge(ETileEdge tileEdge)
    {
        return edge[(int)tileEdge];
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    void SetTileEdges(Sprite sprite)
    {
        for (int edgeIndex = 0; edgeIndex < (int)ETileEdge.COUNT; ++edgeIndex)
        {
            edge[edgeIndex] = new TileEdge(sprite, (ETileEdge)edgeIndex);
        }
    }
}
