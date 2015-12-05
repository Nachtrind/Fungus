using System;
using System.Collections.Generic;

namespace Spawner
{
    public class ModuleWorker
    {
        public readonly EnemySpawner source;
        public List<Action<Enemy, ModuleWorker>> steps = new List<Action<Enemy, ModuleWorker>>();
        public ModuleWorker(EnemySpawner spawner)
        {
            source = spawner;
        }
        public void ProcessNext(Enemy e)
        {
            if (steps.Count > 0)
            {
                Action<Enemy, ModuleWorker> next = steps[0];
                steps.RemoveAt(0);
                next(e, this);
            }
        }
    }
}