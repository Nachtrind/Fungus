using System;
using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Spawns multiple humans (amount) over [timeSpan] after [delay]")]
    public class SpawnOverTime : SpawnModule
    {
        public float delay = 0f;
        public float timeSpan = 2f;
        public int amount = 2;
        public override void Apply(Human e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }
        IEnumerator Execute(ModuleWorker worker)
        {
            yield return new WaitForSeconds(delay);
            float partTime = timeSpan / Mathf.Max(1f, amount);
            for (int i = 0; i < amount; i++)
            {
                if (humanPrefab != null)
                {
                    worker.ProcessNext(Instantiate(humanPrefab, worker.source.transform.position, Quaternion.identity) as Human);
                }
                yield return new WaitForSeconds(partTime);
            }
        }
    }
}