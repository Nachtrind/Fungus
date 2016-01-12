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
        public override void Apply(Entity e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }
        IEnumerator Execute(ModuleWorker worker)
        {
            yield return new WaitForSeconds(delay);
            float partTime = timeSpan / Mathf.Max(1f, amount);
            for (int i = 0; i < amount; i++)
            {
                if (prefab != null)
                {
                    Entity e = Instantiate(prefab, worker.source.transform.position, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up)) as Entity;
                    e.name = e.GetInstanceID().ToString();
                    worker.ProcessNext(e);
                }
                yield return new WaitForSeconds(partTime);
                worker.Restart();
            }
        }
    }
}