using System;
using UnityEngine;

namespace ModularBehaviour
{
    public class ClearTarget : OneShotAction
    {
        public override ActionResult Fire(IntelligenceController controller)
        {
            controller.Storage.SetParameter("target", null);
            return ActionResult.Finished;
        }
    }
}