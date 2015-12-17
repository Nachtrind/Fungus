using System;
using UnityEngine;

[RequireComponent(typeof(HumanSpawner))]
public class PoliceStation: House
{
    [SerializeField]
    HumanSpawner spawner;

    public override void Damage(Entity attacker, int amount) { }

    Vector3 lastSeenNodePosition;

    void Start()
    {
        if (spawner == null)
        {
            spawner = GetComponent<HumanSpawner>();
        }
    }

    public override void Trigger()
    {
        spawner.Activate();
    }

    public void OnPoliceSpawned(Human human)
    {
        if (human.Behaviour)
        {
            human.Behaviour.TryExecuteTrigger("Alarm", lastSeenNodePosition);
        }
    }

    public override void ReceiveBroadcast(Message message)
    {
        if (message.type == NotificationType.PoliceAlarm)
        {
            lastSeenNodePosition = message.position;
            Trigger();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.25f);
    }
}
