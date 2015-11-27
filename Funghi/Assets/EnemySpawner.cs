using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner: MonoBehaviour
{
    [Serializable]
    public class SpawnConfig
    {
        public float startTime;
        public int amount;
    }

    public List<SpawnConfig> spawns = new List<SpawnConfig>();

    void Update()
    {
        for (int i = 0; i < spawns.Count; i++)
        {

        }
    }
}

