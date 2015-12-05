using UnityEngine;

namespace Spawner.Modules
{
    public abstract class EventModule : BaseModule
    {
        public abstract bool BeforeSpawn { get; }
        public virtual void DrawGizmos(Vector3 center) { }
    }

}