using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	private const ushort MaxHealth = 100;

	[HideInInspector] public string username = "< Username >";

	[SyncVar] private bool dead;
	public bool Dead => dead;
	[SyncVar] private short health;

	[HideInInspector, SyncVar] public int kills, deaths, assists, ping;

	public delegate void OnSpawn();

	public delegate void OnDeath();

	public OnSpawn onSpawn;
	public OnDeath onDeath;

	[Command]
	public void CmdInitPlayer(string username) => RpcInitPlayer(username);

	[ClientRpc]
	public void RpcInitPlayer(string username) {
		this.username = username;
		name = username;
	}

	[Command]
	public void CmdSpawn(Vector3 position, Quaternion rotation) {
		health = (short) MaxHealth;
		dead = false;
		RpcSpawn(position, rotation);
	}

	[ClientRpc]
	private void RpcSpawn(Vector3 position, Quaternion rotation) {
		transform.position = position;
		transform.rotation = rotation;
		gameObject.SetActive(true);
		onSpawn.Invoke();
	}

	[Command]
	public void CmdHit(HitInfo hitInfo) {
		GameManager.GetPlayer(hitInfo.hitId).CmdTakeDamage(hitInfo);
	}

	[Command]
	public void CmdTakeDamage(HitInfo hitInfo) {
		if (!dead && hitInfo.damage > 0) {
			health -= (short) hitInfo.damage;
			if (health <= 0) CmdDie(hitInfo);
		}
	}

	[Command]
	private void CmdDie(HitInfo hitInfo) {
		if (dead) {
			Debug.LogWarning($"Trying to kill player that is already dead ({GameManager.GetPlayer(hitInfo.hitId)})");
			return;
		}

		if (!hitInfo.Suicide) GameManager.GetPlayer(hitInfo.shooterId).kills++;
		dead = true;
		deaths++;
		RespawnManager.QueueRespawn(netId);
		RpcDie(hitInfo);
	}

	[ClientRpc]
	private void RpcDie(HitInfo hitInfo) {
		Killfeed.AddKill(hitInfo);
		onDeath.Invoke();

		transform.position = Vector3.zero;
		GetComponent<Rigidbody>().position = Vector3.zero;
		GetComponent<Rigidbody>().velocity = Vector3.zero;

		gameObject.SetActive(false);
	}

	public override string ToString() => username;
}

public struct HitInfo {
	public readonly NetworkInstanceId shooterId, hitId;
	public readonly ushort damage;
	public readonly string weapon;

	public bool Suicide => shooterId == hitId;

	public string KillText {
		get {
			if (Suicide) return $"<b>{GameManager.GetPlayer(hitId)}</b> died (Suicide by {weapon})";
			else return $"<b>{GameManager.GetPlayer(shooterId)}</b> killed <b>{GameManager.GetPlayer(hitId)}</b> ({weapon})";
		}
	}

	public HitInfo(NetworkInstanceId shooterId, NetworkInstanceId hitId, ushort damage, string weapon) {
		this.shooterId = shooterId;
		this.hitId = hitId;
		this.damage = damage;
		this.weapon = weapon;
	}
}