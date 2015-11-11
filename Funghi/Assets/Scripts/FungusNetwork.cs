using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FungusNetwork : MonoBehaviour
{
    //parameters
    public float maxGrowthRadius;
    public float maxGrowthSteps;
    public float growthTick;

    //Network of Fungi
    public List<Fungus> fungi;
    public List<FunNode> nodes;

    public FunCenter center { get; set; }

    List<List<Vector3>> currentGrowthPaths;

    //Prefabs
    public GameObject preNode;
    public GameObject preSlime;

    //AStar
    AStar aStar;

    //Timer
    float growthTimer;

    private static FungusNetwork instance;

    public static FungusNetwork Instance
    {
        get { return instance ?? (instance = new GameObject("FungusNetwork").AddComponent<FungusNetwork>()); }
    }


    // Use this for initialization
    void Awake()
    {
        instance = this;
        aStar = new AStar();
        currentGrowthPaths = new List<List<Vector3>>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGrowthPaths.Count > 0 && growthTimer >= growthTick)
        {
            foreach (List<Vector3> growthPath in currentGrowthPaths)
            {
                if (growthPath.Count > 0)
                {
                    CreateNewSlime(growthPath[0]);
                    growthPath.RemoveAt(0);
                    growthTimer = 0.0f;
                }
            }
        }
        else
        {
            growthTimer += Time.deltaTime;
        }
    }

    //creates a new Slime on a given position
    private void CreateNewSlime(Vector3 _position)
    {
        GameObject funNode = Instantiate(preSlime, _position, transform.rotation) as GameObject;
        WorldGrid.Instance.TileFromWorldPoint(_position).state = 3;
        funNode.transform.position = funNode.transform.position + new Vector3(0, 0, 0.2f);

    }

    //creates a new node (if possible); checks for all nodes in the radius of _position, if they need to grow slime
    public void CreateNewNode(Vector3 _position)
    {

        GameObject funNode = Instantiate(preNode, _position, transform.rotation) as GameObject;

        Tile funT = WorldGrid.Instance.TileFromWorldPoint(_position);

        bool inAnyRadius = false;
        foreach (FunNode n in nodes)
        {
            if (NewNodeInRadius(n, funT.worldPosition))
            {
                List<Vector3> path = aStar.FindPath(WorldGrid.Instance.TileFromWorldPoint(n.worldPos), funT);
                if (path.Count <= maxGrowthSteps)
                {
                    currentGrowthPaths.Add(path);
                    inAnyRadius = true;
                }
            }
        }

        if (inAnyRadius)
        {
            nodes.Add(funNode.GetComponent<FunNode>());
            funNode.transform.position = funT.worldPosition;
            funNode.GetComponent<FunNode>().worldPos = funT.worldPosition;
            funT.state = 2;
        }
        else
        {
            Destroy(funNode);
        }

    }

    //checks if the position of the new node is in the radius of a given node
    private bool NewNodeInRadius(FunNode _oldNode, Vector3 _newNode)
    {
        if (Vector3.Distance(_oldNode.worldPos, _newNode) > maxGrowthRadius)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}
