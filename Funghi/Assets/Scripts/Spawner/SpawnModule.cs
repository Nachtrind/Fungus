using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spawner.Modules
{
    public abstract class SpawnModule : BaseModule
    {
        public Entity prefab;
        public virtual void DrawGizmos(Vector3 center) { }
    }
}