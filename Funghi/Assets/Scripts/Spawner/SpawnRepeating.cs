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
        public override void Apply(Entity e, ModuleWorker worker)
        {
            worker.source.StartCoroutine(Execute(worker));
        }

        IEnumerator Execute(ModuleWorker worker)
        {
            if (started) { yield break; }
            started = true;
            yield return new WaitForSeconds(delay);
        RESTART:
            if (prefab == null) { Debug.LogError("human prefab not assigned"); yield break; }
            if (worker.markedForDeletion) { yield break; }
            while (GameWorld.Instance.IsPaused) yield return null;
            worker.Restart();
            worker.ProcessNext(Instantiate(prefab, worker.source.transform.position, Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up)) as Entity);
            yield return new WaitForSeconds(timeInterval);
            goto RESTART;
        }

    }
}
