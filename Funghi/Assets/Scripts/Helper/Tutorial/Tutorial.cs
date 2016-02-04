using System.Collections.Generic;
using NodeAbilities;
using UnityEngine;

namespace Tutorials
{
    public class Tutorial: MonoBehaviour
    {
        public RectTransform UIAnchor;
        public List<TutorialAction> EventActions = new List<TutorialAction>(); 

        public enum HookType
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
            RegisterHooks();
        }

        void OnDestroy()
        {
            UnregisterHooks();
        }

        public class HookEventInfo
        {
            public HookType type;
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

        void DispatchMessageType(Message m)
        {
            switch (m.type)
            {
                case NotificationType.PoliceAlarm:
                    OnPoliceAlarmed(m.sender);
                    break;
                case NotificationType.FungusInSight:
                    OnCitizenPanic(m.sender);
                    break;
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

        void CallActions(HookEventInfo info)
        {
            for (var i = EventActions.Count; i-->0;)
            {
                if (EventActions[i].Hook != info.type) continue;
                EventActions[i].CachedEventInfo = info;
                EventActions[i].OnInitialize(this, Time.realtimeSinceStartup);
            }
        }

        void OnSpawnerSpawned(Entity e, EntitySpawner spawner, string tutorialtag)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() {LinkedEntity = e, LinkedSpawner = spawner, tag = tutorialtag, type = HookType.SpawnerSpawned};
            CallActions(he);
        }

        void OnEntityDamaged(Entity e)
        {
            if (EventActions.Count == 0) return;
            if (!(e is Human)) return;
            var he = new HookEventInfo() {LinkedEntity = e, type = HookType.HumanDamaged};
            CallActions(he);
        }

        void OnResourceGained()
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { type = HookType.ResourceGained };
            CallActions(he);
        }

        void OnAbilityGained(NodeAbility ability)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedAbility = ability, type = HookType.AbilityGained };
            CallActions(he);
        }

        void OnPoliceAlarmed(Entity e)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedEntity = e, type = HookType.PoliceAlarmed };
            CallActions(he);
        }

        void OnCitizenPanic(Entity e)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedEntity = e, type = HookType.CitizenPanic };
            CallActions(he);
        }

        void OnNodeDestroyed(Entity e)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedEntity = e, type = HookType.NodeDestroyed };
            CallActions(he);
        }

        void OnCoreKilled(Entity e)
        {
            if (EventActions.Count == 0) return;
            var he = new HookEventInfo() { LinkedEntity = e, type = HookType.CoreDied };
            CallActions(he);
        }
#endregion
    }
}
