using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using NPCBehaviours;
using System;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Human : Entity
{
    public enum MoveResult { Preparing, Moving, ReachedTarget, TargetNotReachable }
    enum MoveTypes { Direct, Pathing }

#if UNITY_EDITOR
    public bool debug = false;
#endif

    #region Movement variables
    const float targetReachDistance = 0.025f;
    Seeker seeker;
    Vector3 lastRequestedMoveTargetPosition;
    List<Vector3> pathToTarget = new List<Vector3>();
    const float repathRate = 0.25f;
    float lastPathRequestTime;
    MoveTypes movementType = MoveTypes.Direct;
    MoveResult moveResult = MoveResult.Preparing;
    public float moveSpeed = 1f;
    #endregion

    public int resourceValue;

    //dummy feedback TODO: delete later on
    ParticleSystem particleDamage;

    [SerializeField, ReadOnlyInInspector]
    NPCBehaviour behaviour;
    public NPCBehaviour Behaviour { get { return behaviour; } }

    protected override void OnAwake()
    {
        seeker = GetComponent<Seeker>();
        GetComponent<CapsuleCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
        particleDamage = GetComponent<ParticleSystem>();
    }

    protected override void Tick(float deltaTime)
    {
        if (behaviour) { behaviour.Evaluate(this, deltaTime); }
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
                }
            }
            if (pathToTarget.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToTarget[0], deltaTime * moveSpeed);
                moveResult = MoveResult.Moving;
            }
        }
    }

    public NPCBehaviour SetBehaviour(NPCBehaviour behaviour)
    {
        if (this.behaviour != null) { this.behaviour.Cleanup(this); Destroy(this.behaviour); }
        this.behaviour = Instantiate(behaviour);
        return this.behaviour;
    }

    public void RemoveBehaviour()
    {
        if (behaviour != null) { behaviour.Cleanup(this); Destroy(behaviour); }
    }

    public bool Attack(Entity target, int amount)
    {
        if (!target.isAttackable) { return false; }
        target.Damage(this, amount);
        return true;
    }

    public override void Damage(Entity attacker, int amount)
    {
        SubtractHealth(amount);
        particleDamage.Play();
        if (IsDead)
        {
            world.OnHumanWasKilled(this);
        }
    }

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

    public override void ReceiveBroadcast(Message message)
    {
        if (behaviour)
        {
            behaviour.OnReceivedBroadcastMessage(message);
        }
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
        pathToTarget = p.vectorPath;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.15f);
        if (behaviour)
        {
            behaviour.DrawGizmos(this);
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (debug && behaviour)
        {
            behaviour.DrawDebugInfos(this);
        }
    }
#endif
}
