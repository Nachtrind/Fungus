using ModularBehaviour;

namespace Spawner.Modules
{
    [ModuleDescription("Sets the enemies behaviour and path")]
    public class SetBehaviour : BehaviourModule
    {
        public Intelligence behaviour;
        public PatrolPath path;
        public override void Apply(Entity e, ModuleWorker worker)
        {
            if (e.SetBehaviour(behaviour))
            {
                if (path != null)
                {
                    e.Behaviour.LoadPath(path);
                }
                if (worker.linkedSpawnPath != null)
                {
                    e.Behaviour.LoadSpecialPath(worker.linkedSpawnPath);
                }
                e.Behaviour.name = new System.Random().Next().ToString();
            }
            worker.ProcessNext(e);
        }
    }
}