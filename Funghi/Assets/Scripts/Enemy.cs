using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using NPCBehaviours;

[RequireComponent(typeof(Seeker))]
public class Enemy : Entity
{
	Seeker seeker;
	Vector3 targetPosition;
	List<Vector3> pathToTarget = new List<Vector3> ();
	Entity target;
	public int resourceValue;

	//dummy feedback TODO: delete later on
	ParticleSystem particleDamage;
	[Header("Behaviour")]
	public NPCBehaviour
		behaviour;
	const float repathRate = 0.5f;
	float lastPath;
	public float moveSpeed = 1f;

	protected override void Initialize ()
	{
		seeker = GetComponent<Seeker> ();
		particleDamage = this.GetComponent<ParticleSystem> ();
	}

	public void Alarm (Enemy alarmSource)
	{
		if (behaviour) {
			behaviour.OnAlarm (alarmSource);
		}
	}

	protected override void Tick (float deltaTime)
	{
		if (behaviour) {
			behaviour.Evaluate (deltaTime);
		}
		if (Time.time - lastPath > repathRate) {
			RequestPath (targetPosition);
			lastPath = Time.time;
		}
		if (pathToTarget.Count > 0) {
			if (AstarMath.SqrMagnitudeXZ (pathToTarget [0], transform.position) < 0.05f) {
				pathToTarget.RemoveAt (0);
			}
			if (pathToTarget.Count > 0) {
				transform.position = Vector3.MoveTowards (transform.position, pathToTarget [0], deltaTime * moveSpeed);
			}
		}
	}

	public bool RegisterBehaviour (NPCBehaviour behaviour)
	{
		if (this.behaviour != null) {
			return false;
		}
		this.behaviour = behaviour;
		behaviour.Initialize ();
		return true;
	}

	public void UnregisterBehaviour (NPCBehaviour behaviour)
	{
		if (this.behaviour == behaviour) {
			this.behaviour = null;
		}
	}

	public override void Damage (Entity attacker, int amount)
	{
		SubtractHealth (amount);
		particleDamage.Play ();
		if (IsDead) {
			world.OnEnemyWasKilled (this);
		}
	}

	public bool PathTo (Vector3 position)
	{
		if (targetPosition != position) {
			targetPosition = position;
			RequestPath (position);
		}
		if (pathToTarget.Count > 0) {
			if (pathComputing == true) {
				return false;
			}
			return AstarMath.SqrMagnitudeXZ (transform.position, pathToTarget [pathToTarget.Count - 1]) <= 0.05f;
		}
		return false;
	}

	void RequestPath (Vector3 position)
	{
		seeker.StartPath (transform.position, targetPosition, OnPathCompleted);
		pathComputing = true;
	}

	bool pathComputing = false;

	void OnPathCompleted (Path p)
	{
		if (p.error) {
			Debug.LogError ("Error in pathfinding");
			return;
		}
		pathToTarget = p.vectorPath;
		pathComputing = false;
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = new Color (1, 0, 0f, 0.5f);
		Gizmos.DrawSphere (transform.position, 0.15f);
	}
}
