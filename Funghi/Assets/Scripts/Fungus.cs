using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Fungus : MonoBehaviour
{
    //List<Fungus> neighbours;


    public List<Fungus> GetNeighbours()
    {
        List<Fungus> neighbourList = new List<Fungus>();

        Tile currTile = WorldGrid.Instance.TileFromWorldPoint(this.transform.position);
        int y = currTile.y;
        int x = currTile.x;

        //1 -> x, y + 1
        if (y + 1 < WorldGrid.Instance.grid_SizeY &&
            (WorldGrid.Instance.grid[x, y + 1].state == 3 || WorldGrid.Instance.grid[x, y + 1].state == 2))
        {
            neighbourList.Add(WorldGrid.Instance.grid[x, y + 1].fun);
        }


        //3 -> x - 1, y
        if (x - 1 >= 0 &&
            (WorldGrid.Instance.grid[x, y + 1].state == 3 || WorldGrid.Instance.grid[x, y + 1].state == 2))
        {
            neighbourList.Add(WorldGrid.Instance.grid[x - 1, y].fun);
        }

        //4 -> x + 1, y
        if (x + 1 < WorldGrid.Instance.grid_SizeX &&
             (WorldGrid.Instance.grid[x, y + 1].state == 3 || WorldGrid.Instance.grid[x, y + 1].state == 2))
        {
            neighbourList.Add(WorldGrid.Instance.grid[x + 1, y].fun);
        }

        //6 -> x, y - 1
        if (y - 1 >= 0 &&
             (WorldGrid.Instance.grid[x, y + 1].state == 2 || WorldGrid.Instance.grid[x, y + 1].state == 3))
        {
            neighbourList.Add(WorldGrid.Instance.grid[x, y - 1].fun);
        }

        return neighbourList;
    }


}
