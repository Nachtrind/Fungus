using NPCBehaviours;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Choses a random behaviour out of the provided ones to apply to the spawned enemy and passes it the given path")]
    public class SetRandomBehaviour : BehaviourModule
    {
        public List<NPCBehaviour> behaviours = new List<NPCBehaviour>();
        public PatrolPath path;
        public override void Apply(Enemy e, ModuleWorker worker)
        {
            EnterWorldBehaviour pathBehaviour = e.Behaviour as EnterWorldBehaviour;
            if (pathBehaviour)
            {
                pathBehaviour.afterSpawnPath = path;
                if (behaviours.Count > 0)
                {
                    pathBehaviour.afterPathBehaviour = behaviours[Random.Range(0, behaviours.Count - 1)];
                }
            }
            else
            {
                if (behaviours.Count > 0)
                {
                    e.SetBehaviour(behaviours[Random.Range(0, behaviours.Count - 1)]).path = path;
                }
            }
            worker.ProcessNext(e);
        }
    }
}