using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : MonoBehaviour {
	private static PauseMenu instance;
	private NetworkManager networkManager;
	public static bool Paused => instance != null && instance.gameObject.activeInHierarchy;

	private void Awake() {
		instance = this;
		networkManager = NetworkManager.singleton;
	}

	public void Toggle() {
		gameObject.SetActive(!gameObject.activeSelf);
	}

	[UsedImplicitly]
	public void LeaveMatch() {
		MatchInfo matchInfo = networkManager.matchInfo;
		networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, matchInfo.domain, networkManager.OnDropConnection);
		networkManager.StopHost();
	}
}