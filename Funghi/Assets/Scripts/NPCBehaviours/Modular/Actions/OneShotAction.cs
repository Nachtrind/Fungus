using System;
using UnityEngine;

namespace ModularBehaviour
{
    public abstract class OneShotAction: ScriptableObject
    {
        public abstract ActionResult Fire(IntelligenceController controller);
        public virtual void DrawGUI(IntelligenceState parentState, Intelligence intelligence, CallbackCollection callbacks) { }
        public virtual void OnDelete(CallbackCollection callbacks) { }
    }

}
