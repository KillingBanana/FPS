using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	private const ushort MaxHealth = 100;

	[SyncVar] public string username = "< Username >";

	[SyncVar] private bool dead;
	[SyncVar] private short health;

	[SyncVar] public int kills, deaths, assists, ping;

	private void Update() {
		if (isServer && transform.position.y < -100f && !dead) {
			CmdDie(new HitInfo(netId, netId, MaxHealth, "Fall"));
		}
	}

	public void OnSpawn() {
		health = (short) MaxHealth;
		dead = false;
	}

	[Command]
	public void CmdHit(HitInfo hitInfo) {
		if (hitInfo.shooterId != netId) Debug.LogWarning("Player NetID and shooter NetID do not match");
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
			Debug.LogWarning($"Trying to kill player that is already dead ({GameManager.GetPlayer(hitInfo.hitId).username})");
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
		Debug.Log($"{GameManager.GetPlayer(hitInfo.shooterId).username} killed {username} ({(hitInfo.Suicide ? "Suicide by " + hitInfo.weapon : hitInfo.weapon)})");
		gameObject.SetActive(false);
	}
}

public struct HitInfo {
	public readonly NetworkInstanceId shooterId, hitId;
	public readonly ushort damage;
	public readonly string weapon;

	public bool Suicide => shooterId == hitId;

	public HitInfo(NetworkInstanceId shooterId, NetworkInstanceId hitId, ushort damage, string weapon) {
		this.shooterId = shooterId;
		this.hitId = hitId;
		this.damage = damage;
		this.weapon = weapon;
	}
}