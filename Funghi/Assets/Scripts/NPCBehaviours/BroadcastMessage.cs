using UnityEngine;

public enum NotificationType { PoliceAlarm, FungusInSight }

public class Message
{
    public readonly Entity sender;
    public readonly NotificationType type;
    public readonly Vector3 position;
    public Message(Entity sender, NotificationType type, Vector3 position)
    {
        this.sender = sender;
        this.type = type;
        this.position = position;
    }
}
