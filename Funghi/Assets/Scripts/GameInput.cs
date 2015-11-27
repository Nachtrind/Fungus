using UnityEngine;
using System;
using Pathfinding;
using System.Collections.Generic;

public class GameInput: MonoBehaviour
{
    float lastRequest;
    float requestInterval = 0.1f;

    static event Action<Vector3> OnCoreCommand;
    static event Action<Vector3> OnSpawnFungusCommand;

    void Update()
    {
        if (!Camera.main) {
            Debug.LogError("No Camera tagged as MainCamera");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
            return;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldMousePos.y = 0;
            FungusNode nodeNearCursor = GameWorld.Instance.GetNearestFungusNode(worldMousePos); //TODO: nearest node is not always shortest path (compare multiple paths)
            if (nodeNearCursor)
            {
                if (Time.time - lastRequest >= requestInterval)
                {
                    AstarPath.StartPath(ABPath.Construct(nodeNearCursor.transform.position, worldMousePos, OnPathCompleted));
                    lastRequest = Time.time;
                }
                Color c = pathToCursorLength > GameWorld.nodeConnectionDistance ? Color.red : Color.green;
                for (int i = 1; i < pathToCursor.Count; i++)
                {
                    Debug.DrawLine(pathToCursor[i], pathToCursor[i - 1], c);
                }
            }
            else
            {
                pathToCursor.Clear();
                pathToCursorLength = float.PositiveInfinity;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (pathToCursor.Count == 0) { return; }
            if (pathToCursorLength <= GameWorld.nodeConnectionDistance)
            {
                if (OnSpawnFungusCommand != null)
                {
                    OnSpawnFungusCommand(pathToCursor[pathToCursor.Count - 1]);
                }
            }
            pathToCursor.Clear();
        }
        if (Input.GetMouseButtonUp(1))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldMousePos.y = 0;
            if (OnCoreCommand != null)
            {
                OnCoreCommand(worldMousePos);
            }
        }
    }

    public static void RegisterCoreMoveCallback(Action<Vector3> callback)
    {
        OnCoreCommand += callback;
    }

    public static void ReleaseCoreMoveCallback(Action<Vector3> callback)
    {
        OnCoreCommand -= callback;
    }

    public static void RegisterSpawnFungusCallback(Action<Vector3> callback)
    {
        OnSpawnFungusCommand += callback;
    }

    public static void ReleaseSpawnFungusCallback(Action<Vector3> callback)
    {
        OnSpawnFungusCommand -= callback;
    }

    List<Vector3> pathToCursor = new List<Vector3>();
    float pathToCursorLength = 0;

    void OnPathCompleted(Path p)
    {
        if (p.error) { return; }
        pathToCursor = p.vectorPath;
        pathToCursorLength = Mathf.Lerp(Vector3.Distance(pathToCursor[0], pathToCursor[pathToCursor.Count - 1]), p.GetTotalLength(), 0.5f);
    }
}
