using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ETileAttribute
{
    NONE =          0,
    FLIPX =         1 << 1,
    FLIPY =         1 << 2,
    ROTATE90 =      1 << 3,
    ROTATE180 =     1 << 4,
    ALL =           FLIPX | FLIPY | ROTATE90 | ROTATE180,
    ROTATEANY =     ROTATE90 | ROTATE180,
}

public class TileSpriteNeighbor
{
    ETileEdge joinEdge;
    TileSprite tileSprite;

    public TileSpriteNeighbor(TileSprite tileSprite, ETileEdge joinEdge)
    {
        this.joinEdge = joinEdge;
        this.tileSprite = tileSprite;
    }

    public TileEdge GetEdge()
    {
        if (tileSprite == null)
            return null;

        return tileSprite.GetEdge(joinEdge.Opposite());
    }

    public ETileEdge GetJoinEdge()
    {
        return joinEdge;
    }

    public TileSprite GetTileSprite()
    {
        return tileSprite;
    }
}

public class TileSprite
{
    TileEdge[] edge = new TileEdge[(int)ETileEdge.COUNT];
    ETileAttribute tileFlags = 0;
    Sprite sprite = null;

    public TileSprite(Sprite sprite, ETileAttribute tileFlags = ETileAttribute.NONE)
    {
        this.sprite = sprite;
        this.tileFlags = tileFlags;

        SetTileEdges(sprite);
    }

    public TileEdge GetEdge(ETileEdge tileEdge)
    {
        return edge[(int)tileEdge];
    }

    void SetTileEdges(Sprite sprite)
    {
        for (int edgeIndex = 0; edgeIndex < (int)ETileEdge.COUNT; ++edgeIndex)
        {
            edge[edgeIndex] = new TileEdge(sprite, (ETileEdge)edgeIndex, tileFlags);
        }
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public ETileAttribute GetFlags()
    {
        return tileFlags;
    }

    public void SetupGameObject(GameObject go)
    {
        if (sprite != null)
        {
            SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = go.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = sprite;

            if (FlagUtil.IsSet(tileFlags, ETileAttribute.FLIPX))
                spriteRenderer.flipX = true;
            if (FlagUtil.IsSet(tileFlags, ETileAttribute.FLIPY))
                spriteRenderer.flipY = true;

            if (FlagUtil.IsExactly(tileFlags, (ETileAttribute.ROTATE180 | ETileAttribute.ROTATE90)))
                go.transform.Rotate(Vector3.back * 270);
            else if (FlagUtil.IsExactly(tileFlags, ETileAttribute.ROTATE180))
                go.transform.Rotate(Vector3.back * 180);
            else if (FlagUtil.IsExactly(tileFlags, ETileAttribute.ROTATE90))
                go.transform.Rotate(Vector3.back * 90);
        }
    }

    public bool CompareEdge(TileSprite neighborTileSprite, ETileEdge edge)
    {
        return TileEdge.CompareEdges(this, neighborTileSprite, edge);
    }
}

public class TileSpriteProperties
{
    string fileName = "";
    ETileAttribute spriteFlags = 0;
    Sprite sprite = null;

    public TileSpriteProperties(Sprite sprite)
    {
        this.sprite = sprite;
        fileName = sprite.name;

        SetTileFlags();
    }

    public List<TileSprite> CreateTileSprites()
    {
        List<TileSprite> tileSprites = new List<TileSprite>();

        if (sprite == null)
            return tileSprites;

        tileSprites.Add(new TileSprite(sprite));

        for(ETileAttribute eFlag = ETileAttribute.FLIPX; eFlag < spriteFlags; ++eFlag)
        {
            if (FlagUtil.IsExactly(spriteFlags, eFlag))
            {
                tileSprites.Add(new TileSprite(sprite, eFlag));
            }
        }

        return tileSprites;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    void SetTileFlags()
    {
        if (fileName == null || fileName.Length <= 0)
            return;

        int underscoreIndex = fileName.LastIndexOf("_");
        if (underscoreIndex < 0)
            return;
        int endIndex = fileName.IndexOf("(", underscoreIndex);
        if (endIndex < 0)
            endIndex = fileName.Length;

        string attributes = fileName.Substring(underscoreIndex + 1, endIndex - underscoreIndex - 1);

        for (int attributeIndex = 0; attributeIndex < attributes.Length; ++attributeIndex)
        {
            ETileAttribute eFlag = ConvertToTileFlag(attributes[attributeIndex]);
            spriteFlags |= eFlag;
        }
    }

    ETileAttribute ConvertToTileFlag(char flagChar)
    {
        switch(flagChar)
        {
            case 'X':
                return ETileAttribute.FLIPX;
            case 'Y':
                return ETileAttribute.FLIPY;
            case 'R':
                return ETileAttribute.ROTATEANY;
        }

        return ETileAttribute.NONE;
    }
}
