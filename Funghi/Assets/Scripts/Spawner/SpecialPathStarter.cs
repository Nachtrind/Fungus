namespace Spawner.Modules
{
    public class SpecialPathStarter : BaseModule
    {
        public override void Apply(Human e, ModuleWorker worker)
        {
            if (worker.linkedSpawnPath != null)
            {
                if (e.Behaviour)
                {
                    e.Behaviour.TryExecuteTrigger(worker.pathStartIdentifier, worker.linkedSpawnPath);
                }
            }
        }
    }
}