﻿using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SlimeHandler))]
public class GameWorld : MonoBehaviour
{
    #region Global
    public const int slimeTag = 0x1;
    public const float TickInterval = 0.1f;

    public const float nodeConnectionDistance = 2f;

    static GameWorld instance;
    public static GameWorld Instance
    {
        get
        {
            if (instance == null) { instance = FindObjectOfType<GameWorld>(); }
            return instance;
        }
    }
    #endregion

    List<FungusNode> nodes = new List<FungusNode>();
    List<Enemy> enemies = new List<Enemy>();
    FungusCore core;
    public FungusCore Core { get { return core; } }

    public bool destroyDisconnectedNodes = true;

    [SerializeField]
    SlimeHandler slimeHandler;

    [SerializeField]
    Transform entityHolder;

    [Header("Prefabs")]
    [SerializeField]
    FungusNode fungusNodePrefab;

    [Header("Masks")]
    public LayerMask ObstacleLayer;

    void Awake()
    {
        instance = this;
        slimeHandler = GetComponent<SlimeHandler>();
    }

    void Start()
    {
        SpawnNodeAndCenter();
        GameInput.RegisterSpawnFungusCallback(SpawnFungusNode);
    }

    bool gameShuttingDown = false;
    void OnApplicationQuit()
    {
        gameShuttingDown = true;
    }

    void Update()
    {
        slimeHandler.UpdateSlimeConnections();
    }

    void SpawnNodeAndCenter()
    {
        FungusStart start = FindObjectOfType<FungusStart>();
        if (!start)
        {
            Debug.LogError("No Startpoint defined!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
            return;
        }
        start.Spawn();
    }

    public void SpawnFungusNode(Vector3 position)
    {
        Instantiate(fungusNodePrefab, position, Quaternion.identity);
    }

    #region Registry

    public void Register(Entity e)
    {
        FungusNode fn = e as FungusNode;
        if (fn)
        {
            if (!nodes.Contains(fn))
            {
                nodes.Add(fn);
                fn.transform.parent = entityHolder;
            }
            return;
        }
        Enemy en = e as Enemy;
        if (en)
        {
            if (!enemies.Contains(en))
            {
                enemies.Add(en);
                en.transform.parent = entityHolder;
            }
            return;
        }
        FungusCore fc = e as FungusCore;
        if (fc)
        {
            if (core != null)
            {
                if (core == fc) { return; }
                Debug.LogError("Multiple cores in Level!");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
#endif
            }
            else
            {
                core = fc;
                fc.transform.parent = entityHolder;
            }
        }
    }

    public void Unregister(Entity e)
    {
        FungusNode fn = e as FungusNode;
        if (fn) { nodes.Remove(fn); }
        Enemy en = e as Enemy;
        if (en) { enemies.Remove(en); }
        FungusCore fc = e as FungusCore;
        if (fc) { if (core == fc) { core = null; } }
    }
    #endregion

    #region Queries
    public List<Enemy> GetEnemies(Vector3 position, float radius)
    {
        List<Enemy> rangeQuery = new List<Enemy>();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.SqrMagnitude(enemies[i].transform.position - position) <= radius * radius)
            {
                rangeQuery.Add(enemies[i]);
            }
        }
        return rangeQuery;
    }

    public Enemy GetNearestEnemy(Vector3 position)
    {
        if (enemies.Count == 0) { return null; }
        Enemy nearest = enemies[0];
        float dist = Vector3.SqrMagnitude(nearest.transform.position - position);
        for (int i = 1; i < enemies.Count; i++)
        {
            float curDist = Vector3.SqrMagnitude(enemies[i].transform.position - position);
            if (curDist < dist)
            {
                nearest = enemies[i];
                dist = curDist;
            }
        }
        return nearest;
    }

    public List<FungusNode> GetFungusNodes(Vector3 position, float radius)
    {
        List<FungusNode> rangeQuery = new List<FungusNode>();
        for (int i = 0; i < nodes.Count; i++)
        {
            if (Vector3.SqrMagnitude(nodes[i].transform.position - position) <= radius * radius)
            {
                rangeQuery.Add(nodes[i]);
            }
        }
        return rangeQuery;
    }

    public FungusNode GetNearestFungusNode(Vector3 position)
    {
        if (nodes.Count == 0) { return null; }
        FungusNode nearest = nodes[0];
        float dist = Vector3.SqrMagnitude(nearest.transform.position - position);
        for (int i = 1; i < nodes.Count; i++)
        {
            float curDist = Vector3.SqrMagnitude(nodes[i].transform.position - position);
            if (curDist < dist)
            {
                nearest = nodes[i];
                dist = curDist;
            }
        }  
        return nearest;
    }

    public Vector3 GetDirectionToRandomFungusNode(Vector3 sourcePoint)
    {
        FungusNode node = nodes[UnityEngine.Random.Range(0, nodes.Count - 1)];
        return (node.transform.position - sourcePoint).normalized;
    }

    #endregion

    #region Slime

    public void OnConnectionInitiated(FungusNode a, FungusNode b, List<Vector3> path)
    {
        slimeHandler.AddConnection(new SlimePath(a, b, path));
    }

    public void SetPositionIsSlime(Vector3 point, float size, bool state)
    {
        if (gameShuttingDown) { return; }
        Pathfinding.GraphUpdateObject guo = new Pathfinding.GraphUpdateObject(new Bounds(point, Vector3.one * size));
        guo.modifyTag = true;
        guo.setTag = state ? slimeTag : 0;
        AstarPath.active.UpdateGraphs(guo);
    }
    #endregion

    #region EventHandler
    public void OnNodeWasDisconnected(FungusNode node)
    {
        if (destroyDisconnectedNodes)
        {
            Destroy(node.gameObject);
        }
    }

    public void OnFungusNodeWasDestroyed(FungusNode node)
    {
        Destroy(node.gameObject);
    }

    public void OnEnemyWasKilled(Enemy enemy)
    {
        //TODO handle resources
        Destroy(enemy.gameObject);
    }

    public void OnCoreLostGrounding(FungusCore core)
    {
        Debug.Log("Core ungrounded");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    public void OnCoreWasKilled(FungusCore core)
    {
        Debug.Log("Core killed");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }
    #endregion
}
