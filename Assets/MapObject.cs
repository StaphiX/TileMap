using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject
{
    GameObject go = null;
    TileMapChunk parent = null;
    Vector2Int tilePosition = Vector2Int.zero;
    Vector2Int tileSize = Vector2Int.zero;

    public MapObject(TileMapChunk parent, Vector2Int tilePosition, Vector2Int tileSize)
    {
        this.parent = parent;
        this.tilePosition = tilePosition;
        this.tileSize = tileSize;
    }

    public Vector2Int GetTilePosition()
    {
        return tilePosition;
    }

    public Vector3 GetPosition()
    {
        if (parent == null)
            return Vector3.zero;

        return parent.GetTilePos(tilePosition);
    }


    public void CreateTile()
    {
        if (go != null)
            return;

        Vector3 positionOffset = TileManager.TileToWorld(tileSize)/2;
        Vector3 position = GetPosition() + positionOffset;
        go = (GameObject)Object.Instantiate(Resources.Load("Tile"));
        go.transform.position = position;
    }
}
