using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NPCBehaviours
{
    [DisallowMultipleComponent]
    public abstract class NPCBehaviour: MonoBehaviour
    {
        public abstract void Evaluate(Enemy owner, float deltaTime);

        void Awake()
        {
            Enemy e = GetComponent<Enemy>();
            if (!e || !e.RegisterBehaviour(this))
            {
                Destroy(this);
            }
        }

        void OnDestroy()
        {
            Enemy e = GetComponent<Enemy>();
            if (e)
            {
                e.UnregisterBehaviour(this);
            }
        }

        public virtual void Initialize(Enemy owner) { }
        public virtual void Tick(float deltaTime) { }
    }
}
