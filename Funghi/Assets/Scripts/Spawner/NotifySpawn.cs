using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Notifies the target about the spawned human \n(the function must have the signature: Function(Human))")]
    public class NotifySpawn : EventModule
    {
        public GameObject target;
        public string functionToCall = "";
        public override bool BeforeSpawn
        {
            get
            {
                return false;
            }
        }

        public override void Apply(Human e, ModuleWorker worker)
        {
            if (target)
            {
                target.SendMessage(functionToCall, e);
            }
            worker.ProcessNext(e);
        }
    }
}