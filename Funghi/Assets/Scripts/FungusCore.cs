using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
public class FungusCore : Entity
{

    public float groundCheckInterval = 0.33f;
    Seeker seeker;
    List<Vector3> pathToTarget = new List<Vector3>();

    public float moveSpeed = 1f;

    protected override void Initialize()
    {
        seeker = GetComponent<Seeker>();
        GameInput.RegisterCoreMoveCallback(TryMoveTo);
    }
    protected override void Cleanup()
    {
        GameInput.ReleaseCoreMoveCallback(TryMoveTo);
    }

    protected override void Tick(float deltaTime)
    {
        if (pathToTarget.Count > 0)
        {
            if (AstarMath.SqrMagnitudeXZ(pathToTarget[0], transform.position) >= 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToTarget[0], deltaTime * moveSpeed);
            }
            else
            {
                if (!IsOnValidGround) { world.OnCoreLostGrounding(this); return; }
                pathToTarget.RemoveAt(0);
            }
        }
        else
        {
            if (!IsOnValidGround) { world.OnCoreLostGrounding(this); return; }
        }
    }

    public void TryMoveTo(Vector3 position)
    {
        seeker.StartPath(transform.position, position, OnPathCompleted);
    }

    void OnPathCompleted(Path p)
    {
        if (p.error) { return; }
        pathToTarget = p.vectorPath;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1f, 0.33f);
        Gizmos.DrawSphere(transform.position, 0.15f);
    }

    public bool IsOnValidGround
    {
        get
        {
            NNInfo nn = AstarPath.active.GetNearest(transform.position);
            //Debug.DrawLine(nn.clampedPosition, transform.position);
            return (nn.node.Tag & GameWorld.slimeTag) == GameWorld.slimeTag;
        }
    }

    public override void Damage(Entity attacker, int amount)
    {
        SubtractHealth(amount);
        if (IsDead) { world.OnCoreWasKilled(this); }
    }

}
