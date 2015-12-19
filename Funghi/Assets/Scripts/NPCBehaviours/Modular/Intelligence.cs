using UnityEngine;
using System.Collections.Generic;

namespace ModularBehaviour
{
    public interface IntelligenceController
    {
        void ChangeState(IntelligenceState newState);
        ParameterStorage Storage { get; }
        bool IsActiveState(string stateName);
        bool WasLastState(string stateName);
        void LoadPath(PatrolPath path);
        Entity Owner { get; }
    }

    [CreateAssetMenu(menuName ="Behaviour/Intelligence", fileName ="Behaviour")]
    public class Intelligence : ScriptableObject, IntelligenceController
    {
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

        [HideInInspector]
        ParameterStorage storage = new ParameterStorage();
        ParameterStorage IntelligenceController.Storage { get { return storage; } }

        Entity owner;
        public Entity Owner { get { return owner; } }

        public void Initialize(Entity owner)
        {
            this.owner = owner;
            if (storage == null) { storage = new ParameterStorage(); }
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

        bool IntelligenceController.IsActiveState(string stateName)
        {
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

        public void TryExecuteTrigger(string trigger, object value)
        {
            for (int i = 0; i < triggers.Count; i++)
            {
                if (triggers[i].trigger.Equals(trigger, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (triggers[i].action != null)
                    {
                        triggers[i].action.Fire(this, value);
                    }
                }
            }
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

        public void Store(string name, object value)
        {
            storage.SetParameter(name, value);
        }

        public void LoadPath(PatrolPath path)
        {
            storage.SetParameter(PathIdentifier, path);
        }

        public void LoadSpecialPath(PatrolPath path)
        {
            storage.SetParameter(SpecialPathIdentifier, path);
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
            GUILayout.BeginArea(new Rect(unitPos, new Vector2(150, 25)), GUI.skin.box);
            GUILayout.Label("State: " + activeState.ToString());
            GUILayout.EndArea();
        }

        public void DrawGizmos(Entity e)
        {

        }
    }
}