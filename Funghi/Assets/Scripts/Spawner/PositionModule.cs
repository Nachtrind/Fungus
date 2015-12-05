using UnityEngine;

namespace Spawner.Modules
{
    public abstract class PositionModule : BaseModule
    {
        public virtual void DrawGizmos(Vector3 center) { }
#if UNITY_EDITOR
        public virtual void DrawHandles() { }
#endif
    }
}