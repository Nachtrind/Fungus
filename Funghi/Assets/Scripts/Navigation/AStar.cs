using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar
{


    public List<Tile> FindPath(Tile _start, Tile _end)
    {

        Heap<Tile> openSet = new Heap<Tile>(WorldGrid.Instance.grid_SizeX * WorldGrid.Instance.grid_SizeY);
        HashSet<Tile> closedSet = new HashSet<Tile>();
        List<Tile> path = new List<Tile>();

        openSet.Add(_start);

        int stepCount = 0;
        while (openSet.Count > 0)
        {

            //Failsafe so unity doesn't break
            stepCount++;
            if (stepCount > 10000)
            {
                return null;
            }

            Tile current = openSet.RemoveFirst();
            closedSet.Add(current);

            //End is found
            if (current == _end)
            {
                path = this.Backtracking(_start, _end);
                return path;
            }

            foreach (Tile neigh in current.GetNeighbours())
            {
                if (neigh.state == 1 || closedSet.Contains(neigh))
                {
                    continue;
                }

                int newMovementCostToNeighbour = current.GCost + ApproximateDistance(current.worldPosition, neigh.worldPosition);
                if (newMovementCostToNeighbour < neigh.GCost || !openSet.Contains(neigh))
                {
                    neigh.GCost = newMovementCostToNeighbour;
                    neigh.HCost = ApproximateDistance(neigh.worldPosition, _end.worldPosition);
                    neigh.Parent = current;



                    if (!openSet.Contains(neigh))
                    {
                        openSet.Add(neigh);
                        openSet.UpdateItem(neigh);
                    }
                }
            }//foreach


        }//END while


        return null;

    }

    private List<Tile> Backtracking(Tile _start, Tile _end)
    {

        List<Tile> path = new List<Tile>();
        Tile current = _end;

        while (current != _start)
        {
            if (current == null)
            {
                Debug.Log("Oh no the current!!");
                return path;
            }
            path.Insert(0, current);
            current = current.Parent;
        }

        return path;


    }

    private int ApproximateDistance(Vector3 _start, Vector3 _end)
    {

        int distance = Mathf.RoundToInt(Mathf.Abs(_end.x - _start.x) + Mathf.Abs(_end.y - _start.y) + Mathf.Abs(_end.z - _start.z));
        return distance;
    }


}