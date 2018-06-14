using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using EdgeLookupType = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<TileProperties>>;

public class BrushManager
{
    List<SpriteAtlas> atlas = null;

    EdgeLookupType edgeTopLookup = new EdgeLookupType();
    EdgeLookupType edgeRightLookup = new EdgeLookupType();
    EdgeLookupType edgeBottomLookup = new EdgeLookupType();
    EdgeLookupType edgeLeftLookup = new EdgeLookupType);

    public BrushManager()
    {

    }

    public void Init()
    {
        LoadAllAtlas();
    }

    void LoadAllAtlas()
    {
        atlas = new List<SpriteAtlas>(Resources.LoadAll<SpriteAtlas>("Atlas"));

        foreach (SpriteAtlas spriteAtlas in atlas)
        {
            LoadAtlas(spriteAtlas);
        }
    }

    void LoadAtlas(SpriteAtlas atlas)
    {
        Sprite[] sprites = null;
        atlas.GetSprites(sprites);
        for (int sprite = 0; sprite < atlas.spriteCount; ++sprite)
        {
            RegisterSprite(sprites[sprite]);
        }
    }

    void RegisterSprite(Sprite sprite)
    {
        TileProperties properties = new TileProperties(sprite);
        RegisterTileEdges(properties);
    }

    public void RegisterTileEdges(TileProperties tileProp)
    {
        for (int edgeIndex = 0; edgeIndex < (int)ETileEdge.COUNT; ++edgeIndex)
        {
            ETileEdge tileEdge = (ETileEdge)edgeIndex;
            if (tileProp.GetEdge(tileEdge) == null)
                continue;

            string edgeSample = tileProp.GetEdge(tileEdge).GetString();

            AddToLookup(GetDictionayFromEdge(tileEdge), edgeSample, tileProp);
        }
    }

    public void AddToLookup(EdgeLookupType edgeLookup, string edgeSample, TileProperties tileProp)
    {
        if (!edgeLookup.ContainsKey(edgeSample))
        {
            edgeLookup.Add(edgeSample, new List<TileProperties>());
        }

        edgeLookup[edgeSample].Add(tileProp);
    }

    public Sprite FindValidTile(List<TileEdge> edgeConstraints)
    {
        List<TileProperties> tileList = null;
        foreach (TileEdge edge in edgeConstraints)
        {
            if (edge == null)
                continue;

            EdgeLookupType edgeLookup = GetDictionayFromEdge(edge.GetETileEdge());
            if (edgeLookup.ContainsKey(edge.GetString()))
                tileList = edgeLookup[edge.GetString()];
            break;
        }

        if (tileList == null)
            return null;

        foreach (TileProperties tile in tileList)
        {
            return tile.GetSprite();
        }

        return null;
    }

    EdgeLookupType GetDictionayFromEdge(ETileEdge tileEdge)
    {
        switch (tileEdge)
        {
            case ETileEdge.TOP:
                return edgeTopLookup;
            case ETileEdge.RIGHT:
                return edgeTopLookup;
            case ETileEdge.BOTTOM:
                return edgeTopLookup;
            case ETileEdge.LEFT:
                return edgeTopLookup;
            default:
                return null;
                break;
        }
    }
}
