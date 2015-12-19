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
            if (cancel) { yield break; }
            if (prefab == null) { Debug.LogError("human prefab not assigned"); yield break; }
            worker.Restart();
            worker.ProcessNext(Instantiate(prefab, worker.source.transform.position, Quaternion.identity) as Entity);
            yield return new WaitForSeconds(timeInterval);
            goto RESTART;
        }

        public void Stop()
        {
            cancel = true;
        }
    }
}
