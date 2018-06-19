using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ETileAttribute
{
    NONE =      0,
    ROTATE =    1 << 0,
    FLIPX =     1 << 1,
    FLIPY =     1 << 2
}

public class TileSprite
{
    string path = "";
    ETileAttribute spriteFlags = 0;
    ETileAttribute tileFlagsSet = 0;
    TileEdge[] edge = new TileEdge[(int)ETileEdge.COUNT];
    Sprite sprite = null;

    public TileSprite(Sprite sprite)
    {
        this.sprite = sprite;

        string tileInfo = UnityEditor.AssetDatabase.GetAssetPath(sprite.texture);
        string subString = "Atlas/";
        int substringIndex = tileInfo.LastIndexOf(subString);
        if (substringIndex < 0)
            substringIndex = 0;
        else
            substringIndex += subString.Length;

        path = tileInfo.Substring(substringIndex);

        SetTileFlags();
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

    void SetTileFlags()
    {
        if (path == null || path.Length <= 0)
            return;

        string filename = path.Substring(path.LastIndexOf("/") + 1);
        int underscoreIndex = filename.LastIndexOf("_");
        int dotIndex = Mathf.Max(filename.LastIndexOf("."), underscoreIndex+1);
        string attributes = filename.Substring(underscoreIndex + 1, dotIndex - underscoreIndex-1);

        for(int attributeIndex = 0; attributeIndex < attributes.Length; ++attributeIndex)
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
                return ETileAttribute.ROTATE;
        }

        return ETileAttribute.NONE;
    }
}
