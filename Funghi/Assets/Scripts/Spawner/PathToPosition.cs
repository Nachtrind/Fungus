using System;

namespace Spawner.Modules
{
    [ModuleDescription("Makes the Human walk the path to enter the world (out of a garden for example)")]
    public class PathToPosition : PositionModule
    {
        public PatrolPath path;
        public string triggerIdentifier = "EnterWorld";
        public override void Apply(Human e, ModuleWorker worker)
        {
            if (path == null) { throw new ArgumentException("Path invalid"); }
            worker.linkedSpawnPath = path;
            worker.pathStartIdentifier = triggerIdentifier;
            if (path.points.Count > 0)
            {
                e.transform.position = path.points[0].position;
            }
            worker.ProcessNext(e);
        }
    }
}