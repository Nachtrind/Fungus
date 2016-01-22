using NodeAbilities;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private Vector3 mouseWorldPoint;
	private bool canBuildNode;
	private Vector2 lastMousePos;
	private bool moveMouse;

	//Image Tint Colors
	Color normalTint = new Color (1f, 1f, 1f, 1f);
	public Color selectedTint;
	Color lockedTint = new Color (90 / 255f, 90 / 255f, 90 / 255f, 1f);

	//Plane + Camera
	private Plane plane;
	private Camera cam;

	//Stuff for camera movement &  zoom
	public float moveSpeedX = 0.000100f;
	public float moveSpeedZ = 0.0001f;
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
					BeginBuild (touchWorldPoint);
				}

				if (touches [0].phase == TouchPhase.Moved && canBuildNode) {
					TrackBuild (touchWorldPoint);
				}

				if (touches [0].phase == TouchPhase.Ended && canBuildNode) {
					EndBuild ();
				}

			}

			////////////////////
			//Specialize Nodes//
			////////////////////
			if (currentState == InputState.SkillMode) {	
				if (touches [0].phase == TouchPhase.Ended) {
					List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (touchWorldPoint, 0.4f);
					if (nodesInRadius.Count > 0) {
						if (currentSelection != null) {
							this.SpecializeNode (GameWorld.Instance.GetNearestFungusNode (touchWorldPoint));
						}
					}
				}
			}

			//////////////
			//Move Brain//
			//////////////
			if (currentState == InputState.MoveBrainMode) {
				//Touch	
				if (touches [0].phase == TouchPhase.Ended) {
					if (OnCoreCommand != null) {
						OnCoreCommand (touchWorldPoint);
					}
				}
			}

			/////////////////////////
			//Activate & Deactivate//
			/////////////////////////
			if (currentState == InputState.NoMode) {

				List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (touchWorldPoint, 1.0f);
				if (nodesInRadius.Count > 0) {
					if (touches [0].phase == TouchPhase.Began) {
						GameWorld.Instance.GetNearestFungusNode (touchWorldPoint).ToggleActive ();
					}
				} else {
					///////////////
					//Move Camera//
					///////////////
					if (touches [0].phase == TouchPhase.Moved) {
						Vector2 touchMovement = touches [0].deltaPosition * 0.02f;
						Debug.Log (touchMovement);
						float posX = touchMovement.x * -moveSpeedX * touches [0].deltaTime;

						float posZ = touchMovement.y * -moveSpeedZ * touches [0].deltaTime;

						Debug.Log (cam.transform.position.x);
						cam.transform.position += new Vector3 (posX, 0, posZ);
						Debug.Log (cam.transform.position.x);
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




		/////////////////
		//Mouse Control//
		/////////////////

		//Left Mouse Click
		if (Input.GetMouseButtonUp (0)) {
			//Transform Touch to WorldPosition
			mouseWorldPoint = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
			//////////////
			//Move Brain//
			//////////////
			if (currentState == InputState.MoveBrainMode) {
				if (OnCoreCommand != null) {
					OnCoreCommand (mouseWorldPoint);
				}
			}


		}

		///////////////////
		//Build New Nodes//
		///////////////////
		if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonDown (0) || Input.GetMouseButton (0)) {
			mouseWorldPoint = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
			if (currentState == InputState.BuildMode) {				
				if (Input.GetMouseButtonDown (0)) {
					BeginBuild (mouseWorldPoint);
				}

				if (Input.GetMouseButton (0) && canBuildNode) {
					TrackBuild (mouseWorldPoint);
				}

				if (Input.GetMouseButtonUp (0) && canBuildNode) {
					EndBuild ();
				}

			}
		}


		////////////////////
		//Specialize Nodes//
		////////////////////
		if (currentState == InputState.SkillMode) {	
			if (Input.GetMouseButtonUp (0)) {
				mouseWorldPoint = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
				List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (mouseWorldPoint, 0.4f);
				if (nodesInRadius.Count > 0) {
					if (currentSelection != null) {
						this.SpecializeNode (GameWorld.Instance.GetNearestFungusNode (mouseWorldPoint));
					}
				}
			}
		}


		/////////////////////////
		//Activate & Deactivate//
		/////////////////////////
		if (currentState == InputState.NoMode) {

			if (Input.GetMouseButtonUp (0) || Input.GetMouseButtonDown (0) || Input.GetMouseButton (0)) {
				mouseWorldPoint = GetTouchPosInWorld (cam.ScreenPointToRay (Input.mousePosition));
				List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (mouseWorldPoint, 0.4f);
				if (nodesInRadius.Count > 0 && inputTimer > inputTick) {
					GameWorld.Instance.GetNearestFungusNode (mouseWorldPoint).ToggleActive ();
					inputTimer = 0.0f;
				} else if (inputTimer > inputTick) {
					///////////////
					//Move Camera//
					///////////////
					if (Input.GetMouseButtonDown (0)) {
						lastMousePos = Input.mousePosition;
					} else if (Input.GetMouseButton (0)) {
						Vector2 current = Input.mousePosition;
						Vector2 deltaMouse = current - lastMousePos;

						float posX = deltaMouse.x * -moveSpeedX;

						float posZ = deltaMouse.y * -moveSpeedZ;


						cam.transform.position += new Vector3 (posX, 0, posZ);
						//ClampCamPos ();
					} 
				}
			}
		}


				
		inputTimer += Time.deltaTime;

	}

	private void ClampCamPos ()
	{
		
		Vector3 limitedCameraPosition = cam.transform.position;

		Debug.Log ("Original Cam Pos: " + limitedCameraPosition);

		float distance;
		if (plane.Raycast (cam.ScreenPointToRay (new Vector3 (0, 0)), out distance)) {

			float frustumHeight = 2.0f * distance * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
			float frustumWidth = frustumHeight * cam.aspect;


			limitedCameraPosition.x = Mathf.Clamp (limitedCameraPosition.x, leftBorder + frustumWidth / 2, rightBorder - frustumWidth / 2);
			limitedCameraPosition.z = Mathf.Clamp (limitedCameraPosition.z, upperBorder - frustumHeight / 2, lowerBorder + frustumHeight / 2);
			Debug.Log ("Limited Cam Pos: " + limitedCameraPosition);
			cam.transform.position = limitedCameraPosition;

		}
	}

	private FungusNode GetNodeAroundTouch (Vector3 _touchPos)
	{
		FungusNode node = GameWorld.Instance.GetNearestFungusNode (_touchPos);
		return node;
	}

	private void BeginBuild (Vector3 _pos)
	{

		List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (_pos, 0.4f);
		if (nodesInRadius.Count > 0) {
			if (!spores.isPlaying || spores.emission.enabled == false) {
				ParticleSystem.EmissionModule em = spores.emission;
				spores.Play ();
				em.enabled = true;
				spores.transform.position = new Vector3 (_pos.x, 0.5f, _pos.z);
			}
			canBuildNode = true;
		}
	}

	private void TrackBuild (Vector3 _pos)
	{
		List<FungusNode> nodesInRadius = GameWorld.Instance.GetFungusNodes (_pos, 0.4f);
		spores.transform.position = new Vector3 (_pos.x, 0.5f, _pos.z);

		if (nodesInRadius.Count <= 0) {
			CreateNewSlimePath (_pos);
		}
	}

	private void EndBuild ()
	{

		SpawnNewSlimePath ();
		ParticleSystem.EmissionModule em = spores.emission;
		em.enabled = false;
		canBuildNode = false;

	}

	private void CalcLevelBorders ()
	{
		levelSizeY = ground.bounds.extents.z;
		levelSizeX = ground.bounds.extents.x;

		leftBorder = ground.transform.position.x - levelSizeX;
		rightBorder = ground.transform.position.x + levelSizeX;
		lowerBorder = ground.transform.position.z - levelSizeY;
		upperBorder = ground.transform.position.z + levelSizeY;

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

	public void ToggleSkillMenu (Image _buttonImg)
	{
		bool current = this.skillMenu.activeSelf;
		this.skillMenu.SetActive (!current);
		if (this.skillMenu.activeSelf) {
			currentState = InputState.SkillMode;
			_buttonImg.color = selectedTint;
		} else {
			currentState = InputState.NoMode;
			_buttonImg.color = Color.white;
		}
	}

	public void ToggleBuildMode (Image _buttonImg)
	{
		if (currentState == InputState.BuildMode) {
			currentState = InputState.NoMode;
			_buttonImg.color = Color.white;
		} else {
			currentState = InputState.BuildMode;
			_buttonImg.color = selectedTint;
		}
	}

	public void ToggleBrainMode (Image _buttonImg)
	{
		if (currentState == InputState.MoveBrainMode) {
			currentState = InputState.NoMode;
			_buttonImg.color = Color.white;
		} else {
			currentState = InputState.MoveBrainMode;
			_buttonImg.color = selectedTint;
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


	void OnDrawGizmos ()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere (new Vector3 (0, 0, lowerBorder), 0.5f);
		Gizmos.DrawSphere (new Vector3 (0, 0, upperBorder), 0.5f);
		Gizmos.DrawSphere (new Vector3 (rightBorder, 0, 0), 0.5f);
		Gizmos.DrawSphere (new Vector3 (leftBorder, 0, 0), 0.5f);
	}

}
