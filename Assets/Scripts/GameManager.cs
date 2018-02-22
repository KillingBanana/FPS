using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {
	private static GameManager instance;

	public static GameSettings Settings => instance.settings;
	[SerializeField] private GameSettings settings;

	private void Awake() {
		instance = this;
	}

	private static readonly Dictionary<NetworkInstanceId, Player> Players = new Dictionary<NetworkInstanceId, Player>();

	public static IEnumerable<Player> PlayerList => Players.Values;

	public static void AddPlayer(Player player) {
		if (Players.ContainsKey(player.netId)) {
			Debug.LogWarning("Player ID already in dictionary");
			Players.Remove(player.netId);
		}

		Players.Add(player.netId, player);
		player.name = "Player " + player.netId;
	}

	public static void RemovePlayer(NetworkInstanceId id) {
		Players.Remove(id);
	}

	public static Player GetPlayer(NetworkInstanceId id) => Players[id];

	public static void SpawnPlayer(NetworkInstanceId id) {
		Player player = GetPlayer(id);
		Transform spawn = NetworkManager.singleton.GetStartPosition();
		player.transform.position = spawn.position;
		player.transform.rotation = spawn.rotation;
		player.gameObject.SetActive(true);
		player.OnSpawn();
	}
}