using ModularBehaviour;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Choses a random path out of the provided ones to pass to the given behaviour the spawned human will be set to")]
    public class SetRandomPath : BehaviourModule
    {
        public Intelligence behaviour;
        public List<PatrolPath> paths = new List<PatrolPath>();
        public override void Apply(Entity e, ModuleWorker worker)
        {
            if (e.SetBehaviour(behaviour))
            {
                if (paths.Count > 0)
                {
                    PatrolPath path = paths[Random.Range(0, paths.Count - 1)];
                    e.Behaviour.LoadPath(path);
                }
                if (worker.linkedSpawnPath != null)
                {
                    e.Behaviour.LoadSpecialPath(worker.linkedSpawnPath);
                }
            }
            worker.ProcessNext(e);
        }
    }
}