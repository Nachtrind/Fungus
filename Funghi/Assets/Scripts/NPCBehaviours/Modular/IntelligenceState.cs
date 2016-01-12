using UnityEngine;
using System.Collections.Generic;

namespace ModularBehaviour
{
    public class IntelligenceState: ScriptableObject
    {
        public List<AIAction> enterActions = new List<AIAction>();
        public List<AIAction> updateActions = new List<AIAction>();
        public List<AIAction> exitActions = new List<AIAction>();

        public void OnEnter(IntelligenceController controller)
        {
            for (int i = 0; i < enterActions.Count; i++)
            {
                enterActions[i].Fire(controller);
            }
        }

        public void OnExit(IntelligenceController controller)
        {
            for (int i = 0; i < exitActions.Count; i++)
            {
                exitActions[i].Fire(controller);
            }
        }

        public void OnUpdate(IntelligenceController controller, float deltaTime)
        {
            for (int i = 0; i < updateActions.Count; i++)
            {
                updateActions[i].Run(controller, deltaTime);
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

        public void DeepClone()
        {
            for (int i = 0; i < enterActions.Count; i++)
            {
                enterActions[i] = Instantiate(enterActions[i]);
                enterActions[i].DeepClone();
            }
            for (int i = 0; i < updateActions.Count; i++)
            {
                updateActions[i] = Instantiate(updateActions[i]);
                updateActions[i].DeepClone();
            }
            for (int i = 0; i < exitActions.Count; i++)
            {
                exitActions[i] = Instantiate(exitActions[i]);
                exitActions[i].DeepClone();
            }
        }
    }
}
