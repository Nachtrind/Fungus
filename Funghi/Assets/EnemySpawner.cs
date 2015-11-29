using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner: MonoBehaviour
{

    public float spawnRadius = 0.2f;

    [Serializable]
    public class SpawnConfig
    {
        public float spawnStart;
        public float interval = 10f;
        public int count = 1;
        public float lastSpawn;
        public int amount = 1;
    }

    public List<SpawnConfig> spawns = new List<SpawnConfig>();

    void Update()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            SpawnConfig current = spawns[i];
            if (current.spawnStart < GameWorld.LevelTime)
            {
                continue;
            }
            if (GameWorld.LevelTime-current.lastSpawn < current.interval)
            {
                continue;
            }
            if (current.count > 0 | current.count == -1)
            {
                Spawn();
                if (current.count > 0)
                {
                    current.count -= 1;
                }
            }
            current.lastSpawn = GameWorld.LevelTime;
        }
    }

    public void Spawn()
    {
        Vector2 prePoint = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        GameWorld.Instance.SpawnEnemy(new Vector3(transform.position.x + prePoint.x, 0, transform.position.z + prePoint.y));
    }
}

