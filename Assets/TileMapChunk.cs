using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapChunk
{
    Vector2Int vMapPosition = Vector2Int.zero;
    Vector2Int vChunkSize = Vector2Int.zero;
    Vector2Int vTileSize = Vector2Int.zero;

    GameObject chunk = null;
    GameObject grid = null;
    Object lineObj = null;

    MapObject[,] tTile = null;

    public Vector2Int GetMapPos()
    {
        return vMapPosition;
    }

    public TileMapChunk(Vector2Int _vMapPosition, Vector2Int _vChunkSize, Vector2Int _vTileSize)
    {
        vMapPosition = _vMapPosition;
        vChunkSize = _vChunkSize;
        vTileSize = _vTileSize;

        tTile = new MapObject[vChunkSize.x, vChunkSize.y];
        for (int col = 0; col < _vChunkSize.x; ++col)
        {
            for (int row = 0; row < _vChunkSize.y; ++row)
            {
                tTile[col,row] = new MapObject(this, new Vector2Int(col, row), vTileSize);
            }
        }
    }

    public float GetTileLeft() { return vMapPosition.x; }
    public float GetTileTop() { return vMapPosition.y; }
    public float GetTileWidth() { return vChunkSize.x; }
    public float GetTileHeight() { return vChunkSize.y; }
    public float GetLeft() { return vMapPosition.x * vTileSize.x; }
    public float GetTop() { return vMapPosition.y * vTileSize.y; }
    public float GetWidth() { return vChunkSize.x * vTileSize.x; }
    public float GetHeight() { return vChunkSize.y * vTileSize.y; }

    public Rect GetTileRect()
    {
        return new Rect(GetTileLeft(), GetTileTop(), GetTileWidth(), GetTileHeight());
    }

    public Rect GetPixelRect()
    {
        return new Rect(GetLeft(), GetTop(), GetWidth(), GetHeight());
    }

    public void Init()
    {
        Create();
        SetupGrid();
    }

    public void Create()
    {
        int iMapX = (vMapPosition.x + TileManager.CHUNKCOUNT.x / 2) / TileManager.CHUNKCOUNT.x;
        int iMapY = (vMapPosition.y + TileManager.CHUNKCOUNT.y / 2) / TileManager.CHUNKCOUNT.y;
        chunk = new GameObject("Chunk[" + iMapX + " - " + iMapY +"]");

        Vector2Int vGridPosition = Vector2Int.FloorToInt(vMapPosition) + new Vector2Int(vChunkSize.x/2, vChunkSize.y/2);
        Vector3 vPosition = TileManager.GridToWorld(vGridPosition);

        chunk.transform.position = vPosition;
    }

    public void SetupGrid()
    {
        if (chunk == null)
            return;

        lineObj = Resources.Load("Line");

        grid = new GameObject("Grid");
        grid.transform.SetParent(chunk.transform);
        grid.transform.localPosition = Vector3.zero;

        float fChunkW = vTileSize.x * vChunkSize.x;
        float fChunkH = vTileSize.y * vChunkSize.y;
        float fScaleX = TileManager.ScreenToWorldXScale(fChunkW);
        float fScaleY = TileManager.ScreenToWorldYScale(fChunkH);

        Object whiteObj = Resources.Load("White");
        GameObject gridSprite = (GameObject)GameObject.Instantiate(whiteObj, chunk.transform);
        gridSprite.name = "GridSprite";
        float fMapCol = (Mathf.Abs(GetTileRect().center.x) * 5 + Mathf.Abs(GetTileRect().center.y * 2)) / 255.0f;
        gridSprite.GetComponent<SpriteRenderer>().color = new Color(fMapCol, fMapCol, fMapCol);
        gridSprite.transform.localPosition = Vector3.zero;
        gridSprite.transform.localScale = new Vector3(fScaleX, fScaleY, 1);
        gridSprite.transform.SetParent(grid.transform);

        Vector3 vCameraOffset = Camera.main.WorldToScreenPoint(Camera.main.transform.position);
        Vector2Int vMapOffset = Vector2Int.FloorToInt(TileManager.GetGridCenter());

        float fTileOffsetX = TileManager.ScreenToWorldX(vTileSize.x);
        float fTileOffsetY = TileManager.ScreenToWorldY(vTileSize.y);

        for (int row = 0; row < vChunkSize.y; ++row)
        {
            float yOffset = row * fTileOffsetY - fTileOffsetY * vChunkSize.y/2;
            CreateLine(0, yOffset, true);
        }

        for (int col = 0; col < vChunkSize.x; ++col)
        {
            float xOffset = col * fTileOffsetX - fTileOffsetX * vChunkSize.x / 2;
            CreateLine(xOffset, 0, false);
        }
    }

    void CreateLine(float xOffset, float yOffset, bool bHorizontal)
    {
        if (grid == null || lineObj == null)
            return;

        float fChunkW = vTileSize.x * vChunkSize.x;
        float fChunkH = vTileSize.y * vChunkSize.y;

        GameObject line = (GameObject)GameObject.Instantiate(lineObj);
        if (bHorizontal)
            line.name = "GL-X" + xOffset;
        else
            line.name = "GL-Y" + yOffset;

        line.transform.SetParent(grid.transform);

        line.transform.localPosition = new Vector3(xOffset, yOffset, 0);
        Vector3 vLocalScale = line.transform.localScale;

        if (bHorizontal)
            vLocalScale.x = TileManager.ScreenToWorldXScale(fChunkW);
        else
            vLocalScale.y = TileManager.ScreenToWorldYScale(fChunkH);

        line.transform.localScale = vLocalScale;
    }

    public bool HasTile(Vector2 vTilePos)
    {
        Rect tTileRect = GetTileRect();

        if (tTileRect.Contains(vTilePos))
            return true;

        return false;
    }

    public Vector3 GetTilePos(Vector2Int vTilePos)
    {
        Vector2 gridPosition = vMapPosition;
        Vector3 worldPos = TileManager.GridToWorld(gridPosition);
        Vector3 tileWorldOffset = TileManager.GridToWorld(vTilePos);

        return worldPos + tileWorldOffset;
    }

    public Vector2Int GetTileIndexFromGridPos(Vector2 vTilePos)
    {
        int iTileIndexX = Mathf.FloorToInt(vTilePos.x) - GetMapPos().x;
        int iTileIndexY = Mathf.FloorToInt(vTilePos.y) - GetMapPos().y;
        Vector2Int vTileIndex = new Vector2Int(iTileIndexX, iTileIndexY);

        return vTileIndex;
    }

    public MapObject GetTile(Vector2Int vTilePos)
    {
        if (vTilePos.x >= vChunkSize.x || vTilePos.y >= vChunkSize.y)
            return null;

        return tTile[vTilePos.x, vTilePos.y];
    }
}
