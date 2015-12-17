using UnityEngine;
using System.Collections.Generic;

namespace ModularBehaviour
{
    public class IntelligenceState: ScriptableObject
    {
        public List<OneShotAction> enterActions = new List<OneShotAction>();
        public List<AIAction> updateActions = new List<AIAction>();
        public List<OneShotAction> exitActions = new List<OneShotAction>();

        public void OnEnter(IntelligenceController controller)
        {
            for (int i = 0; i < enterActions.Count; i++)
            {
                if (enterActions[i].Fire(controller)== ActionResult.SkipNext)
                {
                    return;
                }
            }
        }

        public void OnExit(IntelligenceController controller)
        {
            for (int i = 0; i < exitActions.Count; i++)
            {
                if (exitActions[i].Fire(controller) == ActionResult.SkipNext)
                {
                    return;
                }
            }
        }

        public void OnUpdate(IntelligenceController controller, float deltaTime)
        {
            for (int i = 0; i < updateActions.Count; i++)
            {
                if (updateActions[i].Run(controller, deltaTime) == ActionResult.SkipNext)
                {
                    return;
                }
            }
        }

        public void OnDelete(CallbackCollection callbacks)
        {
            for (int i = 0; i < enterActions.Count; i++)
            {
                enterActions[i].OnDelete(callbacks);
            }
            for (int i = 0; i < updateActions.Count; i++)
            {
                updateActions[i].OnDelete(callbacks);
            }
            for (int i = 0; i < exitActions.Count; i++)
            {
                exitActions[i].OnDelete(callbacks);
            }
        }
    }
}
