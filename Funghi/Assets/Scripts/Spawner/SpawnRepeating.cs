using System;
using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Spawns endless enemies, after [delay] in [timeInterval] seconds. Call EnemySpawner.CancelRepeating() if stop is required")]
    public class SpawnRepeating : SpawnModule
    {
        public float delay = 0f;
        public float timeInterval = 1f;
        bool started = false;
        bool cancel = false;
        public override void Apply(Enemy e, ModuleWorker worker)
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
            if (enemyPrefab == null) { Debug.LogError("enemy prefab not assigned"); yield break; }
            if (enemyPrefab != null)
            {
                worker.ProcessNext(Instantiate(enemyPrefab, worker.source.transform.position, Quaternion.identity) as Enemy);
            }
            yield return new WaitForSeconds(timeInterval);
            goto RESTART;
        }

        public void Stop()
        {
            cancel = true;
        }
    }
}
