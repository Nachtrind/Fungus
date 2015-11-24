using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGrid : MonoBehaviour
{

    public float field_SizeX;
    public float field_SizeY;
    public float tile_Size;
    public int grid_SizeX;
    public int grid_SizeY;
    public Tile[,] grid;

    #region Editable Grid
    [SerializeField]
    List<Tile> tGrid = new List<Tile>();

    public bool GetTile(int x, int y, out Tile tile)
    {
        int index = x + grid_SizeX * y;
        if (index < 0 || index >= tGrid.Count) { tile = null; return false; }
        tile = tGrid[index];
        return true;
    }

    public List<Tile> GetNeighbors(Tile tile, int distance)
    {
        List<Tile> neighbors = new List<Tile>();
        int minX = Mathf.Max(0, tile.x - distance);
        int maxX = Mathf.Min(tile.x + distance, tGrid.Count - 1);
        int miny = Mathf.Max(0, tile.y - distance);
        int maxY = Mathf.Min(tile.y + distance, tGrid.Count - 1);
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = miny; y <= maxY; y++)
            {
                Tile t;
                if (GetTile(x, y, out t) && t != tile)
                {
                    neighbors.Add(t);
                }
            }
        }
        return neighbors;
    }

    public void GenerateGrid(int sizeX, int sizeY)
    {
        tGrid = new List<Tile>(sizeX * sizeY);
        Vector3 worldBottomLeft = transform.position - Vector3.right * field_SizeX / 2 - Vector3.up * field_SizeY / 2;
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * tile_Size + tile_Size / 2) + Vector3.up * (y * tile_Size + tile_Size / 2) + Vector3.forward * 6;
                Tile t = new Tile(worldPoint, TileStates.Free, x, y, null, null);
            }
        }
    }
    #endregion

    //Masks
    public LayerMask obstacle;
    public LayerMask node;
    public LayerMask slime;
    public LayerMask human;
    public LayerMask center;


    private static WorldGrid instance;

    public static WorldGrid Instance
    {
        get { return instance ?? (instance = new GameObject("WorldGrid").AddComponent<WorldGrid>()); }
    }


    // Use this for initialization
    void Awake()
    {
        grid_SizeX = Mathf.RoundToInt(field_SizeX / tile_Size);
        grid_SizeY = Mathf.RoundToInt(field_SizeY / tile_Size);
        instance = this;
        CreateGrid();

    }

    public void CreateGrid()
    {
        grid = new Tile[grid_SizeX, grid_SizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * field_SizeX / 2 - Vector3.up * field_SizeY / 2;

        //fill grid
        for (int x = 0; x < grid_SizeX; x++)
        {
            for (int y = 0; y < grid_SizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * tile_Size + tile_Size / 2) + Vector3.up * (y * tile_Size + tile_Size / 2) + Vector3.forward * 6;
                FunNode funode = null;
                //set tiles
                //0 = free, 1 = obstacle, 2 = funghi, 3 = fun, 4 = human
                TileStates state = TileStates.Free;
                Fungus funi = null;
                if (Physics.CheckSphere(worldPoint, tile_Size / 5, obstacle))
                {
                    state = TileStates.Obstacle;
                }

                if (Physics.CheckSphere(worldPoint, tile_Size / 5, slime))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(worldPoint, tile_Size / 5, slime);
                    hitColliders[0].gameObject.transform.position = worldPoint;
                    state = TileStates.Slime;
                    funi = hitColliders[0].GetComponent<FunSlime>();
                }


                if (Physics.CheckSphere(worldPoint, tile_Size / 5, node))
                {

                    Collider[] hitColliders = Physics.OverlapSphere(worldPoint, tile_Size / 5, node);
                    hitColliders[0].gameObject.transform.position = worldPoint;
                    funode = hitColliders[0].GetComponent<FunNode>();
                    funode.worldPos = worldPoint;
                    funi = funode;
                    state = TileStates.Node;
                }

                /*
                if (Physics.CheckSphere(worldPoint, tile_Size / 5, human))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(worldPoint, tile_Size / 5, human);
                    hitColliders[0].gameObject.transform.position = worldPoint;
                    state = 4;
                }*/

                if (Physics.CheckSphere(worldPoint, tile_Size / 5, center))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(worldPoint, tile_Size / 5, center);
                    hitColliders[0].gameObject.transform.position = worldPoint - new Vector3(0, 0, 0.03f);
                    FungusNetwork.Instance.center = hitColliders[0].GetComponent<FunCenter>();
                }

               
                Tile t = new Tile(worldPoint, state, x, y, funi, funode);
                grid[x, y] = t;
            }
        }
    }

    public Tile TileFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + field_SizeX / 2) / field_SizeX;
        float percentY = (worldPosition.y + field_SizeY / 2) / field_SizeY;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((grid_SizeX - 1) * percentX);
        int y = Mathf.RoundToInt((grid_SizeY - 1) * percentY);

        return grid[x, y];
    }


    void DrawGridPreview()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Vector3 worldBottomLeft = transform.position - Vector3.right * field_SizeX / 2 - Vector3.up * field_SizeY / 2;
        Vector3 cubeSize = new Vector3(tile_Size * 0.9f, tile_Size * 0.9f, 0.1f);
        int g_grid_SizeX = Mathf.RoundToInt(field_SizeX / tile_Size);
        int g_grid_SizeY = Mathf.RoundToInt(field_SizeY / tile_Size);
        for (int x = 0; x < g_grid_SizeX; x++)
        {
            for (int y = 0; y < g_grid_SizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * tile_Size + tile_Size / 2) + Vector3.up * (y * tile_Size + tile_Size / 2) + Vector3.forward * 6;
                Gizmos.DrawCube(worldPoint, cubeSize);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            DrawGridPreview();
            return;
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(field_SizeX, field_SizeY, 1));


        if (grid == null)
        {
            return;
        }
        return;
        foreach (Tile t in grid)
        {
            if (t.state == TileStates.Free)
            {
                Gizmos.color = Color.white;
            }

            if (t.state == TileStates.Obstacle)
            {
                Gizmos.color = Color.red;
            }

            if (t.state == TileStates.Node)
            {
                Gizmos.color = Color.green;
            }

            if (t.state == TileStates.Slime)
            {
                Gizmos.color = Color.yellow;
            }

            if (t.state == TileStates.Human)
            {
                Gizmos.color = Color.black;
            }

            if (t.state == TileStates.Food)
            {
                Gizmos.color = Color.magenta;
            }


            Gizmos.DrawCube(t.worldPosition, Vector3.one * (tile_Size - tile_Size * 0.1f));
        }

    }


}
