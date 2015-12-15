using UnityEngine;

namespace NPCBehaviours
{

    public abstract class NPCBehaviour: ScriptableObject
    {
        [HideInInspector]
        public PatrolPath path;

        [HideInInspector]
        public bool isInstantiated = false;

        public abstract void Evaluate(Enemy owner, float deltaTime);
        public virtual void OnReceivedBroadcastMessage(Message message) { }
        public virtual void DrawGizmos(Enemy owner) { }
#if UNITY_EDITOR
        public virtual void DrawDebugInfos(Enemy owner) { }
#endif
        public virtual void Cleanup(Enemy owner) { }
    }
}
