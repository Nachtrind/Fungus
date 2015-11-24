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
    public float movementOffset;

    //movement
    bool onTheMove = false;
    List<Tile> currentPath;
    Vector3 currentOffset;

    //attack
    FunNode nodeToAttack = null;


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
        PathToCenter();
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
                    FunCenter.Instance.Damage(damage);
                    attackTimer = 0.0f;
                }
            }
            else
            {
                if (!PathToCenter())
                {
                    if (NodeInRange())
                    {
                        if (attackTimer >= attackTick)
                        {
                            Debug.Log("Attacked Node!");
                            nodeToAttack.Damage(damage);
                            attackTimer = 0.0f;
                        }
                    }
                    else
                    {
                        CalculatePathToNearestNode();
                    }
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
            List<Tile> path = star.FindPath(WorldGrid.Instance.TileFromWorldPoint(transform.position), WorldGrid.Instance.TileFromWorldPoint(node.worldPos), new List<TileStates> { TileStates.Free, TileStates.Node });
            if (path != null)
            {
                if (path.Count < shortestCount)
                {
                    shortestPath = path;
                    shortestCount = path.Count;
                    nearestNode = node;
                    nodeToAttack = node;
                }
            }
            else
            {

            }
        }

        if (shortestPath == null)
        {
            Debug.Log("No Path found to any node :(");
            nodeToAttack = null;
            return false;
        }
        else
        {
            shortestPath.RemoveAt(shortestPath.Count - 1);
            SetNewPath(shortestPath);
            return true;
        }


    }


    //Try to Calculate a path to the center; return true on success
    public bool PathToCenter()
    {

        List<Tile> adjacendToCenter = WorldGrid.Instance.TileFromWorldPoint(FunCenter.Instance.transform.position).GetNeighboursWithDiagonals();

        List<Tile> shortestPath = null;
        int shortestCount = int.MaxValue;

        foreach (Tile t in adjacendToCenter)
        {
            if (t.state == TileStates.Free)
            {
                List<Tile> currPath = star.FindPath(WorldGrid.Instance.TileFromWorldPoint(this.transform.position), t, new List<TileStates> { TileStates.Free });
                if (currPath != null && currPath.Count <= shortestCount)
                {
                    shortestPath = currPath;
                    shortestCount = shortestPath.Count;
                }
            }
        }

        if (shortestPath != null)
        {
            SetNewPath(shortestPath);
            return true;
        }

        return false;
    }


    public void GotAttacked()
    {
        //TODO: Stuff to alarm others
        Destroy(this.gameObject);
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

    private bool NodeInRange()
    {
        Tile currTile = WorldGrid.Instance.TileFromWorldPoint(transform.position);

        List<Tile> neighbours = currTile.GetNeighboursWithDiagonals();

        foreach (Tile t in neighbours)
        {
            if (t.state == TileStates.Node)
            {
                nodeToAttack = t.funNode;
                return true;
            }
        }
        return false;

    }

    public void Move()
    {
        if (currentTarget.state == TileStates.Free)
        {
            float disCovered = (Time.time - startTime) * speed;
            float fracJourney = disCovered / dis;

            transform.position = Vector3.Lerp(lastPosition, currentTarget.worldPosition + currentOffset, fracJourney);

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
                    currentOffset = new Vector3(UnityEngine.Random.Range(-movementOffset, movementOffset), UnityEngine.Random.Range(-movementOffset, movementOffset), 0.0f);
                    dis = Vector3.Distance(currentTarget.worldPosition + currentOffset, lastPosition);
                }
            }
        }
        else
        {
            onTheMove = false;
        }
    }

    public void SetNewPath(List<Tile> _path)
    {
        currentPath = _path;

        if (currentPath != null && currentPath.Count > 0)
        {
            onTheMove = true;
            currentTarget = currentPath[0];
            currentPath.RemoveAt(0);
            lastPosition = this.transform.position;
            currentOffset = new Vector3(UnityEngine.Random.Range(-movementOffset, movementOffset), UnityEngine.Random.Range(-movementOffset, movementOffset), 0.0f);
            dis = Vector3.Distance(currentTarget.worldPosition + currentOffset, lastPosition);
            startTime = Time.time;
        }
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
