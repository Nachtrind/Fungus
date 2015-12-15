using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spawner.Modules
{
    public abstract class SpawnModule : BaseModule
    {
        [FormerlySerializedAs("enemyPrefab")]
        public Human humanPrefab;
        public virtual void DrawGizmos(Vector3 center) { }
    }
}