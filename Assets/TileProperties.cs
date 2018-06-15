using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSprite
{
    TileEdge[] edge = new TileEdge[(int)ETileEdge.COUNT];
    Sprite sprite = null;

    public TileSprite(Sprite sprite)
    {
        this.sprite = sprite;
        SetTileEdges(sprite);
    }

    public void SetObjectSprite(GameObject go)
    {
        if (sprite != null)
        {
            SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = go.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprite;
        }
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
