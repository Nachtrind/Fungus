using System;
using System.Collections;
using UnityEngine;

namespace Spawner.Modules
{
    public abstract class SpawnModule : BaseModule
    {
        public Enemy enemyPrefab;
        public virtual void DrawGizmos(Vector3 center) { }
    }
}