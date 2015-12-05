using UnityEngine;

namespace Spawner.Modules
{
    public abstract class BehaviourModule : BaseModule
    {
        public virtual void DrawGizmos(Vector3 center) { }
    }
}