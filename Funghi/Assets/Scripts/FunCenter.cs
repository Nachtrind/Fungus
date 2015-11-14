using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FunCenter : MonoBehaviour
{
    public float speed;


    bool onTheMove = false;
    List<Tile> currentPath;

    Tile currentTarget;
    Vector3 lastPosition;

    float startTime;
    float dis;

    AStar star = new AStar();


    private static FunCenter instance;

    public static FunCenter Instance
    {
        get { return instance ?? (instance = new GameObject("FunCenter").AddComponent<FunCenter>()); }
    }


    // Use this for initialization
    void Start()
    {
        instance = this;
        lastPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (onTheMove)
        {
            MoveCenter();

        }
    }

    public void MoveCenter()
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


    public void MoveToNewTile(Tile _target)
    {
        currentPath = star.FindPath(WorldGrid.Instance.TileFromWorldPoint(this.transform.position), _target, new List<int> { 2, 3 });

        if (currentPath != null && currentPath.Count > 0)
        {
            onTheMove = true;
            currentTarget = currentPath[0];
            currentPath.RemoveAt(0);
            lastPosition = this.transform.position;
            dis = Vector3.Distance(currentTarget.worldPosition, lastPosition);
        }

    }

    /*
    void OnMouseDrag()
    {
        float distance = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }*/

}
