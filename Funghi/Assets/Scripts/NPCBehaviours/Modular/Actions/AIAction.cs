using System;
using UnityEngine;
using System.Collections.Generic;

namespace ModularBehaviour
{
    public abstract class AIAction: ScriptableObject
    {
        [Flags]
        public enum UsageType
        {
            AsOneShot = 1,
            AsCondition = 2,
            AsContinuous = 4
        }
        public virtual void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks) { }
        public virtual void OnDelete(CallbackCollection callbacks) { }
        public virtual ActionResult Run(IntelligenceController controller, float deltaTime) { return ActionResult.Failed; }
        public virtual ActionResult Fire(IntelligenceController controller) { return ActionResult.Failed; }
        public virtual void DeepClone(List<Action<Func<IntelligenceState, IntelligenceState>>> stateCloneCallbacks) { }
    }

}
