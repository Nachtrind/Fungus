using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FunCenter : MonoBehaviour
{
    public float speed;

    public float maxHealth;
    public float currHealth { get; set; }

    public bool selected { get; set; }

    public bool onTheMove { get; set; }
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
    void Awake()
    {
        instance = this;
        lastPosition = this.transform.position;
        currHealth = maxHealth;
        selected = false;
        onTheMove = false;
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
        if (currentTarget.state == TileStates.Node || currentTarget.state == TileStates.Slime)
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


    public void MoveToNewTile(Tile _target)
    {
        currentPath = star.FindPath(WorldGrid.Instance.TileFromWorldPoint(this.transform.position), _target, new List<TileStates> { TileStates.Node, TileStates.Slime });

        if (currentPath != null && currentPath.Count > 0)
        {
            onTheMove = true;
            currentTarget = currentPath[0];
            currentPath.RemoveAt(0);
            lastPosition = this.transform.position;
            dis = Vector3.Distance(currentTarget.worldPosition, lastPosition);
        }

    }

    public void Damage(float _damage)
    {
        currHealth -= _damage;
        Debug.Log(currHealth);
        if (currHealth <= 0)
        {
            //TODO: Game Over Stuff
            Debug.Log("Oh no, it's dead!");
        }

    }


    /*
    void OnMouseDrag()
    {
        float distance = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
    }*/

}
