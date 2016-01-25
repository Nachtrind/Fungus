using UnityEngine;
using System.Collections.Generic;

namespace ModularBehaviour
{
    public interface IntelligenceController
    {
        void ChangeState(IntelligenceState newState);
        bool HasMemory<T>(string identifier);
        void SetMemoryValue(string identifier, object value);
        bool GetMemoryValue<T>(string identifier, out T value);
        bool IsActiveState(string stateName);
        bool WasLastState(string stateName);
        void LoadPath(PatrolPath path);
        Entity Owner { get; }
    }

    public enum IntelligenceType
    {
        Undefined,
        Human,
        Citizen,
        Police,
        PoliceCar,
        Zombie,
        Infected,
        FungusNode,
        FungusCore
    }

    [CreateAssetMenu(menuName ="Behaviour/Intelligence", fileName ="Behaviour")]
    public class Intelligence : ScriptableObject, IntelligenceController
    {

        public IntelligenceType Classification;

        #region const
        public const string PathIdentifier = "Path";
        public const string PathIndexIdentifier = "PathIndex";
        public const string SpecialPathIdentifier = "SpecialPath";
        public const string SpecialPathIndexIdentifier = "SpecialPathIndex";
        #endregion

        [HideInInspector]
        public List<IntelligenceState> states = new List<IntelligenceState>();

        [HideInInspector, SerializeField]
        public List<ActionTrigger> triggers = new List<ActionTrigger>();

        Entity owner;
        public Entity Owner { get { return owner; } }

        public void Initialize(Entity owner)
        {
            this.owner = owner;
            DeepClone();
        }

        [System.Serializable]
        public class ActionTrigger
        {
            public string trigger = "";
            public TriggerAction action;
            public ActionTrigger(string trigger)
            {
                this.trigger = trigger;
            }
        }

        IntelligenceState lastState;
        [SerializeField, HideInInspector]
        IntelligenceState activeState;

        bool initialized = false;

        IntelligenceState GetStateFromCloned(IntelligenceState unClonedState)
        {
            if (unClonedState == null) { return null; }
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].name.Equals(unClonedState.name, System.StringComparison.OrdinalIgnoreCase) | states[i].name.Equals(unClonedState.name+"(Clone)", System.StringComparison. OrdinalIgnoreCase))
                {
                    return states[i];
                }
            }
            return activeState;
        }

        public bool IsActiveState(string stateName)
        {
            if (activeState == null) { return false; }
            return activeState.name.Equals(stateName, System.StringComparison.OrdinalIgnoreCase);
        }

        bool IntelligenceController.WasLastState(string stateName)
        {
            if (lastState == null) { return false; }
            return lastState.name.Equals(stateName, System.StringComparison.OrdinalIgnoreCase);
        }

        void IntelligenceController.ChangeState(IntelligenceState newState)
        {
            if (newState != activeState)
            {
                if (activeState != null)
                {
                    activeState.OnExit(this);
                }
                lastState = activeState;
                activeState = newState;
                initialized = false;
            }
        }

        public bool TryExecuteTrigger(string trigger, object value)
        {
            bool success = false;
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].trigger.Equals(trigger, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (triggers[i].action != null)
                    {
                        if (triggers[i].action.Fire(this, value) == ActionResult.Success)
                        {
                            success = true;
                        }
                    }
                }
            }
            return success;
        }

        public void HandleMessageBroadcast(Message m)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].action != null && triggers[i].action is HandleMessage)
                {
                    triggers[i].action.Fire(this, m);
                }
            }
        }

        #region Memory
        Dictionary<string, object> memory = new Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);

        public void SetMemoryValue(string name, object value)
        {
            if (memory.ContainsKey(name))
            {
                memory[name] = value;
            }
            else
            {
                memory.Add(name, value);
            }
        }

        public bool HasMemory<T>(string name)
        {
            if (memory.ContainsKey(name) && memory[name] is T && !memory[name].Equals(default(T)))
            {
                return true;
            }
            return false;
        }

        public bool GetMemoryValue<T>(string name, out T value)
        {
            object existing;
            if (memory.TryGetValue(name, out existing))
            {
                if (existing is T && !existing.Equals(default(T)))
                {
                    value = (T)(existing);
                    return true;
                }
                value = default(T);
                return false;
            }
            value = default(T);
            return false;
        }

        #endregion

        public void LoadPath(PatrolPath path)
        {
            SetMemoryValue(PathIdentifier, path);
            if (path)
            {
                SetMemoryValue(PathIndexIdentifier, path.GetNearestPatrolPointIndex(owner.transform.position));
            }
        }

        public void LoadSpecialPath(PatrolPath path)
        {
            SetMemoryValue(SpecialPathIdentifier, path);
            if (path)
            {
                SetMemoryValue(SpecialPathIndexIdentifier, path.GetNearestPatrolPointIndex(owner.transform.position));
            }
        }

        public void UpdateTick(float delta)
        {
            if (activeState)
            {
                if (!initialized)
                {
                    initialized = true;
                    activeState.OnEnter(this);
                }
                else
                {
                    activeState.OnUpdate(this, delta);
                }
            }
        }

        public void DrawDebugInfos(Entity e)
        {
            Vector2 unitPos = Camera.main.WorldToScreenPoint(owner.transform.position);
            unitPos.y = Screen.height - unitPos.y;
            string txt = "State: " + activeState.name;
            Vector2 gSize = GUI.skin.box.CalcSize(new GUIContent(txt));
            GUILayout.BeginArea(new Rect(unitPos, new Vector2(gSize.x, gSize.y)), GUI.skin.box);
            GUILayout.Label(txt);
            GUILayout.EndArea();
        }

        public void DrawGizmos(Entity e)
        {

        }

        void DeepClone()
        {
            var stateCloneCallbacks = new List<System.Action<System.Func<IntelligenceState, IntelligenceState>>>();
            for (int i = 0; i < states.Count; i++)
            {
                bool isStartState = states[i] == activeState;
                states[i] = Instantiate(states[i]);
                if (isStartState) { activeState = states[i]; }
                states[i].DeepClone(stateCloneCallbacks);
            }
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].action)
                {
                    triggers[i].action = Instantiate(triggers[i].action);
                    triggers[i].action.DeepClone(stateCloneCallbacks);
                }
            }
            for (int i = 0; i < stateCloneCallbacks.Count; i++)
            {
                stateCloneCallbacks[i](GetStateFromCloned);
            }
        }
    }
}