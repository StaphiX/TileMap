using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using EdgeLookupType = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<TileSprite>>;

public class BrushManager
{
    List<SpriteAtlas> atlas = null;

    EdgeLookupType edgeTopLookup = new EdgeLookupType();
    EdgeLookupType edgeRightLookup = new EdgeLookupType();
    EdgeLookupType edgeBottomLookup = new EdgeLookupType();
    EdgeLookupType edgeLeftLookup = new EdgeLookupType();

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
        if (atlas.spriteCount <= 0)
            return;

        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);
        for (int sprite = 0; sprite < atlas.spriteCount; ++sprite)
        {
            RegisterSprite(sprites[sprite]);
        }
    }

    void RegisterSprite(Sprite sprite)
    {
        TileSpriteProperties tileSpriteProperties = new TileSpriteProperties(sprite);
        List<TileSprite> tTileSprites = tileSpriteProperties.CreateTileSprites();
        RegisterTileEdges(tTileSprites);
    }

    public void RegisterTileEdges(List<TileSprite> tileSprites)
    {
        foreach (TileSprite tileSprite in tileSprites)
        {
            for (int edgeIndex = 0; edgeIndex < (int)ETileEdge.COUNT; ++edgeIndex)
            {
                ETileEdge tileEdge = (ETileEdge)edgeIndex;
                if (tileSprite.GetEdge(tileEdge) == null)
                    continue;

                string edgeSample = tileSprite.GetEdge(tileEdge).GetHash();

                AddToLookup(GetDictionayFromEdge(tileEdge), edgeSample, tileSprite);
            }
        }
    }

    public void AddToLookup(EdgeLookupType edgeLookup, string edgeSample, TileSprite tileProp)
    {
        if (!edgeLookup.ContainsKey(edgeSample))
        {
            edgeLookup.Add(edgeSample, new List<TileSprite>());
        }

        edgeLookup[edgeSample].Add(tileProp);
    }

    public List<TileSprite> FindValidTiles(List<TileEdge> neighborEdges)
    {
        // Find a list of tiles from the edgeLookup
        // This finds tiles based on a sample of the tile edge
        // This allows us to quickly narrow down a possible set of tiles to pull from
        // We then compare all the edges to do a full comparison of tile edges
        // This gives us a list of all suitable tiles for a space

        bool bHasConstraints = false;
        List<TileSprite> tileList = null;
        foreach (TileEdge edge in neighborEdges)
        {
            if (edge == null)
                continue;

            string edgeHash = edge.GetHash();
            ETileEdge eTileEdge = edge.GetETileEdge();
            ETileEdge eConstraintEdge = edge.GetETileEdge().Opposite();

            if (tileList == null)
            {
                bHasConstraints = true;

                EdgeLookupType edgeLookup = GetDictionayFromEdge(eConstraintEdge);
                if (edgeLookup.ContainsKey(edge.GetHash()))
                {
                    tileList = new List<TileSprite>(edgeLookup[edge.GetHash()]);
                    continue;
                }
            }
            else
            {
                for(int tileIndex = tileList.Count-1; tileIndex >= 0; --tileIndex)
                {
                    TileSprite tileListSprite = tileList[tileIndex];
                    TileEdge tileListEdge = tileListSprite.GetEdge(eConstraintEdge);
                    string tileSpriteEdgeHash = tileListEdge.GetHash();

                    if (!tileSpriteEdgeHash.Equals(edgeHash))
                        tileList.RemoveAt(tileIndex);
                }
            }
        }

        if (tileList == null)
        {
            // We can use any tile
            if(!bHasConstraints)
            {
                tileList = new List<TileSprite>();
                foreach(List<TileSprite> lookupList in edgeTopLookup.Values)
                {
                    foreach(TileSprite tileSprite in lookupList)
                    {
                        tileList.Add(tileSprite);
                    }
                }
            }
        }

        return tileList;

        // Now do a full comparison of tile edges to make sure it is suitable
        /*
        foreach (TileSprite tile in tileList)
        {
            return tile.GetEdge();
        }*/

        return null;
    }

    EdgeLookupType GetDictionayFromEdge(ETileEdge tileEdge)
    {
        switch (tileEdge)
        {
            case ETileEdge.TOP:
                return edgeTopLookup;
            case ETileEdge.RIGHT:
                return edgeRightLookup;
            case ETileEdge.BOTTOM:
                return edgeBottomLookup;
            case ETileEdge.LEFT:
                return edgeLeftLookup;
            default:
                return null;
        }
    }
}
