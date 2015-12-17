using System;
using System.Collections.Generic;

namespace Spawner
{
    public class ModuleWorker
    {
        public readonly HumanSpawner source;
        public List<Action<Human, ModuleWorker>> steps = new List<Action<Human, ModuleWorker>>();
        int currentStep = 0;
        public PatrolPath linkedSpawnPath;
        public string pathStartIdentifier = "EnterWorld";
        public ModuleWorker(HumanSpawner spawner)
        {
            source = spawner;
        }
        public void ProcessNext(Human e)
        {
            if (currentStep < steps.Count)
            {
                Action<Human, ModuleWorker> next = steps[currentStep];
                currentStep++;
                if (currentStep >= steps.Count) { currentStep = 0; linkedSpawnPath = null; }
                next(e, this);
            }
        }
    }
}