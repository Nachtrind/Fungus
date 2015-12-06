using UnityEngine;

namespace Spawner.Modules
{
    [ModuleDescription("Calls [functionToCall] on the target at the specified time (call On Start = before spawn) \nThe function must have no parameters")]
    public class RunEvent : EventModule
    {
        public GameObject target;
        public string functionToCall = "";
        public bool callOnStart = true;
        public override bool BeforeSpawn
        {
            get
            {
                return callOnStart;
            }
        }

        public override void Apply(Enemy e, ModuleWorker worker)
        {
            if (target)
            {
                target.BroadcastMessage(functionToCall, SendMessageOptions.RequireReceiver);
            }
            worker.ProcessNext(e);
        }
    }
}