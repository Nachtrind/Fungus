using System;
using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Spawns one enemy after given [delay]")]
    public class SpawnOnce : SpawnModule
    {
        public float delay = 0f;
        public override void Apply(Enemy e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }

        IEnumerator Execute(ModuleWorker worker)
        {
            yield return new WaitForSeconds(delay);
            if (enemyPrefab != null)
            {
                worker.ProcessNext(Instantiate(enemyPrefab, worker.source.transform.position, Quaternion.identity) as Enemy);
            }
        }
    }
}