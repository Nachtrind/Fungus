using UnityEngine;

namespace NPCBehaviours
{

    public abstract class NPCBehaviour: ScriptableObject
    {
        [HideInInspector]
        public PatrolPath path;

        [HideInInspector]
        public bool isInstantiated = false;

        public abstract void Evaluate(Human owner, float deltaTime);
        public virtual void OnReceivedBroadcastMessage(Message message) { }
        public virtual void DrawGizmos(Human owner) { }
#if UNITY_EDITOR
        public virtual void DrawDebugInfos(Human owner) { }
#endif
        public virtual void Cleanup(Human owner) { }
    }
}
