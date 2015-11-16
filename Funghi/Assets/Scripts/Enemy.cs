using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour
{
    //parameters
    public float speed;
    public float attackRadius;
    public float callRadius;
    public float damage;

    //movement
    bool onTheMove = false;
    List<Tile> currentPath;


    Tile currentTarget;
    Vector3 lastPosition;

    //Timer
    float attackTick = 0.8f;
    float attackTimer;

    float startTime;
    float dis;

    public LayerMask enemyLayer;

    AStar star = new AStar();

    // Use this for initialization
    void Start()
    {
        CalculatePathToCenter();
    }

    // Update is called once per frame
    void Update()
    {
        if (onTheMove)
        {
            Move();
        }
        else
        {
            if (CenterInRange())
            {
                if (attackTimer >= attackTick)
                {
                    Debug.Log("Attacked Center!");
                    FunCenter.Instance.Attacked(damage);
                    attackTimer = 0.0f;
                }
            }
            else
            {
                if (!CalculatePathToCenter())
                {
                    //CalculatePathToNearestNode();
                }

            }
        }

        //Timer
        attackTimer += Time.deltaTime;

    }

    private bool CalculatePathToNearestNode()
    {
        int shortestCount = int.MaxValue;
        List<Tile> shortestPath = null;
        FunNode nearestNode = null;

        foreach (FunNode node in FungusNetwork.Instance.nodes)
        {
            List<Tile> path = star.FindPath(WorldGrid.Instance.TileFromWorldPoint(transform.position), WorldGrid.Instance.TileFromWorldPoint(node.worldPos), new List<int> { 0, 2 });
            if (path != null)
            {
                if (path.Count < shortestCount)
                {
                    shortestPath = path;
                    shortestCount = path.Count;
                    nearestNode = node;
                }
            }
            else
            {
                Debug.Log("Path is Null");
            }
        }

        if (shortestPath == null)
        {
            Debug.Log("No Path found to node :(");
            return false;
        }
        else
        {
            shortestPath.RemoveAt(shortestPath.Count - 1);
            currentPath = shortestPath;
            return true;
        }


    }

    public bool CalculatePathToCenter()
    {

        List<Tile> adjacendToCenter = WorldGrid.Instance.TileFromWorldPoint(FunCenter.Instance.transform.position).GetNeighboursWithDiagonals();
        foreach (Tile t in adjacendToCenter)
        {
            if (MoveToNewTile(t))
            {
                return true;
            }
        }

        return false;
    }


    public void GotAttacked()
    {

    }

    private bool CenterInRange()
    {
        Tile centerTile = WorldGrid.Instance.TileFromWorldPoint(FunCenter.Instance.transform.position);
        Tile currTile = WorldGrid.Instance.TileFromWorldPoint(transform.position);

        List<Tile> neighbours = currTile.GetNeighboursWithDiagonals();

        foreach (Tile t in neighbours)
        {
            if (t == centerTile)
            {
                return true;
            }
        }

        return false;

    }

    public void Move()
    {
        if (currentTarget.state == 0)
        {
            float disCovered = (Time.time - startTime) * speed;
            float fracJourney = disCovered / dis;

            transform.position = Vector3.Lerp(lastPosition, currentTarget.worldPosition, fracJourney);

            if (fracJourney >= 1.0f)
            {

                lastPosition = currentTarget.worldPosition;
                startTime = Time.time;
                if (currentPath.Count <= 0)
                {
                    onTheMove = false;
                }
                else
                {
                    startTime = Time.time;
                    currentTarget = currentPath[0];
                    currentPath.RemoveAt(0);
                    dis = Vector3.Distance(currentTarget.worldPosition, lastPosition);
                }
            }
        }
        else
        {
            onTheMove = false;
        }
    }

    public bool MoveToNewTile(Tile _target)
    {
        currentPath = star.FindPath(WorldGrid.Instance.TileFromWorldPoint(this.transform.position), _target, new List<int> { 0 });

        if (currentPath != null && currentPath.Count > 0)
        {
            Debug.Log("FoundPath");
            onTheMove = true;
            currentTarget = currentPath[0];
            currentPath.RemoveAt(0);
            lastPosition = this.transform.position;
            dis = Vector3.Distance(currentTarget.worldPosition, lastPosition);
            startTime = Time.time;
            return true;
        }
        return false;
    }


    //Get other Enemys in radius
    private List<Enemy> GetFriendsInRadius()
    {
        List<Enemy> friends = new List<Enemy>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, callRadius, enemyLayer);

        foreach (Collider co in colliders)
        {
            friends.Add(co.GetComponent<Enemy>());
        }

        return friends;
    }

    //alarms Enemy of an agressor in the region
    public void Alarm(Tile _agressor)
    {

    }

}
