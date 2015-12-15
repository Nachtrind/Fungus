using Spawner;
using UnityEngine;
using Spawner.Modules;

public class EnemySpawner: MonoBehaviour
{
    public bool autoActivateOnStart = true;

    #region Settings
    [SerializeField, HideInInspector]
    SpawnModule spawnModule;
    [SerializeField, HideInInspector]
    PositionModule positionModule;
    [SerializeField, HideInInspector]
    BehaviourModule behaviourModule;
    [SerializeField, HideInInspector]
    EventModule eventModule;
    #endregion

    void Start()
    {
        if (autoActivateOnStart)
        {
            Activate();
        }
    }

    void Activate()
    {
        if (spawnModule == null) { Debug.LogError("Spawner is missing configuration"); return; }
        ModuleWorker worker = new ModuleWorker(this);
        if (eventModule != null && eventModule.BeforeSpawn) { worker.steps.Add(eventModule.Apply); }
        worker.steps.Add(spawnModule.Apply);
        if (positionModule != null) { worker.steps.Add(positionModule.Apply); }
        if (behaviourModule != null) { worker.steps.Add(behaviourModule.Apply); }
        if (eventModule != null && !eventModule.BeforeSpawn) { worker.steps.Add(eventModule.Apply); }
        worker.ProcessNext(null);
    }

    Enemy CreateEmptyEnemy(Enemy e)
    {
        return Instantiate(e, transform.position, Quaternion.identity) as Enemy;
    }

    public void CancelRepeating()
    {
        if (spawnModule != null)
        {
            SpawnRepeating sr = spawnModule as SpawnRepeating;
            if (sr) { sr.Stop(); }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        if (spawnModule != null)
        {
            spawnModule.DrawGizmos(transform.position);
        }
        if (positionModule != null)
        {
            positionModule.DrawGizmos(transform.position);
        }
        if (behaviourModule != null)
        {
            behaviourModule.DrawGizmos(transform.position);
        }
    }
}

