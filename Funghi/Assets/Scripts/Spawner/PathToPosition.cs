using NPCBehaviours;
using System;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Makes the Human walk the path to enter the world (out of a garden for example)")]
    public class PathToPosition : PositionModule
    {
        public PatrolPath path;
        public bool invulnerableUntilTargetReached = false;
        public override void Apply(Human e, ModuleWorker worker)
        {
            if (path == null) { throw new ArgumentException("Path invalid"); }
            EnterWorldBehaviour ewb = e.SetBehaviour(CreateInstance<EnterWorldBehaviour>()) as EnterWorldBehaviour;
            if (path.points.Count > 0)
            {
                e.transform.position = path.points[0].position;
            }
            e.isAttackable = !invulnerableUntilTargetReached;
            ewb.path = path;
            worker.ProcessNext(e);
        }
    }
}