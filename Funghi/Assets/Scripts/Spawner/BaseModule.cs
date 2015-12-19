using UnityEngine;

namespace Spawner.Modules
{
    public abstract class BaseModule: ScriptableObject
    {
        public abstract void Apply(Entity e, ModuleWorker worker);
    }

}
