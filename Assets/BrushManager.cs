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

    public List<TileSprite> FindValidTiles(List<TileSpriteNeighbor> neighborTiles)
    {
        // Find a list of tiles from the edgeLookup
        // This finds tiles based on a sample of the tile edge
        // This allows us to quickly narrow down a possible set of tiles to pull from
        // We then compare all the edges to do a full comparison of tile edges
        // This gives us a list of all suitable tiles for a space

        bool bHasConstraints = false;
        List<TileSprite> candidateList = null;
        foreach (TileSpriteNeighbor neighbor in neighborTiles)
        {
            if (neighbor == null)
                continue;

            TileEdge neighborEdge = neighbor.GetEdge();
            if (neighborEdge == null)
                continue;

            TileSprite neighborTileSprite = neighbor.GetTileSprite();
            string neighborEdgeHash = neighborEdge.GetHash();
            ETileEdge eNeighborEdge = neighborEdge.GetETileEdge();
            ETileEdge eCandidateEdge = eNeighborEdge.Opposite();

            if (candidateList == null)
            {
                bHasConstraints = true;

                EdgeLookupType edgeLookup = GetDictionayFromEdge(eCandidateEdge);
                if (edgeLookup.ContainsKey(neighborEdgeHash))
                {
                    candidateList = new List<TileSprite>(edgeLookup[neighborEdgeHash]);
                }
            }
            else
            {
                // Remove candidates based on edge hash
                for (int tileIndex = candidateList.Count - 1; tileIndex >= 0; --tileIndex)
                {
                    TileSprite candidateTileSprite = candidateList[tileIndex];
                    TileEdge candidateTileEdge = candidateTileSprite.GetEdge(eCandidateEdge);
                    string candidateTileEdgeHash = candidateTileEdge.GetHash();

                    if (!candidateTileEdgeHash.Equals(neighborEdgeHash))
                        candidateList.RemoveAt(tileIndex);
                }
            }

            // Remove candidates based on full edge comparison
            for (int tileIndex = candidateList.Count - 1; tileIndex >= 0; --tileIndex)
            {
                TileSprite candidateTileSprite = candidateList[tileIndex];

                if (!candidateTileSprite.CompareEdge(neighborTileSprite, eCandidateEdge))
                    candidateList.RemoveAt(tileIndex);
            }
        }

        if (candidateList == null)
        {
            // We can use any tile
            if (!bHasConstraints)
            {
                candidateList = new List<TileSprite>();
                foreach (List<TileSprite> lookupList in edgeTopLookup.Values)
                {
                    foreach (TileSprite tileSprite in lookupList)
                    {
                        candidateList.Add(tileSprite);
                    }
                }
            }
        }

        return candidateList;
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
