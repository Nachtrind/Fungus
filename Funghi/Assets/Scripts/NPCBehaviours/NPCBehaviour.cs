using UnityEngine;

namespace NPCBehaviours
{

    public abstract class NPCBehaviour: ScriptableObject
    {
        public PatrolPath path;

        [HideInInspector]
        public bool isInstantiated = false;

        public abstract void Evaluate(IBehaviourControllable owner, float deltaTime);
        public virtual void OnReceivedBroadcastMessage(Message message) { }
    }
}
