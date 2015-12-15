using System;
using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Spawns one human after given [delay]")]
    public class SpawnOnce : SpawnModule
    {
        public float delay = 0f;
        public override void Apply(Human e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }

        IEnumerator Execute(ModuleWorker worker)
        {
            yield return new WaitForSeconds(delay);
            if (humanPrefab != null)
            {
                worker.ProcessNext(Instantiate(humanPrefab, worker.source.transform.position, Quaternion.identity) as Human);
            }
        }
    }
}