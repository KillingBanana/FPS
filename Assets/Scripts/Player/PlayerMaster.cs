using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

[RequireComponent(typeof(Player)), RequireComponent(typeof(PlayerController))]
public class PlayerMaster : NetworkBehaviour {
	[SerializeField] private PlayerUI playerUIPrefab;
	private PlayerUI playerUI;
	[SerializeField] private Behaviour[] localOnlyComponents;
	[SerializeField] private Renderer[] remoteOnlyRenderers;

	private Player player;
	private Camera lobbyCamera;

	private void Awake() {
		player = GetComponent<Player>();
		player.onSpawn += OnSpawn;
		player.onDeath += OnDeath;
	}

	private void Start() {
		if (isLocalPlayer) {
			DisableRenderers();
			gameObject.layer = LayersManager.LocalPlayerLayer;

			lobbyCamera = Camera.main;
			playerUI = Instantiate(playerUIPrefab);
			playerUI.Init(GetComponent<PlayerController>(), GetComponent<WeaponManager>());

			if (AccountManager.CurrentAccount != null) player.CmdInitPlayer(AccountManager.CurrentAccount.Username);
			player.CmdSpawn(transform.position, transform.rotation);
		} else {
			DisableComponents();
			gameObject.layer = LayersManager.RemotePlayerLayer;
		}

		GameManager.AddPlayer(player);
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

	private void OnSpawn() {
		if (isLocalPlayer) {
			foreach (Behaviour behaviour in localOnlyComponents) {
				behaviour.enabled = true;
			}

			if (playerUI) playerUI.gameObject.SetActive(true);
			if (lobbyCamera) lobbyCamera.gameObject.SetActive(false);
		}
	}

	private void OnDeath() {
		if (isLocalPlayer) {
			foreach (Behaviour behaviour in localOnlyComponents) {
				behaviour.enabled = false;
			}

			if (playerUI) playerUI.gameObject.SetActive(false);
			if (lobbyCamera) lobbyCamera.gameObject.SetActive(true);
		}
	}
}