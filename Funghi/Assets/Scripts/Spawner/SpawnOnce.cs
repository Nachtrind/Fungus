using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Spawns one human after given [delay]")]
    public class SpawnOnce : SpawnModule
    {
        public float delay = 0f;
        public override void Apply(Entity e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }

        IEnumerator Execute(ModuleWorker worker)
        {
            worker.Restart();
            yield return new WaitForSeconds(delay);
            if (prefab != null)
            {
                worker.ProcessNext(Instantiate(prefab, worker.source.transform.position, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up)) as Entity);
            }
        }
    }
}