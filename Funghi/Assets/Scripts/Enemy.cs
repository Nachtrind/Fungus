using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
public class Enemy : Entity
{
    Seeker seeker;
    List<Vector3> pathToTarget = new List<Vector3>();
    Entity target;

    [Header("Attacking")]
    public float attackRadius = 0.2f;
    public int damagerPerSecond = 20;

    const float repathRate = 0.5f;
    float lastPath;

    public float moveSpeed = 1f;

    protected override void Initialize()
    {
        seeker = GetComponent<Seeker>();
    }

    protected override void Tick(float deltaTime)
    {
        SelectTarget();
        if (Time.time - lastPath > repathRate)
        {
            PathTo(target.transform.position);
            lastPath = Time.time;
        }
        if (pathToTarget.Count > 0)
        {
            if (AstarMath.SqrMagnitudeXZ(pathToTarget[0], transform.position) >= 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToTarget[0], deltaTime * moveSpeed);
            }
            else
            {
                pathToTarget.RemoveAt(0);
            }
        }
        else
        {
            if (AstarMath.SqrMagnitudeXZ(target.transform.position, transform.position) < attackRadius * attackRadius)
            {
                Attack();
            }
        }
    }

    void SelectTarget()
    {
        FungusNode nearestNode = world.GetNearestFungusNode(transform.position);
        if (!nearestNode)
        {
            target = world.Core;
        }
        else
        {
            FungusCore core = world.Core;
            if (!core) { return; }
            float nnDist = Vector3.SqrMagnitude(nearestNode.transform.position - transform.position);
            float cDist = Vector3.SqrMagnitude(core.transform.position - transform.position);
            if (nnDist < cDist)
            {
                target = nearestNode;
            }
            else
            {
                target = core;
            }
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

    void PathTo(Vector3 position)
    {
        seeker.StartPath(transform.position, position, OnPathCompleted);
    }

    void OnPathCompleted(Path p)
    {
        if (p.error) { Debug.LogError("Error in pathfinding"); return; }
        pathToTarget = p.vectorPath;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
