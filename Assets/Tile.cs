using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    GameObject go = null;
    TileMapChunk parent = null;
    Vector2Int tilePosition = Vector2Int.zero;
    Vector2Int tileSize = Vector2Int.zero;

    TileSprite tileSprite = null;

    public Tile(TileMapChunk parent, Vector2Int tilePosition, Vector2Int tileSize)
    {
        this.parent = parent;
        this.tilePosition = tilePosition;
        this.tileSize = tileSize;
    }

    public Vector2Int GetTileIndex()
    {
        return tilePosition;
    }

    public Vector2Int GetTilePosition()
    {
        if (parent == null)
            return Vector2Int.zero;

        return parent.GetGridPosFromTileIndex(GetTileIndex());
    }

    public Vector3 GetPosition()
    {
        if (parent == null)
            return Vector3.zero;

        return parent.GetTilePos(tilePosition);
    }


    public void CreateTile(TileSprite tileSprite)
    {
        if (go != null)
            return;

        Vector3 positionOffset = TileManager.TileToWorld(tileSize) / 2;
        Vector3 position = GetPosition() + positionOffset;
        go = (GameObject)Object.Instantiate(Resources.Load("Tile"));
        go.name = "Tile [" + GetTilePosition().x + "][" + GetTilePosition().y + "]";
        if (parent != null)
        {
            go.transform.SetParent(parent.GetTilesObject().transform);
        }

        go.transform.position = position;

        SetSprite(tileSprite);
    }

    public void SetSprite(TileSprite tileSprite)
    {
        this.tileSprite = tileSprite;
        this.tileSprite.SetupGameObject(go);
    }

    public Tile GetNeighbor(ETileEdge eDirection)
    {
        Vector2Int tilePos = GetTilePosition();
        Vector2Int neighborPos = tilePos + eDirection.GetVector2Int();

        if (parent != null && parent.HasTile(neighborPos))
        {
            return parent.GetTileFromGridPos(neighborPos);
        }
        else
        {
            return TileManager.GetTile(neighborPos);
        }
    }

    public List<TileEdge> GetTileNeighborEdges()
    {
        List<TileEdge> tileEdges = new List<TileEdge>();
        for (int tileEdge = 0; tileEdge < (int)ETileEdge.COUNT; ++tileEdge)
        {
            ETileEdge eTileEdge = (ETileEdge)tileEdge;
            ETileEdge eOppositeEdge = eTileEdge.Opposite();

            Tile neighbor = GetNeighbor(eTileEdge);
            if (neighbor != null)
            {
                TileEdge neighborEdge = neighbor.GetTileEdge(eOppositeEdge);
                if (neighborEdge != null)
                {
                    tileEdges.Add(neighborEdge);
                }
            }

        }

        return tileEdges;
    }

    public TileEdge GetTileEdge(ETileEdge eEdge)
    {
        if (tileSprite == null)
            return null;

        return tileSprite.GetEdge(eEdge);
    }
}
