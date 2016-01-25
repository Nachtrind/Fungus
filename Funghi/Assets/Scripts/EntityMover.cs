using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
public class EntityMover: MonoBehaviour
{
    #region Movement variables
    public enum MoveResult { Preparing, Moving, ReachedTarget, TargetNotReachable, NotAllowed }
    enum MoveTypes { Direct, Pathing, FleePathing }
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
    public MovePathSmoothing extraPathSmoothing = MovePathSmoothing.NoSmoothing;
    Vector3 lookDirection;
    #endregion

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        lookDirection = transform.forward;
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

    public MoveResult FleeFrom(Vector3 position)
    {
        movementType = MoveTypes.FleePathing;
        if (lastRequestedMoveTargetPosition != position)
        {
            pathToTarget.Clear();
            moveResult = MoveResult.Preparing;
            lastRequestedMoveTargetPosition = position;
            if (!RequestFleePath(position))
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

    /// <summary>
    /// only usable when not moving
    /// </summary>
    public void LookAt(Vector3 position)
    {
        lookDirection = (position - transform.position).normalized;
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

    bool RequestFleePath(Vector3 fromPosition)
    {
        if (movementType != MoveTypes.FleePathing)
        {
            return false;
        }
        if (!seeker.IsDone())
        {
            return false;
        }
        moveResult = MoveResult.Preparing;
        var fp = FleePath.Construct(transform.position, fromPosition, 2000);
        fp.aim = (transform.position- fromPosition) *10;
        fp.aimStrength = 5f;
        seeker.StartPath(fp, OnPathCompleted);
        return true;
    }

    void OnPathCompleted(Path p)
    {
        if (movementType != MoveTypes.Pathing && movementType != MoveTypes.FleePathing)
        {
            return;
        }
        if (p.error)
        {
            pathToTarget.Clear();
            moveResult = MoveResult.TargetNotReachable;
            return;
        }
        switch (extraPathSmoothing)
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

    public void UpdateMovement(float deltaTime)
    {
        if (Time.time - lastPathRequestTime > repathRate)
        {
            if (movementType == MoveTypes.Pathing)
            {
                if (RequestPath(lastRequestedMoveTargetPosition))
                {
                    lastPathRequestTime = Time.time;
                }
            }
            if (movementType == MoveTypes.FleePathing)
            {
                if (RequestFleePath(lastRequestedMoveTargetPosition))
                {
                    lastPathRequestTime = Time.time;
                }
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
                LookAt(pathToTarget[0]);
            }
            if (pathToTarget.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, pathToTarget[0], deltaTime * moveSpeed);
                moveResult = MoveResult.Moving;
            }
        }
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDirection), deltaTime / turnSpeed);
        }
    }
}
