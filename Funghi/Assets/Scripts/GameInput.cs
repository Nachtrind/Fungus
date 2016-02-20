using System;
using System.Collections.Generic;
using NodeAbilities;
using Pathfinding;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    static InputHandler handlerInstance;

    static GameInput instance;
    Camera cam;

    [SerializeField] ClampCamera camClampComponent;

    bool canBuildNode;
    AbilityButton currentSelection;

    InputState currentState;

    EventSystem eventsystem;

    public Renderer ground;

    Vector2 lastMousePos;
    float lastRequest;

    //Level Borders
    float leftBorder;
    bool leftSkillMode;
    float levelSizeX;
    //private Vector2 scrollDirection = Vector2.zero;

    float levelSizeY;
    float lowerBorder;
    Vector3 mouseWorldPoint;
    bool moveMouse;

    //Stuff for camera movement &  zoom
    public float moveSpeedX = 2.1f;
    public float moveSpeedZ = 2.1f;

    public float nodeCost;

    List<Vector3> pathToCursor = new List<Vector3>();
    float pathToCursorLength;
    //Color lockedTint = new Color (90 / 255f, 90 / 255f, 90 / 255f, 1f);

    //Plane + Camera
    Plane plane;
    float requestInterval = 0.1f;
    float rightBorder;
    NodeAbility selectedSkill;
    public Color selectedTint;

    GameObject skillMenu;
    float skillTick = 0.25f;

    //Skill Mode Timer
    float skillTimer;


    public ParticleSystem spores;

    //Stuff for Building a new Node
    Vector3 touchWorldPoint;
    float upperBorder;
    public float zoomSpeed = 0.1f;

    public static GameInput Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameInput>();
            }
            return instance;
        }
    }

    //private float inputTick = 0.2f;
    //float coreInputRange = 0.1f;

    static event Action<Vector3> OnCoreCommand;
    static event Func<Vector3, FungusNode> OnSpawnFungusCommand;

    void Start()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
        cam = Camera.main;
        currentState = InputState.NoMode;
        eventsystem = EventSystem.current;
        selectedSkill = FungusResources.Instance.attract;
    }

    void Update()
    {
        if (!cam)
        {
            Debug.LogError("No Camera tagged as MainCamera");
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
            return;
        }

        if (eventsystem == null)
        {
            eventsystem = EventSystem.current;
        }

        ///////////////
        //Get Touches//
        ///////////////
        var touches = Input.touches;

        if (currentState != InputState.NoMode && touches.Length > 0)
        {
            var touchToUseInWorld = 0;
            //decide which touch to use
            if (touches.Length == 2)
            {
                if (!eventsystem.IsPointerOverGameObject(0))
                {
                    touchToUseInWorld = 0;
                }
                if (!eventsystem.IsPointerOverGameObject(1))
                {
                    touchToUseInWorld = 1;
                }
            }

            //Transform Touch to WorldPosition
            touchWorldPoint = GetTouchPosInWorld(cam.ScreenPointToRay(touches[touchToUseInWorld].position));
            spores.transform.position = cam.transform.position +((new Vector3(touchWorldPoint.x, 0.1f, touchWorldPoint.z) - cam.transform.position).normalized)*3;

            ///////////////////
            //Build New Nodes//
            ///////////////////
            if (currentState == InputState.BuildMode)
            {
                if (touches[touchToUseInWorld].phase == TouchPhase.Began)
                {
                    if (!BeginBuild(touchWorldPoint))
                    {
                        UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.None);
                        lastMousePos = Input.mousePosition;
                        return;
                    }
                }
                if (canBuildNode && touches[touchToUseInWorld].phase == TouchPhase.Moved)
                {
                    TrackBuild(touchWorldPoint);
                }
                if (canBuildNode && touches[touchToUseInWorld].phase == TouchPhase.Ended)
                {
                    if (EndBuild())
                    {
                        if (FungusResources.Instance.CurrentResources >= nodeCost)
                        {
                            FungusResources.Instance.SubResources(nodeCost);
                        }
                    }
                    return;
                }
            }

            ////////////////////
            //Specialize Nodes//
            ////////////////////
            if (currentState == InputState.SkillMode)
            {
                if (touches[touchToUseInWorld].phase == TouchPhase.Began & !eventsystem.IsPointerOverGameObject(touchToUseInWorld))
                {
                    var nodesInRadius = GameWorld.Instance.GetFungusNodes(touchWorldPoint, 0.55f);
                    if (nodesInRadius.Count > 0)
                    {
                        if (selectedSkill != null)
                        {
                            SpecializeNode(GameWorld.Instance.GetNearestFungusNode(touchWorldPoint));
                            UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.Skill);
                            return;
                        }
                    }
                    else
                    {
                        UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.None);
                        lastMousePos = Input.mousePosition;
                        return;
                    }
                }
            }

            //////////////
            //Move Brain//
            //////////////
            if (currentState == InputState.MoveBrainMode)
            {
                //Touch	
                if (touches[touchToUseInWorld].phase == TouchPhase.Began & !eventsystem.IsPointerOverGameObject(touchToUseInWorld))
                {
                    if (OnCoreCommand != null)
                    {
                        OnCoreCommand(touchWorldPoint);
                        UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.None);
                        lastMousePos = Input.mousePosition;
                        return;
                    }
                }
            }
        }

        /////////////////////////
        //Activate & Deactivate//
        /////////////////////////
        if (touches.Length == 1)
        {
            var touchToUseInWorld = 0;
            if (!eventsystem.IsPointerOverGameObject(0))
            {
                touchWorldPoint = GetTouchPosInWorld(cam.ScreenPointToRay(touches[touchToUseInWorld].position));
                if (currentState == InputState.NoMode)
                {
                    var nodesInRadius = GameWorld.Instance.GetFungusNodes(touchWorldPoint, 1.0f);
                    if (nodesInRadius.Count > 0)
                    {
                        if (touches[touchToUseInWorld].phase == TouchPhase.Began)
                        {
                            GameWorld.Instance.GetNearestFungusNode(touchWorldPoint).ToggleActive();
                        }
                    }
                    else
                    {
                        ///////////////
                        //Move Camera//
                        ///////////////
                        if (touches[0].phase == TouchPhase.Moved)
                        {
                            var touchMovement = touches[0].deltaPosition*0.02f;

                            var posX = touchMovement.x*-moveSpeedX*touches[touchToUseInWorld].deltaTime;
                            var posZ = touchMovement.y*-moveSpeedZ*touches[touchToUseInWorld].deltaTime;

                            cam.transform.position += new Vector3(posX, 0, posZ);
                        }

                        if (touches[touchToUseInWorld].phase == TouchPhase.Began)
                        {
                            lastMousePos = touches[touchToUseInWorld].position;
                        }
                        else if (Input.GetMouseButton(0))
                        {
                            Vector2 current = Input.mousePosition;

                            var lastMouseInWorldPos = GetTouchPosInWorld(cam.ScreenPointToRay(lastMousePos));
                            var currentMouseInWorldPos = GetTouchPosInWorld(cam.ScreenPointToRay(current));

                            var x = (currentMouseInWorldPos.x - lastMouseInWorldPos.x)*-moveSpeedX;
                            var z = (currentMouseInWorldPos.z - lastMouseInWorldPos.z)*-moveSpeedZ;

                            cam.transform.position += new Vector3(x, 0, z);

                            lastMousePos = current;
                            camClampComponent.ClampCamViewPos(cam);
                            //ClampCamPos ();
                        }
                    }
                }
                else
                {
                    if (touches[0].phase == TouchPhase.Began)
                    {
                        var nodesInRadius = GameWorld.Instance.GetFungusNodes(touchWorldPoint, 1.0f);
                        if (nodesInRadius.Count == 0)
                        {
                            UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.None);
                            Debug.Log("activate none");
                            lastMousePos = Input.mousePosition;
                            return;
                        }
                    }
                }
            }
        }

        ///////////////
        //Zoom Camera//
        ///////////////
        if (touches.Length == 2 && currentState == InputState.NoMode)
        {
            if (!eventsystem.IsPointerOverGameObject(0) && !eventsystem.IsPointerOverGameObject(1))
            {
                var touchOne = touches[0];
                var touchTwo = touches[1];

                var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                var touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

                var prevTouchLength = (touchOnePrevPos - touchTwoPrevPos).magnitude;
                var touchDeltaLength = (touchOne.position - touchTwo.position).magnitude;

                var lengthDiff = prevTouchLength - touchDeltaLength;

                cam.fieldOfView += lengthDiff*zoomSpeed;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 10.0f, 40.0f);
                camClampComponent.ClampCamViewPos(cam);
            }
        }
        skillTimer += Time.deltaTime;
        //Check SkillMode Deactivation
        if (leftSkillMode && skillTimer >= skillTick)
        {
            currentState = InputState.NoMode;
            leftSkillMode = false;
        }

    }

    bool BeginBuild(Vector3 _pos)
    {
        var nodesInRadius = GameWorld.Instance.GetFungusNodes(_pos, 0.4f);
        if (nodesInRadius.Count > 0)
        {
            if (!spores.isPlaying || spores.emission.enabled == false)
            {
                var em = spores.emission;
                spores.Play();
                em.enabled = true;
                //spores.transform.position = new Vector3(_pos.x, 0.1f, _pos.z);
            }
            canBuildNode = true;
            return true;
        }
        return false;
    }

    void TrackBuild(Vector3 _pos)
    {
        var nodesInRadius = GameWorld.Instance.GetFungusNodes(_pos, 0.4f);
        //spores.transform.position = new Vector3(_pos.x, 0.5f, _pos.z);

        if (nodesInRadius.Count <= 0)
        {
            CreateNewSlimePath(_pos);
        }
    }

    bool EndBuild()
    {
        var ret = SpawnNewSlimePath();
        if (ret)
        {
            UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.Build);
        }
        else
        {
            UserMenu.current.InputModeCallback(UserMenu.UserMenuButtonType.None);
        }
        var em = spores.emission;
        em.enabled = false;
        canBuildNode = false;
        return ret;
    }

    public Vector3 GetTouchPosInWorld(Ray _ray)
    {
        float enter;
        if (plane.Raycast(_ray, out enter))
        {
            var point = _ray.GetPoint(enter);
            return point;
        }
        return new Vector3(0, 0, 0);
    }


    public void SelectSkill(UserMenu.AbilityType _button)
    {
        currentState = InputState.SkillMode;
        switch (_button)
        {
            case UserMenu.AbilityType.Lure:
                if (FungusResources.Instance.attract.isUnlocked)
                {
                    selectedSkill = FungusResources.Instance.attract;
                }
                break;
            case UserMenu.AbilityType.Eat:
                if (FungusResources.Instance.beatneat.isUnlocked)
                {
                    selectedSkill = FungusResources.Instance.beatneat;
                }
                break;
            case UserMenu.AbilityType.Spawn:
                if (FungusResources.Instance.growth.isUnlocked)
                {
                    selectedSkill = FungusResources.Instance.growth;
                }
                break;
            case UserMenu.AbilityType.Slow:
                if (FungusResources.Instance.slowdown.isUnlocked)
                {
                    selectedSkill = FungusResources.Instance.slowdown;
                }
                break;
            case UserMenu.AbilityType.Speedup:
                if (FungusResources.Instance.speedup.isUnlocked)
                {
                    selectedSkill = FungusResources.Instance.speedup;
                }
                break;
            case UserMenu.AbilityType.Enslave:
                if (FungusResources.Instance.zombies.isUnlocked)
                {
                    selectedSkill = FungusResources.Instance.zombies;
                }
                break;
            default:
                Debug.Log("Something went wrong");
                selectedSkill = null;
                break;
        }
    }


    public void ActivateMode(UserMenu.UserMenuButtonType type)
    {
        leftSkillMode = false;
        switch (type)
        {
            case UserMenu.UserMenuButtonType.Brain:
                currentState = InputState.MoveBrainMode;
                break;
            case UserMenu.UserMenuButtonType.Build:
                currentState = InputState.BuildMode;
                break;
            case UserMenu.UserMenuButtonType.Skill:
                currentState = InputState.SkillMode;
                break;
            case UserMenu.UserMenuButtonType.Wind:
                currentState = InputState.ChangeWind;
                break;
            case UserMenu.UserMenuButtonType.None:
                currentState = InputState.NoMode;
                skillTimer = 0.0f;
                leftSkillMode = true;
                break;
        }
    }

    void SpecializeNode(FungusNode fungusNode)
    {
        fungusNode.Specialize(selectedSkill);
    }

    public static void RegisterCoreMoveCallback(Action<Vector3> callback)
    {
        OnCoreCommand += callback;
    }

    public static void ReleaseCoreMoveCallback(Action<Vector3> callback)
    {
        OnCoreCommand -= callback;
    }

    public static void RegisterSpawnFungusCallback(Func<Vector3, FungusNode> callback)
    {
        OnSpawnFungusCommand += callback;
    }

    public static void ReleaseSpawnFungusCallback(Func<Vector3, FungusNode> callback)
    {
        OnSpawnFungusCommand -= callback;
    }

    void OnPathCompleted(Path p)
    {
        if (p.error)
        {
            return;
        }
        pathToCursor = p.vectorPath;
        pathToCursorLength = Mathf.Lerp(Vector3.Distance(pathToCursor[0], pathToCursor[pathToCursor.Count - 1]), p.GetTotalLength(), 0.5f);
    }

    bool SpawnNewSlimePath()
    {
        if (pathToCursor.Count == 0)
        {
            return false;
        }
        var ret = false;
        if (pathToCursorLength <= GameWorld.nodeConnectionDistance)
        {
            if (OnSpawnFungusCommand != null)
            {
                OnSpawnFungusCommand(pathToCursor[pathToCursor.Count - 1]);
            }
            ret = true;
        }
        pathToCursor.Clear();
        return ret;
    }

    void CreateNewSlimePath(Vector3 _mousePosition)
    {
        if (FungusResources.Instance.CurrentResources >= nodeCost)
        {
            _mousePosition.y = 0;
            var nodeNearCursor = GameWorld.Instance.GetNearestFungusNode(_mousePosition);
                //HACK: nearest node is not always shortest path (if needed, compare multiple paths)
            if (nodeNearCursor)
            {
                if (Time.time - lastRequest >= requestInterval)
                {
                    AstarPath.StartPath(ABPath.Construct(nodeNearCursor.transform.position, _mousePosition, OnPathCompleted));
                    lastRequest = Time.time;
                }
                var c = pathToCursorLength > GameWorld.nodeConnectionDistance ? Color.red : Color.green;
                //non gizmo indicator
                spores.startColor = pathToCursorLength > GameWorld.nodeConnectionDistance
                    ? new Color(232/255f, 82/255f, 0/255f, 1f)
                    : new Color(167/255f, 255/255f, 0/255f, 1f);
                for (var i = 1; i < pathToCursor.Count; i++)
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
        else
        {
            spores.startColor = new Color(232/255f, 82/255f, 0/255f, 1f);
        }
    }

    enum InputState
    {
        SkillMode,
        BuildMode,
        MoveBrainMode,
        ChangeWind,
        NoMode
    }
}