using System;
using System.Collections.Generic;

namespace Spawner
{
    public class ModuleWorker
    {
        public readonly HumanSpawner source;
        public List<Action<Human, ModuleWorker>> steps = new List<Action<Human, ModuleWorker>>();
        public ModuleWorker(HumanSpawner spawner)
        {
            source = spawner;
        }
        public void ProcessNext(Human e)
        {
            if (steps.Count > 0)
            {
                Action<Human, ModuleWorker> next = steps[0];
                steps.RemoveAt(0);
                next(e, this);
            }
        }
    }
}