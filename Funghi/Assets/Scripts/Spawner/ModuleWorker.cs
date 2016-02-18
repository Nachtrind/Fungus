using System;
using System.Collections.Generic;

namespace Spawner
{
    public class ModuleWorker
    {
        public readonly EntitySpawner source;
        public List<Action<Entity, ModuleWorker>> steps = new List<Action<Entity, ModuleWorker>>();
        Action<Entity> SpawnCompletedCallback;
        int currentStep = 0;
        public PatrolPath linkedSpawnPath;
        public string pathStartIdentifier = "EnterWorld";
        public bool markedForDeletion;
        public ModuleWorker(EntitySpawner spawner, Action<Entity> completedCallback)
        {
            source = spawner;
            SpawnCompletedCallback = completedCallback;
        }
        public void Restart()
        {
            currentStep = 1;
            linkedSpawnPath = null;
        }
        public void ProcessNext(Entity e)
        {
            if (currentStep < steps.Count)
            {
                Action<Entity, ModuleWorker> next = steps[currentStep];
                currentStep++;
                next(e, this);
            }
            else
            {
                if (SpawnCompletedCallback != null)
                {
                    SpawnCompletedCallback(e);
                }
                if (!markedForDeletion)
                {
                    Restart();
                }
            }
        }
    }
}