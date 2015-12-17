using ModularBehaviour;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Choses a random behaviour out of the provided ones to apply to the spawned human and passes it the given path")]
    public class SetRandomBehaviour : BehaviourModule
    {
        public List<Intelligence> behaviours = new List<Intelligence>();
        public PatrolPath path;
        public override void Apply(Human e, ModuleWorker worker)
        {
            if (behaviours.Count == 0) { throw new System.Exception("No behaviours to apply!"); }
            Intelligence behaviour = behaviours[Random.Range(0, behaviours.Count - 1)];
            if (e.SetBehaviour(behaviour))
            {
                if (path != null)
                {
                    e.Behaviour.Store(Intelligence.PathIdentifier, path);
                }
                if (worker.linkedSpawnPath != null)
                {
                    e.Behaviour.Store(Intelligence.SpecialPathIdentifier, worker.linkedSpawnPath);
                }
            }
            worker.ProcessNext(e);
        }
    }
}