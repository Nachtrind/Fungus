using ModularBehaviour;

namespace Spawner.Modules
{
    [ModuleDescription("Sets the enemies behaviour and path")]
    public class SetBehaviour : BehaviourModule
    {
        public Intelligence behaviour;
        public PatrolPath path;
        public override void Apply(Human e, ModuleWorker worker)
        {
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