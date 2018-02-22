using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

[RequireComponent(typeof(Player)), RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {
	[SerializeField] private PlayerUI playerUIPrefab;
	private PlayerUI playerUI;
	[SerializeField] private Behaviour[] localOnlyComponents;
	[SerializeField] private Renderer[] remoteOnlyRenderers;

	private Camera lobbyCamera;

	private void Start() {
		if (isLocalPlayer) {
			DisableRenderers();
			gameObject.layer = LayersManager.LocalPlayerLayer;

			lobbyCamera = Camera.main;
			playerUI = Instantiate(playerUIPrefab);
			playerUI.controller = GetComponent<PlayerController>();

			OnEnable();
		} else {
			DisableComponents();
			gameObject.layer = LayersManager.RemotePlayerLayer;
		}
	}

	public override void OnStartClient() {
		base.OnStartClient();
		Player player = GetComponent<Player>();
		GameManager.AddPlayer(player);
		player.username = AccountManager.CurrentAccount.Username;
	}

	private void OnDestroy() {
		GameManager.RemovePlayer(netId);
		if (playerUI) Destroy(playerUI.gameObject);
	}

	private void DisableComponents() {
		foreach (Behaviour behaviour in localOnlyComponents) {
			behaviour.enabled = false;
		}
	}

	private void DisableRenderers() {
		foreach (Renderer renderer in remoteOnlyRenderers) {
			renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
		}
	}

	private void OnEnable() {
		if (isLocalPlayer) {
			if (playerUI) playerUI.gameObject.SetActive(true);
			if (lobbyCamera) lobbyCamera.gameObject.SetActive(false);
		}

		GetComponent<Player>().OnSpawn();
	}

	private void OnDisable() {
		if (isLocalPlayer) {
			if (playerUI) playerUI.gameObject.SetActive(false);
			if (lobbyCamera) lobbyCamera.gameObject.SetActive(true);
		}
	}
}