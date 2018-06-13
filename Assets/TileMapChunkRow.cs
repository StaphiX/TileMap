using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapChunkRow
{
    int length = 0;
    List<TileMapChunk> mapChunks = new List<TileMapChunk>();

    public TileMapChunkRow(int _length)
    {
        length = _length;

        for(int iChunk = 0; iChunk < length; ++iChunk)
        {
            mapChunks.Add(null);
        }
    }

    public void Add(TileMapChunk chunk, int iCol)
    {
        if (mapChunks[iCol] != null)
        {
            Delete(mapChunks[iCol]);
        }

        mapChunks[iCol] = chunk;
    }

    public void Delete(TileMapChunk chunk)
    {

    }

    public bool GetTilePos(Vector2 vScreenPos, out Vector3 vTilePos)
    {
        vTilePos = Vector3.zero;

        Vector2 vTileGridPos = TileManager.ScreenToGrid(vScreenPos);

        foreach (TileMapChunk chunk in mapChunks)
        {
            if (chunk == null)
                continue;

            if (chunk.HasTile(vTileGridPos))
            {
                Vector2Int vTileIndex = chunk.GetTileIndexFromGridPos(vTileGridPos);
                vTilePos = chunk.GetTilePos(vTileIndex);
                return true;
            }
        }
        return false;
    }

    public MapObject GetObject(Vector2 vScreenPos)
    {
        Vector2 vTileGridPos = TileManager.ScreenToGrid(vScreenPos);
        foreach (TileMapChunk chunk in mapChunks)
        {
            if (chunk == null)
                continue;

            if (chunk.HasTile(vTileGridPos))
            {
                Vector2Int vTileIndex = chunk.GetTileIndexFromGridPos(vTileGridPos);
                return chunk.GetTile(vTileIndex);
            }
        }

        return null;
    }
}
