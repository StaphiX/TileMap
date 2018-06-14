using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject
{
    GameObject go = null;
    TileMapChunk parent = null;
    Vector2Int tilePosition = Vector2Int.zero;
    Vector2Int tileSize = Vector2Int.zero;
    SpriteAtlasRenderer spriteAtlasRenderer = null;

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
        spriteAtlasRenderer = go.GetComponent<SpriteAtlasRenderer>();

        switch((int)(Random.value * 4))
        {
            case 0:
                SetSprite("Tile1");
                break;
            case 1:
                SetSprite("Tile2");
                break;
            case 2:
                SetSprite("Tile3");
                break;
            case 3:
                SetSprite("Tile4");
                break;
            default:
                SetSprite("Tile1");
                break;
        }

        go.name = "Tile [" + GetTilePosition().x + "][" + GetTilePosition().y + "]";

        if (parent != null)
        {
            go.transform.SetParent(parent.GetTilesObject().transform);
        }

        go.transform.position = position;
    }

    public void SetSprite(string sprite)
    {
        if (spriteAtlasRenderer == null)
            return;

        spriteAtlasRenderer.SetSprite(sprite);
    }
}
