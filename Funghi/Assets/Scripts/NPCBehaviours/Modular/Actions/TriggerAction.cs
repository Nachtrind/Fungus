using System;
using UnityEngine;
using System.Collections.Generic;

namespace ModularBehaviour
{
    public abstract class TriggerAction : ScriptableObject
    {
        public abstract ActionResult Fire(IntelligenceController controller, object value = null);
        public virtual void DrawGUI(Intelligence intelligence, CallbackCollection callbacks) { }
        public virtual void OnDelete(CallbackCollection callbacks) { }
        public virtual void DeepClone(List<Action<Func<IntelligenceState, IntelligenceState>>> stateCloneCallbacks) { }
    }
}