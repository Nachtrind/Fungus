using System;
using System.Collections.Generic;
using NodeAbilities;
using UnityEngine;

namespace Tutorials
{
    public class Tutorial: MonoBehaviour
    {
        public RectTransform UIAnchor;
        public List<TutorialAction> EventActions = new List<TutorialAction>();
        List<TutorialAction> actionBackup = new List<TutorialAction>(); 

        public enum TutorialEventType
        {
            SpawnerSpawned,
            HumanDamaged,
            ResourceGained,
            AbilityGained,
            PoliceAlarmed,
            CitizenPanic, 
            NodeDestroyed,
            CoreDied
        }

        void Awake()
        {
            if (EventActions.Count == 0)
            {
                Debug.Log("No Tutorial actions specified. Disabling Tutorial.");
                Destroy(gameObject);
            }
            if (UIAnchor == null)
            {
                Debug.Log("No UI anchor specified. Disabling Tutorial to prevent Exceptions");
                Destroy(gameObject);
            }
            actionBackup.Clear();
            for (var i = 0; i < EventActions.Count; i++)
            {
                actionBackup.Add(Instantiate(EventActions[i]));
            }
            RegisterHooks();
        }

        public void Reset()
        {
            for (var i = 0; i < EventActions.Count; i++)
            {
                EventActions[i].OnCleanup(this);
            }
            EventActions.Clear();
            UnregisterHooks();
            for (var i = 0; i < actionBackup.Count; i++)
            {
                EventActions.Add(Instantiate(actionBackup[i]));
            }
            policeAlarmHandled = false;
            citizenPanicHandled = false;
            RegisterHooks();
        }

        void OnDestroy()
        {
            UnregisterHooks();
        }

        public class HookEventInfo
        {
            public TutorialEventType type;
            public Entity LinkedEntity;
            public NodeAbility LinkedAbility;
            public int linkedValue;
            public string tag;
            public EntitySpawner LinkedSpawner;
        }

#region Hooks

        void RegisterHooks()
        {
            EntitySpawner.OnSpawn += OnSpawnerSpawned;
            Entity.OnDamaged += OnEntityDamaged;
            FungusResources.OnResourceGain += OnResourceGained;
            AbilityBuilding.OnAbilityGained += OnAbilityGained;
            GameWorld.OnMessageBroadcast += DispatchMessageType;
            GameWorld.OnNodeDestroyed += OnNodeDestroyed;
            GameWorld.OnCoreKilled += OnCoreKilled;
        }

        void UnregisterHooks()
        {
            EntitySpawner.OnSpawn -= OnSpawnerSpawned;
            Entity.OnDamaged -= OnEntityDamaged;
            FungusResources.OnResourceGain -= OnResourceGained;
            AbilityBuilding.OnAbilityGained -= OnAbilityGained;
            GameWorld.OnMessageBroadcast -= DispatchMessageType;
            GameWorld.OnNodeDestroyed -= OnNodeDestroyed;
            GameWorld.OnCoreKilled -= OnCoreKilled;
        }

        bool policeAlarmHandled = false;
        bool citizenPanicHandled = false; 
        void DispatchMessageType(Message m)
        {
            switch (m.type)
            {
                case NotificationType.PoliceAlarm:
                    if (policeAlarmHandled) break;
                    if (OnPoliceAlarmed(m.sender))
                    {
                        policeAlarmHandled = true;
                    }
                    break;
                case NotificationType.FungusInSight:
                    if (citizenPanicHandled) break;
                    if (OnCitizenPanic(m.sender))
                    {
                        citizenPanicHandled = true;
                    }
                    break;
            }
            if (policeAlarmHandled && citizenPanicHandled)
            {
                GameWorld.OnMessageBroadcast -= DispatchMessageType;
            }
        }

        void Update()
        {
            for (var i = EventActions.Count; i-- > 0;)
            {
                if (EventActions[i].Execute(this, Time.realtimeSinceStartup)) continue;
                EventActions[i].OnCleanup(this);
                EventActions[i].CachedEventInfo = null;
                EventActions.RemoveAt(i);
            }
        }

        bool CallActions(HookEventInfo info)
        {
            for (var i = EventActions.Count; i-->0;)
            {
                if (EventActions[i].Event != info.type) continue;
                if (EventActions[i].NeedsTag & EventActions[i].RequiredTag.Equals(info.tag, StringComparison.OrdinalIgnoreCase)) continue;
                EventActions[i].CachedEventInfo = info;
                EventActions[i].OnInitialize(this, Time.realtimeSinceStartup);
                return true;
            }
            return false;
        }

        void OnSpawnerSpawned(Entity e, EntitySpawner spawner, string tutorialtag)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() {LinkedEntity = e, LinkedSpawner = spawner, tag = tutorialtag, type = TutorialEventType.SpawnerSpawned};
            if (CallActions(he))
            {
                EntitySpawner.OnSpawn -= OnSpawnerSpawned;
            }
        }

        void OnEntityDamaged(Entity e)
        {
            if (EventActions.Count == 0) return;
            if (!(e is Human)) return;
            var he = new HookEventInfo() {LinkedEntity = e, type = TutorialEventType.HumanDamaged};
            if (CallActions(he))
            {
                Entity.OnDamaged -= OnEntityDamaged;
            }
        }

        void OnResourceGained()
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { type = TutorialEventType.ResourceGained };
            if (CallActions(he))
            {
                FungusResources.OnResourceGain -= OnResourceGained;
            }
        }

        void OnAbilityGained(NodeAbility ability)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedAbility = ability, type = TutorialEventType.AbilityGained };
            if (CallActions(he))
            {
                AbilityBuilding.OnAbilityGained -= OnAbilityGained;
            }
        }

        bool OnPoliceAlarmed(Entity e)
        {
            if (EventActions.Count == 0) return false;
            var he = new HookEventInfo() { LinkedEntity = e, type = TutorialEventType.PoliceAlarmed };
            return CallActions(he);
        }

        bool OnCitizenPanic(Entity e)
        {
            if (EventActions.Count == 0) return false;
            var he = new HookEventInfo() { LinkedEntity = e, type = TutorialEventType.CitizenPanic };
            return CallActions(he);
        }

        void OnNodeDestroyed(Entity e)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedEntity = e, type = TutorialEventType.NodeDestroyed };
            if (CallActions(he))
            {
                GameWorld.OnNodeDestroyed -= OnNodeDestroyed;
            }
        }

        void OnCoreKilled(Entity e)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedEntity = e, type = TutorialEventType.CoreDied };
            if (CallActions(he))
            {
                GameWorld.OnCoreKilled -= OnCoreKilled;
            }
        }
#endregion
    }
}
