using UnityEngine;
using System.Collections;

namespace Tutorials
{
    public abstract class TutorialAction: ScriptableObject
    {
#if UNITY_EDITOR
        public bool Preview = false;
#endif
        public Tutorial.HookType Hook;
        public float Timeout = -1;
        protected float StartTime;
        public bool PauseGame = false;

        public Tutorial.HookEventInfo CachedEventInfo;

        public abstract bool Execute(Tutorial t, float time);

        public abstract void OnInitialize(Tutorial t, float time);

        public abstract void OnCleanup(Tutorial t);
    }

}

