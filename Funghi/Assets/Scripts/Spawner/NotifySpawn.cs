using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Notifies the target about the spawned Enemy \n(the function must have the signature: Function(Enemy))")]
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

        public override void Apply(Enemy e, ModuleWorker worker)
        {
            if (target)
            {
                target.SendMessage(functionToCall, e);
            }
            worker.ProcessNext(e);
        }
    }
}