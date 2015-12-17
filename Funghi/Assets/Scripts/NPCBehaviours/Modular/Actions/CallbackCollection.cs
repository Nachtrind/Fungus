using System;
using UnityEngine;

namespace ModularBehaviour
{
    public class CallbackCollection
    {
        public Func<Type> OneShotPopup;
        public Func<Type> RegularActionPopup;
        public Func<Type> TriggerActionPopup;
        public Action<ScriptableObject> AddAsset;
        public Action<ScriptableObject> RemoveAsset;
    }
}
