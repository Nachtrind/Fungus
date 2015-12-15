using NPCBehaviours;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Sets the enemies behaviour and path")]
    public class SetBehaviour : BehaviourModule
    {
        public NPCBehaviour behaviour;
        public PatrolPath path;
        public override void Apply(Human e, ModuleWorker worker)
        {
            EnterWorldBehaviour pathBehaviour = e.Behaviour as EnterWorldBehaviour;
            if (pathBehaviour)
            {
                pathBehaviour.afterSpawnPath = path;
                pathBehaviour.afterPathBehaviour = behaviour;
            }
            else
            {
                e.SetBehaviour(behaviour).path = path;
            }
            worker.ProcessNext(e);
        }
    }
}