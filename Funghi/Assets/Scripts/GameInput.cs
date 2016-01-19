using NodeAbilities;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInput: MonoBehaviour
{

	enum InputState
	{
		SkillMode,
		BuildMode,
		MoveBrainMode,
		NoMode}

	;

	InputState currentState;

	public Renderer ground;


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

	//Stuff for Building a new Node
	private Vector3 touchWorldPoint;
	private bool canBuildNode;

	//Image Tint Colors
	Color normalTint = new Color (1f, 1f, 1f, 1f);
	public Color selectedTint;
	Color lockedTint = new Color (90 / 255f, 90 / 255f, 90 / 255f, 1f);
	private Plane plane;
	private Camera cam;

	//Stuff for camera movement &  zoom
	public float moveSpeedX = 2.0000f;
	public float moveSpeedZ = 2.00f;
	private Vector2 scrollDirection = Vector2.zero;

	float levelSizeY;
	float levelSizeX;

	//Level Borders
	float leftBorder;
	float rightBorder;
	float upperBorder;
	float lowerBorder;

	private GameObject skillMenu;

	void Start ()
	{
		plane = new Plane (Vector3.up, Vector3.zero);
		cam = Camera.main;

		levelSizeY = ground.bounds.extents.y;
		levelSizeX = ground.bounds.extents.x;

		CalcLevelBorders ();

		currentState = InputState.NoMode;

		//Find and Deativate Skill Menu
		skillMenu = GameObject.Find ("SkillMenu");
		skillMenu.SetActive (false);

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

		///////////////
		//Get Touches//
		///////////////
		Touch[] touches = Input.touches;



		if (touches.Length == 1) {

			//Transform Touch to WorldPosition
			touchWorldPoint = GetTouchPosInWorld (cam.ScreenPointToRay (Input.GetTouch (0).position));

			///////////////////
			//Build New Nodes//
			///////////////////
			if (currentState == InputState.BuildMode) {				
				if (touches [0].phase == TouchPhase.Began) {
					List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (touchWorldPoint, 0.4f);
					if (nodesInRadius.Count > 0) {
						if (!spores.isPlaying || spores.emission.enabled == false) {
							ParticleSystem.EmissionModule em = spores.emission;
							spores.Play ();
							em.enabled = true;
							spores.transform.position = new Vector3 (touchWorldPoint.x, 0.5f, touchWorldPoint.z);
						}
						canBuildNode = true;
					}
				}

				if (touches [0].phase == TouchPhase.Moved && canBuildNode) {
					List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (touchWorldPoint, 0.4f);
					spores.transform.position = new Vector3 (touchWorldPoint.x, 0.5f, touchWorldPoint.z);

					if (nodesInRadius.Count <= 0) {
						CreateNewSlimePath (touchWorldPoint);
					}
				}

				if (touches [0].phase == TouchPhase.Ended && canBuildNode) {
					SpawnNewSlimePath ();
					ParticleSystem.EmissionModule em = spores.emission;
					em.enabled = false;
					canBuildNode = false;
				}

			}

			//////////////
			//Move Brain//
			//////////////
			if (currentState == InputState.MoveBrainMode) {	
				if (touches [0].phase == TouchPhase.Ended) {
					if (OnCoreCommand != null) {
						OnCoreCommand (touchWorldPoint);
					}
				}
			}


			if (currentState == InputState.NoMode) {

				List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (touchWorldPoint, 0.4f);
				if (nodesInRadius.Count > 0) {
					if (touches [0].phase == TouchPhase.Began) {
						GameWorld.Instance.GetNearestFungusNode (touchWorldPoint).ToggleActive ();
					}
				} else {
					///////////////
					//Move Camera//
					///////////////
					if (touches [0].phase == TouchPhase.Began) {

					} else if (touches [0].phase == TouchPhase.Moved) {
						Vector2 touchMovement = touches [0].deltaPosition;

						float posX = touchMovement.x * -moveSpeedX * Time.deltaTime;
								
						float posZ = touchMovement.y * -moveSpeedZ * Time.deltaTime;


						cam.transform.position += new Vector3 (posX, 0, posZ);
						//ClampCamPos ();
					} 
				}
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
				
		inputTimer += Time.deltaTime;

	}

	private FungusNode GetNodeAroundTouch (Vector3 _touchPos)
	{
		FungusNode node = GameWorld.Instance.GetNearestFungusNode (_touchPos);
		return node;
	}

	private void CalcLevelBorders ()
	{
		float vSize = cam.orthographicSize;
		float hSize = cam.orthographicSize * Screen.width / Screen.height;
		leftBorder = hSize - levelSizeX / 2.0f;
		rightBorder = levelSizeX / 2.0f - hSize;
		upperBorder = vSize - levelSizeY / 2.0f;
		lowerBorder = levelSizeY / 2.0f - vSize;
	}

	private void ClampCamPos ()
	{
		Vector3 limitedCameraPosition = cam.transform.position;
		limitedCameraPosition.x = Mathf.Clamp (limitedCameraPosition.x, leftBorder, rightBorder);
		limitedCameraPosition.z = Mathf.Clamp (limitedCameraPosition.z, upperBorder, lowerBorder);
		cam.transform.position = limitedCameraPosition;
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
		if (_button.isUnlocked) {
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
		inputTimer = 0.0f;	
	}

	public void ToggleSkillMenu ()
	{
		bool current = this.skillMenu.activeSelf;
		this.skillMenu.SetActive (!current);
		if (this.skillMenu.activeSelf) {
			currentState = InputState.SkillMode;
		} else {
			currentState = InputState.NoMode;
		}
	}

	public void ToggleBuildMode ()
	{
		if (currentState == InputState.BuildMode) {
			currentState = InputState.NoMode;
		} else {
			currentState = InputState.BuildMode;
		}
	}

	public void ToggleBrainMode ()
	{
		if (currentState == InputState.MoveBrainMode) {
			currentState = InputState.NoMode;
		} else {
			currentState = InputState.MoveBrainMode;
		}
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
				fungusNode.Specialize (FungusResources.Instance.beatneat);
				break;
			}
		case ButtonName.ButtonName.ScentNode:
			{
				fungusNode.Specialize (FungusResources.Instance.attract);
				break;
			}
		case ButtonName.ButtonName.SlowDown:
			{
				fungusNode.Specialize (FungusResources.Instance.slowdown);
				break;
			}
		case ButtonName.ButtonName.SpeedUp:
			{
				fungusNode.Specialize (FungusResources.Instance.speedup);
				break;
			}
		case ButtonName.ButtonName.Zombies:
			{
				fungusNode.Specialize (FungusResources.Instance.zombies);
				break;
			}
		case ButtonName.ButtonName.GrowthSpores:
			{
				fungusNode.Specialize (FungusResources.Instance.growth);
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


	void OnDrawGizmo ()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere (touchWorldPoint, 0.5f);
	}

}
