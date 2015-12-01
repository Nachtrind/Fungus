using UnityEngine;

namespace NPCBehaviours
{

    [DisallowMultipleComponent]
    public abstract class NPCBehaviour: MonoBehaviour
    {
        public abstract void Evaluate(float deltaTime);
        public abstract void OnAlarm(Enemy alarmSource);

        protected Enemy owner;

        void Awake()
        {
            Enemy e = GetComponent<Enemy>();
            if (!e || !e.RegisterBehaviour(this))
            {
                Destroy(this);
            }
            owner = e;
        }

        void OnDestroy()
        {
            
            if (owner)
            {
                owner.UnregisterBehaviour(this);
            }
        }

        public virtual void Initialize() { }
        public virtual void Tick(float deltaTime) { }
    }
}
