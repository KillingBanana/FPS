﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RespawnManager : NetworkBehaviour {
	private RespawnManager instance;
	private static Dictionary<NetworkInstanceId, float> respawnTimer = new Dictionary<NetworkInstanceId, float>();

	private void Start() {
		if (instance != null) {
			Debug.LogWarning("RespawnManager instance already set");
			return;
		}

		instance = this;
	}

	private void Update() {
		if (!isServer) return;

		Dictionary<NetworkInstanceId, float> nextValues = new Dictionary<NetworkInstanceId, float>();
		foreach (NetworkInstanceId id in respawnTimer.Keys) {
			if (respawnTimer[id] <= 0) {
				RespawnPlayer(id);
			} else {
				nextValues.Add(id, respawnTimer[id] - Time.deltaTime);
			}
		}

		respawnTimer = nextValues;
	}

	[Server]
	public static void QueueRespawn(NetworkInstanceId id) {
		if (respawnTimer.ContainsKey(id)) {
			Debug.Log($"{GameManager.GetPlayer(id)} already queued for respawn");
		} else {
			respawnTimer.Add(id, GameManager.Settings.respawnTime);
		}
	}

	[Server]
	private void RespawnPlayer(NetworkInstanceId id) {
		Player player = GameManager.GetPlayer(id);
		Transform spawn = NetworkManager.singleton.GetStartPosition();

		player.CmdSpawn(spawn.position, spawn.rotation);
	}
}