using UnityEngine;

public class Killzone : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		Player player = other.GetComponent<Player>();
		if (player) {
			player.CmdTakeDamage(new HitInfo(player.netId, player.netId, 999, "Fall"));
		}
	}
}