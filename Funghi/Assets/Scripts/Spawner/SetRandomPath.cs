using NPCBehaviours;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Choses a random path out of the provided ones to pass to the given behaviour the spawned enemy will be set to")]
    public class SetRandomPath : BehaviourModule
    {
        public NPCBehaviour behaviour;
        public List<PatrolPath> paths = new List<PatrolPath>();
        public override void Apply(Enemy e, ModuleWorker worker)
        {
            EnterWorldBehaviour pathBehaviour = e.Behaviour as EnterWorldBehaviour;
            if (pathBehaviour)
            {
                if (paths.Count > 0)
                {
                    pathBehaviour.afterSpawnPath = paths[Random.Range(0, paths.Count - 1)];
                }
                pathBehaviour.afterPathBehaviour = behaviour;
            }
            else
            {
                if (paths.Count > 0)
                {
                    e.SetBehaviour(behaviour).path = paths[Random.Range(0, paths.Count - 1)];
                }
            }
            worker.ProcessNext(e);
        }
    }
}