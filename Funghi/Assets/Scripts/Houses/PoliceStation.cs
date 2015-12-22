using UnityEngine;

[RequireComponent(typeof(EntitySpawner))]
public class PoliceStation: House
{
    [SerializeField]
    EntitySpawner spawner;

    Vector3 lastSeenNodePosition;

    protected override void OnAwake()
    {
        if (spawner == null)
        {
            spawner = GetComponent<EntitySpawner>();
        }
        if (spawner != null)
        {
            spawner.OnSpawned += OnPoliceSpawned;
            spawner.autoActivateOnStart = false;
        }
    }

    protected override void Cleanup()
    {
        if (spawner)
        {
            spawner.OnSpawned -= OnPoliceSpawned;
        }
    }

    public override void Trigger()
    {
        spawner.Activate();
    }

    public void OnPoliceSpawned(Entity police)
    {
        police.TriggerBehaviour("Alarm", lastSeenNodePosition);
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
