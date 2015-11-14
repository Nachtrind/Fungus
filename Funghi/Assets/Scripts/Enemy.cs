using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    bool pathToCenter;

    Tile currentTarget;
    Vector3 lastPosition;

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
            CalculatePathToCenter();
        }
    }

    public void CalculatePathToCenter()
    {

        List<Tile> adjacendToCenter = WorldGrid.Instance.TileFromWorldPoint(FunCenter.Instance.transform.position).GetNeighboursWithDiagonals();
        foreach (Tile t in adjacendToCenter)
        {
            if (MoveToNewTile(t))
            {
                pathToCenter = true;
                return;
            }
        }
    }


    public void GotAttacked()
    {

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
