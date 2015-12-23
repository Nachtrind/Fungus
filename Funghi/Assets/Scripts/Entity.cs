using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using ModularBehaviour;

[RequireComponent(typeof(Seeker))]
public abstract class Entity : MonoBehaviour
{
    protected GameWorld world;
    #region Health
    [Header("State")]
    [SerializeField]
    int currentHealth;
    public int Health { get { return currentHealth; } }
    [SerializeField]
    int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    public bool isAttackable = true;

    public void Damage(Entity attacker, int amount)
    {
        SubtractHealth(amount);
        OnDamage(attacker);
    }
    public virtual void OnDamage(Entity attacker) { }
    public void Kill(Entity murderer)
    {
        Damage(murderer, currentHealth);
    }

    protected void AddHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    protected void SubtractHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
    }

    public bool IsDead
    {
        get { return currentHealth <= 0; }
    }
    #endregion

    #region Behaviour
    [SerializeField, ReadOnlyInInspector]
    protected Intelligence behaviour;
    /// <summary>
    /// Direct access to the behaviour, used by the Spawner (for triggers use <seealso cref="TriggerBehaviour(string, object)"/>)
    /// </summary>
    public Intelligence Behaviour { get { return behaviour; } }

    /// <summary>
    /// Overrides the behaviour of this entity
    /// </summary>
    /// <param name="behaviour">selected behaviour to replace the old one</param>
    /// <returns></returns>
    public bool SetBehaviour(Intelligence behaviour)
    {
        if (this.behaviour != null) { Destroy(this.behaviour); }
        if (behaviour)
        {
            this.behaviour = Instantiate(behaviour);
            this.behaviour.Initialize(this);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the behaviour of this entity, it will no longer react to triggers or make decisions
    /// </summary>
    public void RemoveBehaviour()
    {
        if (behaviour != null) { Destroy(behaviour); }
    }

    /// <summary>
    /// If the behaviour supports <paramref name="triggerName"/> it will be executed and <paramref name="optionalParameter"/> will be passed to it
    /// </summary>
    /// <param name="triggerName">the name of trigger specified in the behaviour editor</param>
    /// <param name="optionalParameter">an optional parameter which the behaviour can use</param>
    public bool TriggerBehaviour(string triggerName, object optionalParameter = null)
    {
        if (behaviour)
        {
            return behaviour.TryExecuteTrigger(triggerName, optionalParameter);
        }
        return false;
    }
    #endregion

    void Awake()
    {
        world = GameWorld.Instance;
        world.Register(this);
        seeker = GetComponent<Seeker>();
        OnAwake();
    }

    void Start()
    {
        lookTarget = transform.position + Vector3.forward;
        OnStart();
    }

    public void UpdateEntity(float delta)
    {
        HandleMovement(delta);
        if (behaviour) { behaviour.UpdateTick(delta); }
        OnUpdate(delta);
    }

    void OnDestroy()
    {
        Cleanup();
        world.Unregister(this);
    }

    protected virtual void OnStart() { }
    protected virtual void OnAwake() { }
    protected virtual void OnUpdate(float deltaTime) { }
    protected virtual void Cleanup() { }

    #region Movement

    #region Movement variables
    public enum MoveResult { Preparing, Moving, ReachedTarget, TargetNotReachable }
    enum MoveTypes { Direct, Pathing }
    public enum MovePathSmoothing { NoSmoothing, Force, LikeSlime }

    const float targetReachDistance = 0.025f;
    Seeker seeker;
    Vector3 lastRequestedMoveTargetPosition;
    List<Vector3> pathToTarget = new List<Vector3>();
    const float repathRate = 0.25f;
    float lastPathRequestTime;
    MoveTypes movementType = MoveTypes.Direct;
    MoveResult moveResult = MoveResult.Preparing;
    public float moveSpeed = 1f;
    [Range(0.01f, 0.5f)]
    public float turnSpeed = 0.1f;
    public MovePathSmoothing movePathSmoothing = MovePathSmoothing.NoSmoothing;
    #endregion

    public void StopMovement()
    {
        movementType = MoveTypes.Direct;
        pathToTarget = new List<Vector3> { transform.position };
        moveResult = MoveResult.ReachedTarget;
    }

    /// <summary>
    /// Uses pathfinding to reach the target
    /// </summary>
    public MoveResult MoveTo(Vector3 position)
    {
        movementType = MoveTypes.Pathing;
        if (lastRequestedMoveTargetPosition != position)
        {
            pathToTarget.Clear();
            moveResult = MoveResult.Preparing;
            lastRequestedMoveTargetPosition = position;
            if (!RequestPath(lastRequestedMoveTargetPosition))
            {
                lastRequestedMoveTargetPosition = Vector3.zero;
            }
        }
        return moveResult;
    }

    /// <summary>
    /// ignores pathfinding to reach the target
    /// </summary>
    public MoveResult MoveToDirect(Vector3 position)
    {
        movementType = MoveTypes.Direct;
        if (lastRequestedMoveTargetPosition != position)
        {
            moveResult = MoveResult.Moving;
            pathToTarget = new List<Vector3> { position };
            lastRequestedMoveTargetPosition = position;
        }
        return moveResult;
    }

    bool RequestPath(Vector3 position)
    {
        if (movementType != MoveTypes.Pathing)
        {
            return false;
        }
        if (!seeker.IsDone())
        {
            return false;
        }
        moveResult = MoveResult.Preparing;
        seeker.StartPath(transform.position, lastRequestedMoveTargetPosition, OnPathCompleted);
        return true;
    }

    void OnPathCompleted(Path p)
    {
        if (movementType != MoveTypes.Pathing)
        {
            return;
        }
        if (p.error)
        {
            pathToTarget.Clear();
            moveResult = MoveResult.TargetNotReachable;
            return;
        }
        switch (movePathSmoothing)
        {
            default:
                pathToTarget = p.vectorPath;
                break;
            case MovePathSmoothing.Force:
                pathToTarget = GameWorld.Instance.SmoothPath(p.vectorPath, false);
                break;
            case MovePathSmoothing.LikeSlime:
                pathToTarget = GameWorld.Instance.SmoothPath(p.vectorPath, true);
                break;
        }
    }

    Vector3 lookTarget;

    protected void HandleMovement(float deltaTime)
    {
        if (Time.time - lastPathRequestTime > repathRate)
        {
            if (RequestPath(lastRequestedMoveTargetPosition))
            {
                lastPathRequestTime = Time.time;
            }
        }
        if (pathToTarget.Count > 0)
        {
            if (AstarMath.SqrMagnitudeXZ(pathToTarget[0], transform.position) <= targetReachDistance)
            {
                pathToTarget.RemoveAt(0);
                if (pathToTarget.Count == 0)
                {
                    moveResult = MoveResult.ReachedTarget;
                    return;
                }
                lookTarget = pathToTarget[0] + (pathToTarget[0] - transform.position).normalized;
            }
            if (pathToTarget.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToTarget[0], deltaTime * moveSpeed);
                moveResult = MoveResult.Moving;
            }
        }
        Vector3 lookVector = (lookTarget - transform.position);
        if (lookVector != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookVector), deltaTime/turnSpeed);
        }
    }

    /// <summary>
    /// only usable when not moving
    /// </summary>
    public void LooAt(Vector3 position)
    {
        lookTarget = position;
    }
    #endregion

    #region Messaging
    public virtual void ReceiveBroadcast(Message message)
    {
        if (behaviour)
        {
            behaviour.HandleMessageBroadcast(message);
        }
    }
    #endregion

}
