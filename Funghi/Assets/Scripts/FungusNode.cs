using NodeAbilities;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class FungusNode : Entity
{
	bool isActive;

	public bool IsActive {
		get { return isActive; }
		set {
			isActive = value;
			if (OnToggleActive != null) {
				OnToggleActive (isActive);
			}
		}
	}

	float attackTimer;
	[Header ("Materials & Sprites")]
	public Material matActive;
	public Material matInactive;
	SpriteRenderer abilityDisplay;

	public event System.Action<bool> OnToggleActive;

	protected override void OnStart ()
	{
		world.SetPositionIsSlime (transform.position, StandardGameSettings.Get.nodeSlimeExtend, true);
		CreateConnections ();
		IsActive = false;
		attackTimer = 0.0f;
		abilityDisplay = GetComponentInChildren<SpriteRenderer> ();
		ability = Instantiate (ability);
	}

	protected override void Cleanup ()
	{
		for (int i = 0; i < nodeConnections.Count; i++) {
			if (nodeConnections [i] != null) {
				nodeConnections [i].DisconnectionFrom (this);
			}
		}
		world.SetPositionIsSlime (transform.position, StandardGameSettings.Get.nodeSlimeExtend, false);
	}

	protected override void OnUpdate (float deltaTime)
	{
		//using ability if active
		if (isActive && attackTimer > ability.tickRate) {
			ExecuteAbility ();
		} else {
			attackTimer += deltaTime;
		}


		for (int i = pendingPaths.Count; i-- > 0;) {
			if (!pendingPaths [i].IsValid) {
				pendingPaths.RemoveAt (i);
				continue;
			}
			if (pendingPaths [i].IsDone) {
				world.OnNodeConnectionInitiated (this, pendingPaths [i].other, pendingPaths [i].path.vectorPath);
				pendingPaths.RemoveAt (i);
			}
		}
	}

	Color gizmoRadiusColor = new Color (1, 1, 1, 0.25f);
	Color gizmoRadiusAbilityColor = new Color (1, 0, 0, 0.25f);

	void OnDrawGizmos ()
	{
		Gizmos.color = gizmoRadiusColor;
		Gizmos.DrawWireSphere (transform.position, GameWorld.nodeConnectionDistance);
		if (ability != null) {
			Gizmos.color = gizmoRadiusAbilityColor;
			Gizmos.DrawWireSphere (transform.position, ability.radius);
		}
	}

	#region Abilities

	[Header ("Node specialization")]
	[SerializeField]
	NodeAbility ability;

	public float GetAbilityRange ()
	{
		if (!ability) {
			return 0f;
		}
		return ability.GetRange ();
	}

	public void ExecuteAbility ()
	{
		PlayOneShotSound (ability.executionSound);
		ability.Execute (this);
		attackTimer = 0.0f;
	}

	public bool IsSpecialized {
		get { return ability != null; }
	}

	public void Specialize (NodeAbility newAbility)
	{
		ability.StopExecution (this);
		ability = Instantiate (newAbility);
		abilityDisplay.color = ability.color;

	}

	#endregion

	#region Network

	public bool IsConnected {
		get { return nodeConnections.Count > 0; }
	}

	class PathRequest
	{
		public ABPath path;
		public FungusNode other;

		public PathRequest (FungusNode self, FungusNode other)
		{
			this.other = other;
			path = ABPath.Construct (self.transform.position, other.transform.position);
			AstarPath.StartPath (path);
		}

		public bool IsDone {
			get { return path.IsDone (); }
		}

		public bool IsValid {
			get { return other != null; }
		}
	}

	List<PathRequest> pendingPaths = new List<PathRequest> ();
	List<FungusNode> nodeConnections = new List<FungusNode> ();

	public List<FungusNode> Connections { get { return nodeConnections; } }

	public void ConnectTo (FungusNode other)
	{
		if (!nodeConnections.Contains (other)) {
			nodeConnections.Add (other);
			pendingPaths.Add (new PathRequest (this, other));
		}
	}

	public void DisconnectionFrom (FungusNode other)
	{
		nodeConnections.Remove (other);
	}

	void CreateConnections ()
	{
		List<FungusNode> neighbors = world.GetFungusNodes (transform.position, GameWorld.nodeConnectionDistance);
		for (int i = 0; i < neighbors.Count; i++) {
			if (neighbors [i] == this) {
				continue;
			}
			if (!nodeConnections.Contains (neighbors [i])) {
				nodeConnections.Add (neighbors [i]);
				neighbors [i].ConnectTo (this);
			}
		}
	}

	#endregion

	#region State

	public override void OnDamage (Entity attacker)
	{
		if (IsDead) {
			world.OnFungusNodeWasKilled (this);
		}
	}

	#endregion

	public void ToggleActive ()
	{
		if (isActive) {
			GetComponent<MeshRenderer> ().material = matInactive;
			attackTimer = ability.tickRate * 2.0f;
			IsActive = false;
			ability.StopExecution (this);
		} else {
			GetComponent<MeshRenderer> ().material = matActive;
			IsActive = true;
		}
	}



}