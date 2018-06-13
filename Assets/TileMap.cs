using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap
{
    Vector2Int vGridCenter = Vector2Int.zero;

    List<TileMapChunkRow> mapRows = new List<TileMapChunkRow>();

    Vector2Int vTileSize = new Vector2Int(64, 64);
    Vector2Int vChunkSize = new Vector2Int(10, 10);
    int chunkBuffer = 1;

    public int GetBuffer() { return chunkBuffer; }
    public void SetBuffer(int buffer) { chunkBuffer = buffer; }

    public TileMap(int tileW, int tileH, int chunkCountX, int chunkCountY)
    {
        vTileSize = new Vector2Int(tileW, tileH);
        vChunkSize = new Vector2Int(chunkCountX, chunkCountY);
    }

    public void Init()
    {
        SetupChunks();
    }

    public Vector2 GetChunkSize() { return vChunkSize * vTileSize; }

    public void AddChunk(TileMapChunk tChunk, int iCol, int iRow, int iRowSize)
    {
        if(mapRows[iRow] == null)
        {
            mapRows[iRow] = new TileMapChunkRow(iRowSize);
        }

        mapRows[iRow].Add(tChunk, iCol);
    }

    public void SetupChunks()
    {
        int chunkW = vChunkSize.x * vTileSize.x;
        int chunkH = vChunkSize.x * vTileSize.x;

        int colCount = Mathf.Max(Screen.width / chunkH + chunkBuffer*2, 1);
        int rowCount = Mathf.Max(Screen.height / chunkH + chunkBuffer*2, 1);

        int startPosX = -(vChunkSize.x * colCount) / 2 + vGridCenter.x;
        int startPosY = -(vChunkSize.y * rowCount) / 2 + vGridCenter.y;

        for (int row = 0; row < rowCount; ++row)
        {
            mapRows.Add(null);
            for (int col = 0; col < colCount; ++col)
            {
                int fChunkX = vChunkSize.x * col;
                int fChunkY = vChunkSize.y * row;
                Vector2Int vMapPos = new Vector2Int(fChunkX + startPosX, fChunkY + startPosY);
                TileMapChunk chunk = new TileMapChunk(vMapPos, vChunkSize, vTileSize);
                chunk.Init();

                AddChunk(chunk, col, row, colCount);
            }
        }
    }

    public bool GetTilePos(Vector2 vScreenPos, out Vector3 vTilePos)
    {
        vTilePos = Vector3.zero;
        foreach (TileMapChunkRow row in mapRows)
        {
            if (row == null)
                continue;

            if (row.GetTilePos(vScreenPos, out vTilePos))
            {
                return true;
            }
        }

        return false;
    }

    public MapObject GetObjectFromScreen(Vector2 vScreenPos)
    {
        foreach (TileMapChunkRow row in mapRows)
        {
            if (row == null)
                continue;

            MapObject mapObject = row.GetObject(vScreenPos);
            if (mapObject != null)
                return mapObject;
        }

        return null;
    }
}
