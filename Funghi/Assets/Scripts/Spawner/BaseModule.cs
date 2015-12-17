using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Spawner.Modules
{
    public abstract class BaseModule: ScriptableObject
    {
        public abstract void Apply(Human e, ModuleWorker worker);
    }

}
