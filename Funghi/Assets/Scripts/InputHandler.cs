



using UnityEngine;
using System;
using System.Collections;
using ButtonName;
using Pathfinding;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
	static InputHandler instance;
	AbilityButton currentSelection;
	float lastRequest;
	float requestInterval = 0.1f;
	private float inputTimer;
	private float inputTick = 0.1f;
	
	static event Action<Vector3> OnCoreCommand;
	static event Func<Vector3, FungusNode> OnSpawnFungusCommand;
	
	//Image Tint Colors
	Color normalTint = new Color (1f, 1f, 1f, 1f);
	Color selectedTint = new Color (110 / 255f, 143 / 255f, 67 / 255f, 1f);
#pragma warning disable 0414
    Color lockedTint = new Color (90 / 255f, 90 / 255f, 90 / 255f, 1f);
#pragma warning restore 0414

    public static InputHandler Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<InputHandler> ();
			}
			return instance;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		//Touch Input for Tablet
		if (Input.touchCount > 0) {
			if (Input.GetTouch (0).phase == TouchPhase.Began || Input.GetTouch (0).phase == TouchPhase.Moved) {
				Vector3 worldMousePos = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).deltaPosition);
				List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (worldMousePos, 0.20f);
				if (nodesInRadius.Count <= 0) {
					CreateNewSlimePath (worldMousePos);
				} else {
					nodesInRadius [0].ToggleActive ();
				}
			}
			
			if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				SpawnNewSlimePath ();
			}
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
			Debug.Log ("Mouse Up!");
			SpawnNewSlimePath ();
			inputTimer = 0.0f;
		}
		
		inputTimer += Time.deltaTime;
	}
	
	public void ClickedButton (AbilityButton _button)
	{
		if (_button == currentSelection) {
			if (_button.isSelected) {
				this.currentSelection = null;
				_button.isSelected = false;
				_button.SetTint (normalTint);
			}
		} else {
			if (currentSelection != null) {
				currentSelection.isSelected = false;
				currentSelection.SetTint (normalTint);
			}
			
			currentSelection = _button;
			_button.isSelected = true;
			_button.SetTint (selectedTint);
			
		}
	}
	
	public static void RegisterCoreMoveCallback (Action<Vector3> callback)
	{
		OnCoreCommand += callback;
	}
	
	public static void ReleaseCoreMoveCallback (Action<Vector3> callback)
	{
		OnCoreCommand -= callback;
	}
	
	public static void RegisterSpawnFungusCallback (Func<Vector3, FungusNode> callback)
	{
		OnSpawnFungusCommand += callback;
	}
	
	public static void ReleaseSpawnFungusCallback (Func<Vector3, FungusNode> callback)
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

