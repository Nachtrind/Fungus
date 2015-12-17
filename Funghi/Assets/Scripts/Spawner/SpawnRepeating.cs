using System;
using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Spawns endless enemies, after [delay] in [timeInterval] seconds. Call HumanSpawner.CancelRepeating() if stop is required")]
    public class SpawnRepeating : SpawnModule
    {
        public float delay = 0f;
        public float timeInterval = 1f;
        bool started = false;
        bool cancel = false;
        public override void Apply(Human e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }

        IEnumerator Execute(ModuleWorker worker)
        {
            if (started) { yield break; }
            started = true;
            yield return new WaitForSeconds(delay);
        RESTART:
            if (cancel) { yield break; }
            if (humanPrefab == null) { Debug.LogError("human prefab not assigned"); yield break; }
            worker.ProcessNext(Instantiate(humanPrefab, worker.source.transform.position, Quaternion.identity) as Human);
            yield return new WaitForSeconds(timeInterval);
            goto RESTART;
        }

        public void Stop()
        {
            cancel = true;
        }
    }
}
