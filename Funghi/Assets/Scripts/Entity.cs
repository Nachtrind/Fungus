﻿using ModularBehaviour;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public abstract class Entity : MonoBehaviour
{
    protected GameWorld world;
    #region Health
    [Header("State")]
    [SerializeField]
    int currentHealth;
    public int Health { get { return currentHealth; } }
    [SerializeField]
    int maxHealth;
    public int MaxHealth { get { return maxHealth; } }

    public bool isAttackable = true;

    public void Damage(Entity attacker, int amount)
    {
        SubtractHealth(amount);
        if (IsDead)
        {
            PlaySound(SoundSet.ClipType.Death);
        }
        else
        {
            PlaySound(SoundSet.ClipType.ReceiveDamage);
        }
        OnDamage(attacker);
    }
    public virtual void OnDamage(Entity attacker) { }
    public void Kill(Entity murderer)
    {
        PlaySound(SoundSet.ClipType.Death);
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
    #endregion

    #region Behaviour
    [SerializeField, ReadOnlyInInspector]
    protected Intelligence behaviour;
    /// <summary>
    /// Direct access to the behaviour, used by the Spawner (for triggers use <seealso cref="TriggerBehaviour(string, object)"/>)
    /// </summary>
    public Intelligence Behaviour { get { return behaviour; } }

    /// <summary>
    /// Overrides the behaviour of this entity
    /// </summary>
    /// <param name="behaviour">selected behaviour to replace the old one</param>
    /// <returns></returns>
    public bool SetBehaviour(Intelligence behaviour)
    {
        if (this.behaviour != null) { Destroy(this.behaviour); }
        if (behaviour)
        {
            this.behaviour = Instantiate(behaviour);
            this.behaviour.Initialize(this);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the behaviour of this entity, it will no longer react to triggers or make decisions
    /// </summary>
    public void RemoveBehaviour()
    {
        if (behaviour != null) { Destroy(behaviour); }
    }

    /// <summary>
    /// If the behaviour supports <paramref name="triggerName"/> it will be executed and <paramref name="optionalParameter"/> will be passed to it
    /// </summary>
    /// <param name="triggerName">the name of trigger specified in the behaviour editor</param>
    /// <param name="optionalParameter">an optional parameter which the behaviour can use</param>
    public bool TriggerBehaviour(string triggerName, object optionalParameter = null)
    {
        if (behaviour)
        {
            return behaviour.TryExecuteTrigger(triggerName, optionalParameter);
        }
        return false;
    }
    #endregion

    #region Sound
    AudioSource audioSource;

    [SerializeField]
    [Header("Sound")]
    SoundSet soundSet;
    
    public bool PlaySound(SoundSet.ClipType type)
    {
        if (soundSet == null) { return false; }
        SoundSet.ClipPlayType playType;
        AudioClip clip = soundSet.GetClip(type, out playType);
        if (clip)
        {
            if (playType == SoundSet.ClipPlayType.OneShot)
            {
                audioSource.PlayOneShot(clip);
                return true;
            }
            audioSource.clip = clip;
            audioSource.Play();
            return true;
        }
        return false;
    }

    public void PlayOneShotSound(AudioClip clip)
    {
        if (clip == null) { return; }
        audioSource.PlayOneShot(clip);
    }

    public void OverrideContinuousSound(AudioClip clip)
    {
        if (clip == null) { return; }
        audioSource.clip = clip;
        audioSource.Play();
    }

    public bool StopSound(SoundSet.ClipType type)
    {
        if (soundSet == null) { return false; }
        SoundSet.ClipPlayType playType;
        AudioClip clip = soundSet.GetClip(type, out playType);
        if (clip)
        {
            if (playType == SoundSet.ClipPlayType.OneShot)
            {
                return true;
            }
            if (audioSource.clip == clip)
            {
                audioSource.Stop();
                return true;
            }
        }
        return false;
    }

    #endregion

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        world = GameWorld.Instance;
        world.Register(this);
        mover = GetComponent<EntityMover>();
        audioSource = GetComponent<AudioSource>();
        float rndFactor = StandardGameSettings.Get.entityPitchRandomization;
        audioSource.pitch = Random.Range(1f - rndFactor, 1f + rndFactor);
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        OnAwake();
    }

    void Start()
    {      
        OnStart();
    }

    public void UpdateEntity(float delta)
    {
        if (mover)
        {
            mover.UpdateMovement(delta);
        }
        if (behaviour) { behaviour.UpdateTick(delta); }
        OnUpdate(delta);
    }

    void OnDestroy()
    {
        Cleanup();
        world.Unregister(this);
    }

    protected virtual void OnStart() { }
    protected virtual void OnAwake() { }
    protected virtual void OnUpdate(float deltaTime) { }
    protected virtual void Cleanup() { }

    #region Movement
    EntityMover mover;
    public EntityMover.MoveResult MoveTo(Vector3 position)
    {
        if (!mover) { return EntityMover.MoveResult.NotAllowed; }
        return mover.MoveTo(position);
    }

    public EntityMover.MoveResult MoveToDirect(Vector3 position)
    {
        if (!mover) { return EntityMover.MoveResult.NotAllowed; }
        return mover.MoveToDirect(position);
    }

    public void StopMovement()
    {
        if (mover) { mover.StopMovement(); }
    }
    
    #endregion

    #region Messaging
    public virtual void ReceiveBroadcast(Message message)
    {
        if (behaviour)
        {
            behaviour.HandleMessageBroadcast(message);
        }
    }
    #endregion

}
