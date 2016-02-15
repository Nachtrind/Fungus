using System;
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

    const float TargetReachDistance = 0.05f;
    Seeker _seeker;
    Vector3 _lastRequestedMoveTargetPosition;
    List<Vector3> _pathToTarget = new List<Vector3>();
    const float RepathRate = 0.25f;
    float _lastPathRequestTime;
    MoveTypes _movementType = MoveTypes.Direct;
    MoveResult _moveResult = MoveResult.Preparing;
    public float BaseMoveSpeed = 1f;
    [SerializeField, ReadOnlyInInspector]float _effectiveMovespeed = 1f;
    [Range(0.01f, 0.5f)]
    public float turnSpeed = 0.1f;
    public MovePathSmoothing extraPathSmoothing = MovePathSmoothing.NoSmoothing;
    Vector3 _lookDirection;
    #endregion

    #region Animator
    [SerializeField]
    Animator anim;

    int _speedHash;

    const float NotMoving = 0f;
    const float Moving = 0.5f;
    const float Running = 1f;
    #endregion

    #region SpeedModding

    class SpeedModifier
    {
        static readonly HashSet<int> UsedIDs = new HashSet<int>();

        SpeedModifier(EffectMode type, int percentage)
        {
            _type = type;
            _value = percentage;
        }

        SpeedModifier(SpeedModifier template, int id, float timeout)
        {
            ID = id;
            _type = template._type;
            _value = template._value;
            Timeout = timeout;
            ApplicationTime = Time.time;
        }

        public static SpeedModifier GenerateUnique(SpeedModType type, float timeout)
        {
            var id = 0;
            while (UsedIDs.Contains(id))
            {
                id++;
            }
            UsedIDs.Add(id);
            switch (type)
            {
                case SpeedModType.NoSpeed:
                    return new SpeedModifier(Static, id, timeout);
                case SpeedModType.DoubleSpeed:
                    return new SpeedModifier(DoubleSpeed, id, timeout);
                case SpeedModType.QuarterSlower:
                    return new SpeedModifier(QuarterSlower, id, timeout);
                case SpeedModType.QuarterFaster:
                    return new SpeedModifier(QuarterFaster, id, timeout);
                case SpeedModType.HalfSlower:
                    return new SpeedModifier(HalfSlower, id, timeout);
                case SpeedModType.HalfFaster:
                    return new SpeedModifier(HalfFaster, id, timeout);
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static void ReleaseModifierID(SpeedModifier mod)
        {
            UsedIDs.Remove(mod.ID);
        }

        public void Apply(ref float value)
        {
            if (_type == EffectMode.Add)
            {
                value += value*(_value*0.01f);
            }
            else
            {
                value -= value*(_value*0.01f);
            }
        }

        static readonly SpeedModifier HalfFaster = new SpeedModifier(EffectMode.Add, 50);
        static readonly SpeedModifier HalfSlower = new SpeedModifier(EffectMode.Subtract, 50);
        static readonly SpeedModifier QuarterSlower = new SpeedModifier(EffectMode.Subtract, 25);
        static readonly SpeedModifier QuarterFaster = new SpeedModifier(EffectMode.Add, 25);
        static readonly SpeedModifier DoubleSpeed = new SpeedModifier(EffectMode.Add, 100);
        static readonly SpeedModifier Static = new SpeedModifier(EffectMode.Subtract, 100);

        enum EffectMode
        {
            Add,
            Subtract
        }

        public readonly int ID;
        readonly EffectMode _type;
        readonly int _value;
        public readonly float Timeout = -1f;
        public readonly float ApplicationTime;
        public Entity Owner;
    }

    public enum SpeedModType
    {
        NoSpeed,
        DoubleSpeed,
        QuarterSlower,
        QuarterFaster,
        HalfSlower,
        HalfFaster,
    }

    readonly List<SpeedModifier> _speedMods = new List<SpeedModifier>();

    void TimeOutSpeedmods()
    {
        for (var i=_speedMods.Count;i-->0;)
        {
            if (_speedMods[i].Timeout < 0) continue;
            if (Time.time - _speedMods[i].ApplicationTime > _speedMods[i].Timeout)
            {
                _speedMods.RemoveAt(i);
                RecalculateSpeed();
            }
        }
    }

    public int ApplySpeedMod(Entity source, SpeedModType type, float timeout)
    {
        var mod = SpeedModifier.GenerateUnique(type, timeout);
        mod.Owner = source;
        if (timeout > 0)
        {
            RevokeAllSpeedMods(source);
        }
        _speedMods.Add(mod);
        RecalculateSpeed();
        return mod.ID;
    }

    public void RevokeSpeedMod(int id)
    {
        for (var i = 0; i < _speedMods.Count; i++)
        {
            if (_speedMods[i].ID != id) continue;
            SpeedModifier.ReleaseModifierID(_speedMods[i]);
            _speedMods.RemoveAt(i);
            break;
        }
        RecalculateSpeed();
    }

    public void RevokeAllSpeedMods(Entity fromSource)
    {
        for (var i = _speedMods.Count; i-- > 0;)
        {
            if (_speedMods[i].Owner == fromSource)
            {
                _speedMods.RemoveAt(i);
            }
        }
        RecalculateSpeed();
    }

    void RecalculateSpeed()
    {
        if (_speedMods.Count == 0)
        {
            _effectiveMovespeed = BaseMoveSpeed;
            return;
        }
        var baseSpeed = BaseMoveSpeed;
        for (var i = 0; i < _speedMods.Count; i++)
        {
            _speedMods[i].Apply(ref baseSpeed);
        }
        _effectiveMovespeed = baseSpeed;
    }

    #endregion

    void Awake()
    {
        if (!anim)
        {
            anim = GetComponentInChildren<Animator>();
        }
        if (anim)
        {
            InitializeAnimator();
        }
        _seeker = GetComponent<Seeker>();
        _lookDirection = transform.forward;
        RecalculateSpeed();
    }

    void InitializeAnimator()
    {
        _speedHash = Animator.StringToHash("Speed");
        anim.logWarnings = false;
    }

    void SetAnimatorSpeed(float normalizedValue)
    {
        if (anim)
        {
            anim.SetFloat(_speedHash, normalizedValue);
        }
    }

    public void TriggerAnimator(string triggerID)
    {
        if (!anim) return;
        anim.SetTrigger(triggerID);
    }

    public void StopMovement()
    {
        _movementType = MoveTypes.Direct;
        _pathToTarget = new List<Vector3> {transform.position};
        _moveResult = MoveResult.ReachedTarget;
        SetAnimatorSpeed(NotMoving);
    }

    /// <summary>
    /// Uses pathfinding to reach the target
    /// </summary>
    public MoveResult MoveTo(Vector3 position)
    {
        _movementType = MoveTypes.Pathing;
        if (_lastRequestedMoveTargetPosition != position)
        {
            _pathToTarget.Clear();
            _moveResult = MoveResult.Preparing;
            _lastRequestedMoveTargetPosition = position;
            if (!RequestPath(_lastRequestedMoveTargetPosition))
            {
				_lastRequestedMoveTargetPosition = transform.position;
            }
        }
        return _moveResult;
    }

    public MoveResult FleeFrom(Vector3 position)
    {
        _movementType = MoveTypes.FleePathing;
        if (_lastRequestedMoveTargetPosition != position)
        {
            _pathToTarget.Clear();
            _moveResult = MoveResult.Preparing;
            _lastRequestedMoveTargetPosition = position;
            if (!RequestFleePath(position))
            {
				_lastRequestedMoveTargetPosition = transform.position;
            }
        }
        return _moveResult;
    }

    /// <summary>
    /// ignores pathfinding to reach the target
    /// </summary>
    public MoveResult MoveToDirect(Vector3 position)
    {
        _movementType = MoveTypes.Direct;
        if (_lastRequestedMoveTargetPosition != position)
        {
            _moveResult = MoveResult.Moving;
            _pathToTarget = new List<Vector3> {position};
            _lastRequestedMoveTargetPosition = position;
        }
        return _moveResult;
    }

    /// <summary>
    /// only usable when not moving
    /// </summary>
    public void LookAt(Vector3 position)
    {
        _lookDirection = (position - transform.position).normalized;
    }

    bool RequestPath(Vector3 position)
    {
        if (_movementType != MoveTypes.Pathing)
        {
            return false;
        }
        if (!_seeker.IsDone())
        {
            return false;
        }
        _moveResult = MoveResult.Preparing;
        _seeker.StartPath(transform.position, position, OnPathCompleted);
        return true;
    }

    bool RequestFleePath(Vector3 fromPosition)
    {
        if (_movementType != MoveTypes.FleePathing)
        {
            return false;
        }
        if (!_seeker.IsDone())
        {
            return false;
        }
        _moveResult = MoveResult.Preparing;
        var fp = FleePath.Construct(transform.position, fromPosition, 2000);
        fp.aim = (transform.position - fromPosition)*10;
        fp.aimStrength = 5f;
        _seeker.StartPath(fp, OnPathCompleted);
        return true;
    }

    void OnPathCompleted(Path p)
    {
        if (_movementType != MoveTypes.Pathing && _movementType != MoveTypes.FleePathing)
        {
            return;
        }
        if (p.error)
        {
            _pathToTarget.Clear();
            _moveResult = MoveResult.TargetNotReachable;
            SetAnimatorSpeed(NotMoving);
            return;
        }
        switch (extraPathSmoothing)
        {
            default:
                _pathToTarget = p.vectorPath;
                break;
            case MovePathSmoothing.Force:
                _pathToTarget = GameWorld.Instance.SmoothPath(p.vectorPath, false);
                break;
            case MovePathSmoothing.LikeSlime:
                _pathToTarget = GameWorld.Instance.SmoothPath(p.vectorPath, true);
                break;
        }
    }

    public void UpdateMovement(float deltaTime)
    {
        TimeOutSpeedmods();
        if (Time.time - _lastPathRequestTime > RepathRate)
        {
            if (_movementType == MoveTypes.Pathing)
            {
                if (RequestPath(_lastRequestedMoveTargetPosition))
                {
                    _lastPathRequestTime = Time.time;
                }
            }
            if (_movementType == MoveTypes.FleePathing)
            {
                if (RequestFleePath(_lastRequestedMoveTargetPosition))
                {
                    _lastPathRequestTime = Time.time;
                }
            }
        }
        if (_pathToTarget.Count > 0)
        {
            SetAnimatorSpeed(Moving);
            if (AstarMath.SqrMagnitudeXZ(_pathToTarget[0], transform.position) <= TargetReachDistance)
            {
                _pathToTarget.RemoveAt(0);
                if (_pathToTarget.Count == 0)
                {
                    _moveResult = MoveResult.ReachedTarget;
                    SetAnimatorSpeed(NotMoving);
                    return;
                }
                LookAt(_pathToTarget[0]);
            }
            if (_pathToTarget.Count > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, _pathToTarget[0], deltaTime*_effectiveMovespeed);
                _moveResult = MoveResult.Moving;
            }
        }
        if (_lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_lookDirection), deltaTime/turnSpeed);
        }
    }
}
