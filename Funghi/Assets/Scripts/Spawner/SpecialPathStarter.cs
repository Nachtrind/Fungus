namespace Spawner.Modules
{
    public class SpecialPathStarter : BaseModule
    {
        public override void Apply(Entity e, ModuleWorker worker)
        {
            if (worker.linkedSpawnPath != null)
            {
                if (e.Behaviour)
                {
                    e.Behaviour.TryExecuteTrigger(worker.pathStartIdentifier, worker.linkedSpawnPath);
                }
            }
            worker.ProcessNext(e);
        }
    }
}