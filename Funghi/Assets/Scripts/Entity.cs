using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("State")]
    [SerializeField]
    int currentHealth;
    public int Health { get { return currentHealth; } }
    [SerializeField]
    int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    protected GameWorld world;

    public bool isAttackable = true;

	public abstract void Damage(Entity attacker, int amount);
	public virtual void Repair(int amount) { }
    public void Kill(Entity murderer)
    {
        Damage(murderer, currentHealth);
    }

    protected void AddHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    protected void SubtractHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
    }

    public bool IsDead
    {
        get { return currentHealth <= 0; }
    }

    void Awake()
    {
        world = GameWorld.Instance;
        world.Register(this);
    }

    void Start()
    {
        Initialize();
    }

    public void UpdateEntity(float delta)
    {
        Tick(delta);
    }

    void OnDestroy()
    {
        Cleanup();
        world.Unregister(this);
    }

    protected virtual void Initialize() { }
    protected virtual void Tick(float deltaTime) { }
    protected virtual void Cleanup() { }

    #region Messaging
    public virtual void ReceiveBroadcast(Message message) { }
    #endregion

}
