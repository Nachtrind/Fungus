﻿using Spawner;
using UnityEngine;
using Spawner.Modules;

public class EntitySpawner: MonoBehaviour
{
	public bool autoActivateOnStart = true;
	bool active = false;

	public string tutorialTag = "";

	public bool triggerNewsTicker = false;
	public NewsTickerCategory triggerNewsCategory;
	bool tickerHasBeenTriggered = false;

	#region Settings

	[SerializeField, HideInInspector]
	SpawnModule spawnModule;
	[SerializeField, HideInInspector]
	PositionModule positionModule;
	[SerializeField, HideInInspector]
	BehaviourModule behaviourModule;
	[SerializeField, HideInInspector]
	EventModule eventModule;

	#endregion

	void Start ()
	{
		if (autoActivateOnStart) {
			Activate ();
		}
	}

	public void Activate ()
	{
		if (spawnModule == null) {
			Debug.LogError ("Spawner is missing configuration");
			return;
		}

		if (!active) {
			ModuleWorker worker = new ModuleWorker (this, OnSpawnCompleted);
			if (eventModule != null && eventModule.BeforeSpawn) {
				worker.steps.Add (eventModule.Apply);
			}
			worker.steps.Add (spawnModule.Apply);
			if (positionModule != null) {
				worker.steps.Add (positionModule.Apply);
			}
			if (behaviourModule != null) {
				worker.steps.Add (behaviourModule.Apply);
			}
			if (eventModule != null && !eventModule.BeforeSpawn) {
				worker.steps.Add (eventModule.Apply);
			}
			worker.steps.Add (ScriptableObject.CreateInstance<SpecialPathStarter> ().Apply);
			worker.ProcessNext (null);
			active = true;
		}
	}

	public event System.Action<Entity> OnSpawned;
	public static event System.Action<Entity, EntitySpawner, string> OnSpawn;

	void OnSpawnCompleted (Entity e)
	{
		if (triggerNewsTicker && !tickerHasBeenTriggered) {
			NewsTicker.Trigger (triggerNewsCategory);
			tickerHasBeenTriggered = true;
		}
		if (OnSpawned != null) {
			OnSpawned (e);
		}
		if (OnSpawn != null) {
			OnSpawn (e, this, tutorialTag);
		}
	}

	public void CancelRepeating ()
	{
		if (spawnModule != null) {
			SpawnRepeating sr = spawnModule as SpawnRepeating;
			if (sr) {
				sr.Stop ();
			}
		}
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireSphere (transform.position, 0.1f);
		if (spawnModule != null) {
			spawnModule.DrawGizmos (transform.position);
		}
		if (positionModule != null) {
			positionModule.DrawGizmos (transform.position);
		}
		if (behaviourModule != null) {
			behaviourModule.DrawGizmos (transform.position);
		}
	}
}

