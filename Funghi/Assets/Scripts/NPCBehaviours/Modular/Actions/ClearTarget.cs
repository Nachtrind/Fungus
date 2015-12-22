using System;
using UnityEngine;

namespace ModularBehaviour
{
    public class ClearTarget : OneShotAction
    {
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.SetMemoryValue("target", null);
            return ActionResult.Finished;
        }
    }
}