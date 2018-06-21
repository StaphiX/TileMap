using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public static Vector2Int TILESIZE = new Vector2Int(64, 64);
    public static Vector2Int CHUNKCOUNT = new Vector2Int(10, 10);
    public static Vector2 GRIDCENTER = Vector2.zero;
    public static int PIXELSPERUNIT = 64;
    public static int PIXELSCALE = 1;

    private BrushManager brushManager = new BrushManager();

    public static TileMap tTileMap = null;

    // TESTING VARS
    private bool FOUNDTILE = false;
    private Vector3 TILEPOS = Vector3.zero;

    void Awake()
    {
        SetupCamera();
        InitialiseMap();
        brushManager.Init();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        InputUpdate();

    }

    void InputUpdate()
    {
        if (InputManager.IsDragging())
        {
            Vector2 vDrag = -InputManager.GetDrag();
            Debug.Log("Cam Drag: " + vDrag);
            Vector3 vCameraOffset = Camera.main.ScreenToViewportPoint(vDrag);
            vCameraOffset *= 10;
            Camera.main.transform.Translate(vCameraOffset, Space.World);
        }
        else if (Input.GetMouseButton(0))
        {
            //Vector3 vTilePos = Vector3.zero;
            //bool bFoundTile = tTileMap.GetTilePos(Input.mousePosition, out vTilePos);

            Tile tile =  tTileMap.GetTileFromScreen(Input.mousePosition);

            if(tile != null)
            {
                List<TileEdge> edgeConstraints = tile.GetTileEdgeConstraints();
                List<TileSprite> tileSprites = brushManager.FindValidTiles(edgeConstraints);

                if(tileSprites != null && tileSprites.Count > 0)
                {
                    int iTileIndex = Random.Range(0, tileSprites.Count);
                    tile.CreateTile(tileSprites[iTileIndex]);
                }
            }

            //TILEPOS = vTilePos;
            //FOUNDTILE = bFoundTile;

            //if(bFoundTile)
            //{
            //
            //}
        }
    }

    void SetupCamera()
    {
        Camera.main.orthographicSize = ((float)Screen.height / ((float)PIXELSCALE * (float)PIXELSPERUNIT)) * 0.5f;
    }

    void InitialiseMap()
    {
        tTileMap = new TileMap(64, 64, 10, 10);
        tTileMap.Init();
    }

    void SpawnTile(Vector3 vPosition)
    {
        GameObject tile = (GameObject)Instantiate(Resources.Load("Tile"));
        tile.transform.position = vPosition;
        //if(tBaseMap != null)
            //tile.transform.SetParent(tBaseMap.transform);
    }

    void OnDrawGizmos()
    {
        if (FOUNDTILE)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(TILEPOS, 1);
        }
    }

    public static Vector2 GetChunkSize() { return CHUNKCOUNT * TILESIZE; }
    public static Vector2 GetGridCenter() { return GRIDCENTER; }

    public static float ScreenToWorldX(float fX)
    {
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float worldX = fX / Screen.width * (float)worldScreenWidth;

        return worldX;
    }

    public static float ScreenToWorldXScale(float fX)
    {
        return ScreenToWorldX(fX) * PIXELSPERUNIT;
    }

    public static float ScreenToWorldY(float fY)
    {
        double worldScreenHeight = Camera.main.orthographicSize * 2.0;
        double worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float worldY = fY / Screen.height * (float)worldScreenHeight;

        return worldY;
    }

    public static float ScreenToWorldYScale(float fY)
    {
        return ScreenToWorldX(fY) * PIXELSPERUNIT;
    }

    public static Vector2 ScreenToGrid(Vector2 vScreenPos)
    {
        Vector2 vWorldPos = Camera.main.ScreenToWorldPoint(vScreenPos);
        return WorldToGrid(vWorldPos);
    }

    public static Vector2 TileToWorld(Vector2 vTileSize)
    {
        return GridToWorld(vTileSize) / TILESIZE;
    }

    public static Vector2 GridToWorld(Vector2 vGridPos)
    {
        Vector2 vChunkOffset = ((Vector2)CHUNKCOUNT) / 2;
        Vector2 vGridCenter = GetGridCenter();

        Vector2 vGridOffset = vGridPos - vGridCenter;
        Vector2 vWorldOffset = (vGridOffset * TILESIZE) / PIXELSPERUNIT;

        return vWorldOffset;
    }

    public static Vector2 WorldToGrid(Vector2 vWorldPos)
    {
        Vector2 vChunkOffset = ((Vector2)CHUNKCOUNT) / 2;
        Vector2 vGridCenter = GetGridCenter();

        Vector2 vGridOffset = (vWorldPos * PIXELSPERUNIT) / TILESIZE;
        Vector2 vGridPos = vGridOffset + vGridCenter;

        return vGridPos;
    }

    public static Tile GetTile(Vector2Int vGridPos)
    {
        return tTileMap.GetTile(vGridPos);
    }
}
