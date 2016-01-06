using UnityEngine;
using System;
using Pathfinding;
using ButtonName;
using System.Collections.Generic;
using NodeAbilities;

public class GameInput: MonoBehaviour
{
	public ParticleSystem spores;
	float lastRequest;
	float requestInterval = 0.1f;
	private float inputTimer;
	private float inputTick = 0.2f;
	float coreInputRange = 0.1f;

	static event Action<Vector3> OnCoreCommand;
	static event Func<Vector3, FungusNode> OnSpawnFungusCommand;

	static InputHandler instance;
	AbilityButton currentSelection;
	public NodeAbility beatneat;
	public NodeAbility attract;
	public NodeAbility slowdown;
	public NodeAbility speedup;
	public NodeAbility zombies;
	public NodeAbility growth;
	bool dragCore;
	
	//Image Tint Colors
	Color normalTint = new Color (1f, 1f, 1f, 1f);
	Color selectedTint = new Color (110 / 255f, 143 / 255f, 67 / 255f, 1f);
	Color lockedTint = new Color (90 / 255f, 90 / 255f, 90 / 255f, 1f);
	private Plane plane;
	private Camera cam;

	//Stuff for camera movement &  zoom
	public float moveSpeedX = 0.20f;
	public float moveSpeedZ = 0.20f;
	private Vector2 scrollDirection = Vector2.zero;

	void Start ()
	{
		plane = new Plane (Vector3.up, Vector3.zero);
		cam = Camera.main;

	}

	void Update ()
	{
		if (!cam) {
			Debug.LogError ("No Camera tagged as MainCamera");
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = true;
#endif
			return;
		}


		if (dragCore) {
			////////////////
			//drag stopped//
			////////////////
			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {

				Vector3 worldMousePos;
				//take mouse position or touch position
				if (Input.GetMouseButtonUp (0)) {
					worldMousePos = cam.ScreenToWorldPoint (Input.mousePosition);
				} else {
					worldMousePos = cam.ScreenToWorldPoint (Input.GetTouch (0).deltaPosition);
				}
				worldMousePos.y = 0;


				if (OnCoreCommand != null) {
					OnCoreCommand (worldMousePos);
				}

				inputTimer = 0.0f;
				dragCore = false;
			} else if (Input.touchCount > 0 || Input.GetMouseButton (0)) {
				///////////////////////
				//dragging core along//
				///////////////////////
				Vector3 worldMousePos;
				//take mouse position or touch position
				if (Input.GetMouseButton (0)) {
					worldMousePos = cam.ScreenToWorldPoint (Input.mousePosition);
				} else {
					worldMousePos = cam.ScreenToWorldPoint (Input.GetTouch (0).deltaPosition);
				}
				worldMousePos.y = 0;

				if (OnCoreCommand != null) {
					OnCoreCommand (worldMousePos);
				}
			}
		} 



		if (currentSelection != null && inputTimer > inputTick && !dragCore) {
			if (Input.touchCount > 0 || Input.GetMouseButton (0)) {

				//////////////
				// NEW NODE //
				//////////////
				if (currentSelection.buttonName == ButtonName.ButtonName.NewNode) {
					Vector3 worldMousePos;
					//take mouse position or touch position
					if (Input.GetMouseButton (0)) {
						worldMousePos = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
					} else {
						worldMousePos = GetTouchPosInWorld (cam.ScreenPointToRay (Input.GetTouch (0).deltaPosition));
					}
					List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (worldMousePos, 0.4f);

					if (!spores.isPlaying || spores.enableEmission == false) {
						spores.Play ();
						spores.enableEmission = true;
						Debug.Log ("Activating Particles");
					}
					spores.transform.position = new Vector3 (worldMousePos.x, 0.5f, worldMousePos.z);

					if (nodesInRadius.Count <= 0) {
						CreateNewSlimePath (worldMousePos);
					}
				}

				/////////////////////
				// Change Ability  //
				/////////////////////
				if (currentSelection.buttonName != null && currentSelection.buttonName != ButtonName.ButtonName.NewNode) {

					Vector3 worldMousePos;
					//take mouse position or touch position
					if (Input.GetMouseButton (0)) {
						worldMousePos = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
					} else {
						worldMousePos = GetTouchPosInWorld (cam.ScreenPointToRay (Input.GetTouch (0).deltaPosition));
					}

					List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (worldMousePos, 0.40f);
					if (nodesInRadius.Count > 0) {
						SpecializeNode (nodesInRadius [0]);
						DeactivateSelection ();
					}
				}


			}

			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
				Debug.Log ("Deactivate Particles");
				SpawnNewSlimePath ();
				DeactivateSelection ();
				spores.enableEmission = false;
				Debug.Log ("After Stopping Spores: " + spores.isPlaying);
			}
			
			
		}

		if (currentSelection == null && inputTimer > inputTick && !dragCore) {
			if (Input.GetMouseButton (0) || Input.touchCount > 0) {

				Vector3 worldMousePos;
				//take mouse position or touch position
				if (Input.GetMouseButton (0)) {
					worldMousePos = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
				} else {
					worldMousePos = GetTouchPosInWorld (cam.ScreenPointToRay (Input.GetTouch (0).deltaPosition));
				}


				/////////////////////
				//Checking for Core//
				/////////////////////
				FungusCore core = GameWorld.Instance.CoreInRange (worldMousePos, coreInputRange);

				if (core != null) {
					dragCore = true;

				} else {

					////////////////////
					//Activating Nodes//
					////////////////////
					List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (worldMousePos, 0.40f);

					if (nodesInRadius.Count > 0) {
						nodesInRadius [0].ToggleActive ();
						inputTimer = 0.0f;
					} else {

						Touch[] touches = Input.touches;
						
						///////////////
						//Move Camera//
						///////////////
						if (touches.Length == 1) {
							if (touches [0].phase == TouchPhase.Began) {

							} else if (touches [0].phase == TouchPhase.Moved) {
								Vector2 touchMovement = touches [0].deltaPosition;
								
								float posX = touchMovement.x * -moveSpeedX * Time.deltaTime;
								
								float posZ = touchMovement.y * -moveSpeedZ * Time.deltaTime;
								
								cam.transform.position += new Vector3 (posX, 0, posZ);
								


							} 

						}

						///////////////
						//Zoom Camera//
						///////////////
						if (touches.Length == 2) {

							Vector2 cameraViewsize = new Vector2 (cam.pixelWidth, cam.pixelHeight);
							
							Touch touchOne = touches [0];
							Touch touchTwo = touches [1];
							
							Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
							Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;
							
							float prevTouchLength = (touchOnePrevPos - touchTwoPrevPos).magnitude;
							float touchDeltaLength = (touchOne.position - touchTwo.position).magnitude;
							
							float lengthDiff = prevTouchLength - touchDeltaLength;

							//TODO: Finish Zoom

						}
					}
				}
			}
		}
		


		inputTimer += Time.deltaTime;

	}

	public Vector3 GetTouchPosInWorld (Ray _ray)
	{

		float enter;
		if (plane.Raycast (_ray, out enter)) {
			Vector3 point = _ray.GetPoint (enter);
			return point;

		}

		return new Vector3 (0, 0, 0);

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
		inputTimer = 0.0f;	
	}

	void DeactivateSelection ()
	{
		inputTimer = 0.0f;
		currentSelection.SetTint (normalTint);
		currentSelection.isSelected = false;
		currentSelection = null;
	}

	void SpecializeNode (FungusNode fungusNode)
	{

		switch (currentSelection.buttonName) {
		case ButtonName.ButtonName.BeatNEat:
			{
				fungusNode.Specialize (beatneat);
				break;
			}
		case ButtonName.ButtonName.ScentNode:
			{
				fungusNode.Specialize (attract);
				break;
			}
		case ButtonName.ButtonName.SlowDown:
			{
				fungusNode.Specialize (slowdown);
				break;
			}
		case ButtonName.ButtonName.SpeedUp:
			{
				fungusNode.Specialize (speedup);
				break;
			}
		case ButtonName.ButtonName.Zombies:
			{
				fungusNode.Specialize (zombies);
				break;
			}
		case ButtonName.ButtonName.GrowthSpores:
			{
				fungusNode.Specialize (growth);
				break;
			}
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
		bool reachable = true;
		_mousePosition.y = 0;
		FungusNode nodeNearCursor = GameWorld.Instance.GetNearestFungusNode (_mousePosition); //HACK: nearest node is not always shortest path (if needed, compare multiple paths)
		if (nodeNearCursor) {
			if (Time.time - lastRequest >= requestInterval) {
				AstarPath.StartPath (ABPath.Construct (nodeNearCursor.transform.position, _mousePosition, OnPathCompleted));
				lastRequest = Time.time;
			}
			Color c = pathToCursorLength > GameWorld.nodeConnectionDistance ? Color.red : Color.green;
			//non gizmo indicator
			spores.startColor = pathToCursorLength > GameWorld.nodeConnectionDistance ? new Color (232 / 255f, 82 / 255f, 0 / 255f, 1f) : new Color (167 / 255f, 255 / 255f, 0 / 255f, 1f);
			for (int i = 1; i < pathToCursor.Count; i++) {
				Debug.DrawLine (pathToCursor [i], pathToCursor [i - 1], c);
			}
		} else {
			pathToCursor.Clear ();
			pathToCursorLength = float.PositiveInfinity;
		}

	}


}
