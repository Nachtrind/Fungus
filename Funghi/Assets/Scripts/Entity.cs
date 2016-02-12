using ModularBehaviour;
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

    /// <summary>
    /// CurrentHealth, MaxHealth
    /// </summary>
    public event System.Action<float, float> OnHealthChanged;

    public static event System.Action<Entity> OnDamaged;

    public ParticleEffect deathParticles;

	public bool isAttackable = true;

    public static bool showDebug = false;

	public void Damage (Entity attacker, int amount)
	{
	    TriggerAnimator("Attacked");
		SubtractHealth (amount);
		if (IsDead) {
			PlaySound (SoundSet.ClipType.Death);
		    if (deathParticles)
		    {
		        deathParticles.Fire(transform.position);
		    }
		} else {
			PlaySound (SoundSet.ClipType.ReceiveDamage);
		}
	    if (OnDamaged != null)
	    {
	        OnDamaged(this);
	    }
		OnDamage (attacker);
	}

	public virtual void OnDamage (Entity attacker) { }

	public void Kill (Entity murderer)
	{
		Damage (murderer, currentHealth);
	}

	protected void AddHealth (int amount)
	{
		currentHealth = Mathf.Clamp (currentHealth + amount, 0, maxHealth);
	}

	protected void SubtractHealth (int amount)
	{
		currentHealth = Mathf.Clamp (currentHealth - amount, 0, maxHealth);
        if (OnHealthChanged != null)
        {
            OnHealthChanged(currentHealth, maxHealth);
        }
	}

	public bool IsDead {
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
	/// <param name="newBehaviour">selected behaviour to replace the old one</param>
	/// <returns></returns>
	public bool SetBehaviour (Intelligence newBehaviour)
	{
		if (behaviour != null) {
			Destroy (behaviour);
		}
		if (newBehaviour) {
			behaviour = Instantiate (newBehaviour);
            behaviour.Initialize (this);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Removes the behaviour of this entity, it will no longer react to triggers or make decisions
	/// </summary>
	public void RemoveBehaviour ()
	{
		if (behaviour != null) {
			Destroy (behaviour);
		}
	}

	/// <summary>
	/// If the behaviour supports <paramref name="triggerName"/> it will be executed and <paramref name="optionalParameter"/> will be passed to it
	/// </summary>
	/// <param name="triggerName">the name of trigger specified in the behaviour editor</param>
	/// <param name="optionalParameter">an optional parameter which the behaviour can use</param>
	public bool TriggerBehaviour (string triggerName, object optionalParameter = null)
	{
		if (behaviour) {
			return behaviour.TryExecuteTrigger (triggerName, optionalParameter);
		}
		return false;
	}
    #endregion

    #region Sound
	AudioSource audioSource;
	[SerializeField]
	[Header("Sound")]
	protected SoundSet soundSet;
    
	public bool PlaySound (SoundSet.ClipType type)
	{
		if (soundSet == null) {
			return false;
		}
		SoundSet.ClipPlayType playType;
		AudioClip clip = soundSet.GetClip (type, out playType);
		if (clip) {
			if (playType == SoundSet.ClipPlayType.OneShot) {
				//audioSource.PlayOneShot (clip);
			    AudioSource.PlayClipAtPoint(clip, transform.position, 0.6f);
				return true;
			}
			audioSource.clip = clip;
			audioSource.Play ();
			return true;
		}
		return false;
	}

	public void PlayOneShotSound (AudioClip clip)
	{
		if (clip == null) {
			return;
		}
		audioSource.PlayOneShot (clip);
	}

	public void OverrideContinuousSound (AudioClip clip)
	{
		if (clip == null) {
			return;
		}
		audioSource.clip = clip;
		audioSource.Play ();
	}

	public bool StopSound (SoundSet.ClipType type)
	{
		if (soundSet == null) {
			return false;
		}
		SoundSet.ClipPlayType playType;
		AudioClip clip = soundSet.GetClip (type, out playType);
		if (clip) {
			if (playType == SoundSet.ClipPlayType.OneShot) {
				return true;
			}
			if (audioSource.clip == clip) {
				audioSource.Stop ();
				return true;
			}
		}
		return false;
	}

    #endregion

	void Awake ()
	{
		Rigidbody rb = GetComponent<Rigidbody> ();
		if (rb) {
			rb.isKinematic = true;
			rb.useGravity = false;
		}
		world = GameWorld.Instance;
		world.Register (this);
		mover = GetComponent<EntityMover> ();
		audioSource = GetComponent<AudioSource> ();
		float rndFactor = StandardGameSettings.Get.entityPitchRandomization;
		audioSource.pitch = Random.Range (1f - rndFactor, 1f + rndFactor);
		audioSource.playOnAwake = false;
		audioSource.loop = true;
		OnAwake ();
	}

	void Start ()
	{      
		OnStart ();
	}

	public void UpdateEntity (float delta)
	{
		if (mover) {
			mover.UpdateMovement (delta);
		}
		if (behaviour) {
			behaviour.UpdateTick (delta);
		}
		OnUpdate (delta);
	}

	void OnDestroy ()
	{
		Cleanup ();
		world.Unregister (this);
	}

	protected virtual void OnStart ()
	{

	}

	protected virtual void OnAwake ()
	{

	}

	protected virtual void OnUpdate (float deltaTime)
	{
	}

	protected virtual void Cleanup ()
	{
	}

    #region Animator
    public void TriggerAnimator(string triggerID)
    {
        if (mover)
        {
            mover.TriggerAnimator(triggerID);
        }
    }
    #endregion

    #region Movement
    EntityMover mover;

	public EntityMover.MoveResult MoveTo (Vector3 position)
	{
		if (!mover) {
			return EntityMover.MoveResult.NotAllowed;
		}
		return mover.MoveTo (position);
	}

	public EntityMover.MoveResult MoveToDirect (Vector3 position)
	{
		if (!mover) {
			return EntityMover.MoveResult.NotAllowed;
		}
		return mover.MoveToDirect (position);
	}

    public EntityMover.MoveResult FleeFrom(Vector3 position)
    {
        if (!mover)
        {
            return EntityMover.MoveResult.NotAllowed;
        }
        return mover.FleeFrom(position);
    }

	public void StopMovement ()
	{
		if (mover) {
			mover.StopMovement ();
		}
	}

    /// <summary>
    /// Applies a movement speed modifying effect, timeouts greater than 0 makes it so existing effects from the source are replaced by this new application
    /// </summary>
    /// <returns>the modifier id, can be used to revoke it manually</returns>
    public int ApplySpeedMod(EntityMover.SpeedModType type, Entity source, float timeOut = -1)
    {
        if (!mover) return -1;
        return mover.ApplySpeedMod(source, type, timeOut);
    }

    public void RevokeSpeedMod(int modID)
    {
        if (!mover) return;
        mover.RevokeSpeedMod(modID);
    }

    public void RevokeAllSpeedMods(Entity owner)
    {
        if (!mover) return;
        mover.RevokeAllSpeedMods(owner);
    }
    
    #endregion

    #region Messaging
	public virtual void ReceiveBroadcast (Message message)
	{
		if (behaviour) {
			behaviour.HandleMessageBroadcast (message);
		}
	}
    #endregion

}
