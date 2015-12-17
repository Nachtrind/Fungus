using System;
using UnityEngine;

namespace ModularBehaviour
{
    public abstract class AIAction: ScriptableObject
    {
        public abstract ActionResult Run(IntelligenceController controller, float deltaTime);
        public virtual void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks) { }
        public virtual void OnDelete(CallbackCollection callbacks) { }
    }

}
