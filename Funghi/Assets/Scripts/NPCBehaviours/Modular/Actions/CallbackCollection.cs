using System;
using UnityEngine;

namespace ModularBehaviour
{
    public class CallbackCollection
    {
        public Func<Type> OneShotActionPopup;
        public Func<Type> ContinuousActionPopup;
        public Func<Type> ConditionalActionPopup;
        public Func<Type> TriggerActionPopup;
        public Action<ScriptableObject> AddAsset;
        public Action<ScriptableObject> RemoveAsset;
    }
}
