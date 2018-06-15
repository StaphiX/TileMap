using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasRenderer : MonoBehaviour {

    public SpriteAtlas spriteAtlas;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        AddAtlas();
    }

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    void AddAtlas()
    {
        if (spriteAtlas == null)
            return;

        Sprite[] spritesArray = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(spritesArray);

        if (spriteRenderer != null)
            spriteRenderer.sprite = spriteAtlas.GetSprite("Tile1");

        foreach (Sprite sprite in spritesArray)
        {
            AddTile(sprite);
        }
    }

    void AddTile(Sprite tile)
    {
        //Rect sourceRect = tile.sourceRect();

        //Vector2Int topLeft = new Vector2Int((int)sourceRect.x, (int)sourceRect.y);
        //Vector2Int topRight = new Vector2Int((int)sourceRect.xMax, (int)sourceRect.y);
        //Vector2Int bottomLeft = new Vector2Int((int)sourceRect.x, (int)sourceRect.yMax);
        //Vector2Int bottomRight = new Vector2Int((int)sourceRect.xMax, (int)sourceRect.yMax);

        //string sTL = Convert.ToBase64String(tile.GetXBytes(topLeft.x, topLeft.y, 1));
        //string sTR = Convert.ToBase64String(tile.GetXBytes(topRight.x, topRight.y, 1));
        //string sBL = Convert.ToBase64String(tile.GetXBytes(bottomLeft.x, bottomLeft.y, 1));
        //string sBR = Convert.ToBase64String(tile.GetXBytes(bottomRight.x, bottomRight.y, 1));

        Debug.Log(tile.name);

        TileEdge tTopEdge = new TileEdge(tile, ETileEdge.TOP);
        TileEdge tRightEdge = new TileEdge(tile, ETileEdge.RIGHT);
        TileEdge tBottomEdge = new TileEdge(tile, ETileEdge.BOTTOM);
        TileEdge tLeftEdge = new TileEdge(tile, ETileEdge.LEFT);

        //Debug.Log(tTopEdge.GetString() + " " + tRightEdge.GetString() + " " + tBottomEdge.GetString() + " " + tLeftEdge.GetString());
    }

    public Sprite SetSprite(string sprite)
    {
        if (spriteAtlas == null || spriteRenderer == null)
            return null;

        return spriteRenderer.sprite = spriteAtlas.GetSprite(sprite);
    }
}
