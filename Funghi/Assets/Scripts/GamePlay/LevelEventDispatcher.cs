using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum LevelEventType
{
    Start,
    Goal,
    AbilityAcquired,
    Death
}

public delegate void LevelEventCallbackDelegate();

public class LevelEventDispatcher : MonoBehaviour 
{
    [Header("Invoked immediately")]
    [SerializeField]
    UnityEvent OnStart;
    /// <summary>
    /// time for animations, sound and similar
    /// </summary>
    [SerializeField]
    ParameterEvent OnReachedGoalDirect;
    [SerializeField]
    UnityEvent OnAcquiredAbility;
    /// <summary>
    /// time for animations, sound and similar
    /// </summary>
    [SerializeField]
    ParameterEvent OnDeadDirect;
    [Header("Or immediately if these are empty")]
    [Header("Invoked on callbacks from [x]Direct events")]
    [SerializeField]
    UnityEvent OnReachedGoalCallback;
    [SerializeField]
    UnityEvent OnDeadCallback;

    public float delayDebugAutoCallback;

    [System.Serializable]
    public class ParameterEvent: UnityEvent<LevelEventCallbackDelegate> { }

    public void FireEvent(LevelEventType type)
    {
        switch (type)
        {
            case LevelEventType.Start:
                OnStart.Invoke();
                break;
            case LevelEventType.Goal:
                if (OnReachedGoalDirect.GetPersistentEventCount() == 0)
                {
                    OnReachedGoalCallback.Invoke();
                }
                else
                {
                    OnReachedGoalDirect.Invoke(DelayedCallbackAfterGoal);
                }
                break;
            case LevelEventType.AbilityAcquired:
                OnAcquiredAbility.Invoke();
                break;
            case LevelEventType.Death:
                if (OnDeadDirect.GetPersistentEventCount() == 0)
                {
                    OnDeadCallback.Invoke();
                }
                else
                {
                    OnDeadDirect.Invoke(DelayedCallbackDead);
                }
                break;
        }
    }

    public void DebugAutoCallbackDelayed(LevelEventCallbackDelegate callback)
    {
        StartCoroutine(DelayCallbackRoutine(callback));
    }

    IEnumerator DelayCallbackRoutine(LevelEventCallbackDelegate callback)
    {
        yield return new WaitForSeconds(delayDebugAutoCallback);
        Debug.Log("Animations/Sound finished");
        callback();
    }

    void DelayedCallbackAfterGoal()
    {
        OnReachedGoalCallback.Invoke();
    }

    void DelayedCallbackDead()
    {
        OnDeadCallback.Invoke();
    }
}
