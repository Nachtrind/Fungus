using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TileStates
{
    Free = 0,
    Obstacle = 1,
    Node = 2,
    Slime = 3,
    Human = 4,
    Food = 5,
    FungusCenter = 6
}

[Serializable]
public class Tile : IHeapItem<Tile>
{
    public Vector3 worldPosition;
    public TileStates state;
    //public int state; //0 = free, 1 = obstacle, 2 = node, 3 = slime, 4 = human, 5 = food, 6 = fungus center
    public int x;
    public int y;
    [NonSerialized]
    int gCost;
    [NonSerialized]
    int hCost;
    [NonSerialized]
    int heapIndex;
    [NonSerialized]
    public Fungus fun;
    public FunNode funNode { get; set; }
    public FunSlime slime { get; set; }
    [NonSerialized]
    Tile parent;

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public Tile(Vector3 _pos, TileStates _state, int _x, int _y, Fungus _fun, FunNode _node)
    {
        this.worldPosition = _pos;
        this.state = _state;
        this.x = _x;
        this.y = _y;
        this.fun = _fun;
        funNode = _node;
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public int GCost
    {
        get
        {
            return gCost;
        }
        set
        {
            gCost = value;
        }
    }

    public int HCost
    {
        get
        {
            return hCost;
        }
        set
        {
            hCost = value;
        }
    }

    public int FCost()
    {
        return (gCost + hCost);
    }

    public int CompareTo(Tile _compTile)
    {
        int compare = FCost().CompareTo(_compTile.FCost());
        if (compare == 0)
        {
            compare = hCost.CompareTo(_compTile.HCost);

        }
        return -compare;
    }

    public List<Tile> GetNeighbours()
    {
        List<Tile> neighbours = new List<Tile>();

        //1 -> x, y + 1
        if (y + 1 < WorldGrid.Instance.grid_SizeY)
        {
            neighbours.Add(WorldGrid.Instance.grid[x, y + 1]);
        }


        //3 -> x - 1, y
        if (x - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x - 1, y]);
        }

        //4 -> x + 1, y
        if (x + 1 < WorldGrid.Instance.grid_SizeX)
        {
            neighbours.Add(WorldGrid.Instance.grid[x + 1, y]);
        }

        //6 -> x, y - 1
        if (y - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x, y - 1]);
        }

        //diagonals

        /*
        //7 -> x + 1, y - 1
        if (x + 1 < WorldGrid.Instance.grid_SizeX &&
            y - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x + 1, y - 1]);
        }


        //5 -> x - 1, y - 1
        if (x - 1 >= 0 &&
            y - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x - 1, y - 1]);
        }


        //2 -> x + 1, y + 1
        if (x + 1 < WorldGrid.Instance.grid_SizeX &&
            y + 1 < WorldGrid.Instance.grid_SizeY)
        {
            neighbours.Add(WorldGrid.Instance.grid[x + 1, y + 1]);

        }


        //0 -> x - 1, y + 1
        if (x - 1 >= 0 &&
            y + 1 < WorldGrid.Instance.grid_SizeY)
        {
            neighbours.Add(WorldGrid.Instance.grid[x - 1, y + 1]);
        }
        */
        return neighbours;
    }

    public List<Tile> GetNeighboursWithDiagonals()
    {
        List<Tile> neighbours = new List<Tile>();

        //1 -> x, y + 1
        if (y + 1 < WorldGrid.Instance.grid_SizeY)
        {
            neighbours.Add(WorldGrid.Instance.grid[x, y + 1]);
        }


        //3 -> x - 1, y
        if (x - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x - 1, y]);
        }

        //4 -> x + 1, y
        if (x + 1 < WorldGrid.Instance.grid_SizeX)
        {
            neighbours.Add(WorldGrid.Instance.grid[x + 1, y]);
        }

        //6 -> x, y - 1
        if (y - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x, y - 1]);
        }

        //diagonals


        //7 -> x + 1, y - 1
        if (x + 1 < WorldGrid.Instance.grid_SizeX &&
            y - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x + 1, y - 1]);
        }


        //5 -> x - 1, y - 1
        if (x - 1 >= 0 &&
            y - 1 >= 0)
        {
            neighbours.Add(WorldGrid.Instance.grid[x - 1, y - 1]);
        }


        //2 -> x + 1, y + 1
        if (x + 1 < WorldGrid.Instance.grid_SizeX &&
            y + 1 < WorldGrid.Instance.grid_SizeY)
        {
            neighbours.Add(WorldGrid.Instance.grid[x + 1, y + 1]);

        }


        //0 -> x - 1, y + 1
        if (x - 1 >= 0 &&
            y + 1 < WorldGrid.Instance.grid_SizeY)
        {
            neighbours.Add(WorldGrid.Instance.grid[x - 1, y + 1]);
        }

        return neighbours;
    }


    public Tile Parent
    {
        get
        {
            return parent;
        }

        set
        {
            parent = value;
        }
    }

    //public Vector3 WorldPosition 
    //	{ get; set; }

    //public int State 
    //	{ get; set; }

}

