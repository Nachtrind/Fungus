using UnityEngine;
using System;
using Pathfinding;
using System.Collections.Generic;

public class GameInput: MonoBehaviour
{
	float lastRequest;
	float requestInterval = 0.1f;
	private float inputTimer;
	private float inputTick = 0.1f;

	static event Action<Vector3> OnCoreCommand;
	static event Func<Vector3, bool> OnSpawnFungusCommand;

	void Update ()
	{
		if (!Camera.main) {
			Debug.LogError ("No Camera tagged as MainCamera");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
			return;
		}
		//Left Mouse Click
		if (Input.GetMouseButton (0) && inputTimer > inputTick) {
			Vector3 worldMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (worldMousePos, 0.20f);
			if (nodesInRadius.Count <= 0) {
				CreateNewSlimePath (worldMousePos);
			} else {
				nodesInRadius [0].ToggleActive ();
			}
			inputTimer = 0.0f;
		}

		if (Input.GetMouseButtonUp (0)) {
			SpawnNewSlimePath ();
			inputTimer = 0.0f;
		}

		if (Input.GetMouseButtonUp (1) && inputTimer > inputTick) {
			Vector3 worldMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			worldMousePos.y = 0;
			if (OnCoreCommand != null) {
				OnCoreCommand (worldMousePos);
			}
			inputTimer = 0.0f;
		}

		inputTimer += Time.deltaTime;
	}

	public static void RegisterCoreMoveCallback (Action<Vector3> callback)
	{
		OnCoreCommand += callback;
	}

	public static void ReleaseCoreMoveCallback (Action<Vector3> callback)
	{
		OnCoreCommand -= callback;
	}

	public static void RegisterSpawnFungusCallback (Func<Vector3, bool> callback)
	{
		OnSpawnFungusCommand += callback;
	}

	public static void ReleaseSpawnFungusCallback (Func<Vector3, bool> callback)
	{
		OnSpawnFungusCommand -= callback;
	}

	List<Vector3> pathToCursor = new List<Vector3> ();
	float pathToCursorLength = 0;

	void OnPathCompleted (Path p)
	{
		if (p.error) {
			return;
		}
		pathToCursor = p.vectorPath;
		pathToCursorLength = Mathf.Lerp (Vector3.Distance (pathToCursor [0], pathToCursor [pathToCursor.Count - 1]), p.GetTotalLength (), 0.5f);
	}

	private void SpawnNewSlimePath ()
	{
		if (pathToCursor.Count == 0) {
			return;
		}
		if (pathToCursorLength <= GameWorld.nodeConnectionDistance) {
			if (OnSpawnFungusCommand != null) {
				OnSpawnFungusCommand (pathToCursor [pathToCursor.Count - 1]);
			}
		}
		pathToCursor.Clear ();
	}

	private void CreateNewSlimePath (Vector3 _mousePosition)
	{
		_mousePosition.y = 0;
		FungusNode nodeNearCursor = GameWorld.Instance.GetNearestFungusNode (_mousePosition); //HACK: nearest node is not always shortest path (if needed, compare multiple paths)
		if (nodeNearCursor) {
			if (Time.time - lastRequest >= requestInterval) {
				AstarPath.StartPath (ABPath.Construct (nodeNearCursor.transform.position, _mousePosition, OnPathCompleted));
				lastRequest = Time.time;
			}
			Color c = pathToCursorLength > GameWorld.nodeConnectionDistance ? Color.red : Color.green;
			for (int i = 1; i < pathToCursor.Count; i++) {
				Debug.DrawLine (pathToCursor [i], pathToCursor [i - 1], c);
			}
		} else {
			pathToCursor.Clear ();
			pathToCursorLength = float.PositiveInfinity;
		}
	}


}
