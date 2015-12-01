using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using NPCBehaviours;

[RequireComponent(typeof(Seeker))]
public class Enemy : Entity
{
    Seeker seeker;
    Vector3 targetPosition;
    List<Vector3> pathToTarget = new List<Vector3>();
    Entity target;

    [Header("Behaviour")]
    public NPCBehaviour behaviour;
    public float attackRadius = 0.2f;
    public int damagerPerSecond = 20;

    const float repathRate = 1f;
    float lastPath;

    public float moveSpeed = 1f;

    protected override void Initialize()
    {
        seeker = GetComponent<Seeker>();
    }

    protected override void Tick(float deltaTime)
    {
        if (behaviour) { behaviour.Evaluate(this, deltaTime); }
        if (Time.time - lastPath > repathRate)
        {
            RequestPath(targetPosition);
            lastPath = Time.time;
        }
        if (pathToTarget.Count > 0)
        {
            if (AstarMath.SqrMagnitudeXZ(pathToTarget[0], transform.position) < 0.05f)
            {
                pathToTarget.RemoveAt(0);
            }
            if (pathToTarget.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToTarget[0], deltaTime * moveSpeed);
            }
        }
    }

    //void SelectTarget()
    //{
    //    FungusNode nearestNode = world.GetNearestFungusNode(transform.position);
    //    if (!nearestNode)
    //    {
    //        target = world.Core;
    //    }
    //    else
    //    {
    //        FungusCore core = world.Core;
    //        if (!core) { return; }
    //        float nnDist = Vector3.SqrMagnitude(nearestNode.transform.position - transform.position);
    //        float cDist = Vector3.SqrMagnitude(core.transform.position - transform.position);
    //        if (nnDist < cDist)
    //        {
    //            target = nearestNode;
    //        }
    //        else
    //        {
    //            target = core;
    //        }
    //    }
    //}

    public bool RegisterBehaviour(NPCBehaviour behaviour)
    {
        if (this.behaviour != null) { return false; }
        this.behaviour = behaviour;
        behaviour.Initialize(this);
        return true;
    }

    public void UnregisterBehaviour(NPCBehaviour behaviour)
    {
        if (this.behaviour == behaviour)
        {
            this.behaviour = null;
        }
    }

    void Attack()
    {
        target.Damage(this, (int)(damagerPerSecond*GameWorld.TickInterval));
    }

    public override void Damage(Entity attacker, int amount)
    {
        SubtractHealth(amount);
        if (IsDead) { world.OnEnemyWasKilled(this); }
    }

    public bool PathTo(Vector3 position)
    {
        if (targetPosition != position)
        {
            targetPosition = position;
            RequestPath(position);
        }
        if (pathToTarget.Count > 0)
        {
            if (pathComputing == true) { return false; }
            return AstarMath.SqrMagnitudeXZ(transform.position, pathToTarget[pathToTarget.Count - 1]) <= 0.05f;
        }
        return false;
    }

    void RequestPath(Vector3 position)
    {
        seeker.StartPath(transform.position, targetPosition, OnPathCompleted);
        pathComputing = true;
    }

    bool pathComputing = false;
    void OnPathCompleted(Path p)
    {
        if (p.error) { Debug.LogError("Error in pathfinding"); return; }
        pathToTarget = p.vectorPath;
        pathComputing = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
