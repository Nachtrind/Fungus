using System.Collections.Generic;
using UnityEngine;

public class SpawnerTriggerCollection: MonoBehaviour
{
	[System.Serializable]
	public class SpawnerTrigger
	{
		public enum TargetMode
		{
			Node,
			Core
		}

	    public bool triggerOnce = true;
		public TargetMode target;
		public EntitySpawner spawner;
		public Vector3 position;
		public float radius = 1f;
	}

	public List<SpawnerTrigger> triggers = new List<SpawnerTrigger> ();
    [SerializeField, HideInInspector]
    List<SpawnerTrigger> _onceTriggered = new List<SpawnerTrigger>();
    [SerializeField] Color gizmoColor;

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = gizmoColor;
		for (var i = 0; i < triggers.Count; i++) {
			Gizmos.DrawSphere (triggers [i].position, triggers [i].radius);
		}
	}

    public void Reset()
    {
        triggers.AddRange(_onceTriggered);
        _onceTriggered.Clear();
    }

	public void EvaluateForCore (Vector3 corePosition)
	{
	    for (var i = triggers.Count; i-- > 0 ;) {
			if (triggers [i].target != SpawnerTrigger.TargetMode.Core)
				continue;
			if (Vector3.SqrMagnitude (triggers [i].position - corePosition) < triggers [i].radius * triggers [i].radius) {
				var sp = triggers [i].spawner;
				if (sp) {
					sp.Activate ();
				}
			    if (triggers[i].triggerOnce)
			    {
			        _onceTriggered.Add(triggers[i]);
			        triggers.RemoveAt(i);
			    }
			}
		}
	}

	public void EvaluateForNode (Vector3 newNodePosition)
	{
	    for (var i = triggers.Count; i-- > 0 ;) {
			if (triggers [i].target != SpawnerTrigger.TargetMode.Node)
				continue;
			if (Vector3.SqrMagnitude (triggers [i].position - newNodePosition) < triggers [i].radius * triggers [i].radius) {
				var sp = triggers [i].spawner;
				if (sp) {
					sp.Activate ();
				}
                if (triggers[i].triggerOnce)
                {
                    _onceTriggered.Add(triggers[i]);
                    triggers.RemoveAt(i);
                }
            }
		}
	}

}