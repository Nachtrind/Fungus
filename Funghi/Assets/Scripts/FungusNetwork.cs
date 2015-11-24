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
    public List<Fungus> fungi { get; set; }
    public List<FunNode> nodes { get; set; }

    public FunCenter center { get; set; }

    List<List<Tile>> currentGrowthPaths;

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
        currentGrowthPaths = new List<List<Tile>>();

        fungi = new List<Fungus>();
        nodes = new List<FunNode>();

        FunNode[] inSceneNodes = FindObjectsOfType(typeof(FunNode)) as FunNode[];
        nodes.AddRange(inSceneNodes);
        fungi.AddRange(inSceneNodes);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGrowthPaths.Count > 0 && growthTimer >= growthTick)
        {
            foreach (List<Tile> growthPath in currentGrowthPaths)
            {
                if (growthPath.Count > 0)
                {
                    CreateNewSlime(growthPath[0].worldPosition);
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
        Tile slimeTile = WorldGrid.Instance.TileFromWorldPoint(_position);

        if (slimeTile.slime == null)
        {
            GameObject funNode = Instantiate(preSlime, _position, transform.rotation) as GameObject;

            if (slimeTile.state != 2)
            {
                slimeTile.state = 3;
            }
            slimeTile.slime = funNode.GetComponent<FunSlime>();
            slimeTile.slime.usages = 1;
            funNode.transform.position = funNode.transform.position + new Vector3(0, 0, 0.2f);
            fungi.Add(slimeTile.slime);
        }
        else
        {
            slimeTile.slime.usages += 1;
        }
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
                List<Tile> path = aStar.FindPath(WorldGrid.Instance.TileFromWorldPoint(n.worldPos), funT, new List<int> { 0, 2, 3 });
                if (path.Count <= maxGrowthSteps)
                {
                    currentGrowthPaths.Add(path);
                    funNode.GetComponent<FunNode>().SlimePaths.Add(new List<Tile>(path));
                    inAnyRadius = true;
                }
            }
        }

        if (inAnyRadius)
        {
            nodes.Add(funNode.GetComponent<FunNode>());
            fungi.Add(funNode.GetComponent<FunNode>());
            funNode.transform.position = funT.worldPosition;
            funNode.GetComponent<FunNode>().worldPos = funT.worldPosition;
            funT.state = 2;
            funT.funNode = funNode.GetComponent<FunNode>();
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
