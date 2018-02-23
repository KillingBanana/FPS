using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player)), RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {
	[SerializeField] private new Camera camera;
	public LayerMask hittableLayers;

	private Player player;

	private WeaponManager weaponManager;
	private Weapon weapon;

	private float shootCooldown;

	private void Start() {
		player = GetComponent<Player>();
		weaponManager = GetComponent<WeaponManager>();
	}

	private void Update() {
		weapon = weaponManager.GetCurrentWeapon();

		if (isLocalPlayer && !player.Dead && !PauseMenu.Paused) ProcessInput();
	}

	private void ProcessInput() {
		bool shootInput = weapon.auto && Input.GetButton("Fire1") || Input.GetButtonDown("Fire1");

		shootCooldown -= Time.deltaTime;

		if (shootCooldown <= 0 && shootInput) {
			shootCooldown = 1 / weapon.fireRate;
			Shoot();
		}
	}

	[Command]
	private void CmdOnShoot() => RpcOnShoot();

	[ClientRpc]
	private void RpcOnShoot() {
		if (!isLocalPlayer) MuzzleFlash();
	}

	private void MuzzleFlash() {
		weaponManager.GetCurrentModel().OnShoot();
	}

	[Client]
	private void Shoot() {
		MuzzleFlash();
		CmdOnShoot();
		RaycastHit hit;
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, weapon.range, hittableLayers)) {
			Player hitPlayer = hit.transform.GetComponent<Player>();
			if (hitPlayer) {
				player.CmdHit(new HitInfo(player.netId, hitPlayer.netId, weapon.damage, weapon.name));
			} else {
				Impact(hit.point, hit.normal);
				CmdOnHit(hit.point, hit.normal);
			}
		}
	}

	[Command]
	private void CmdOnHit(Vector3 position, Vector3 normal) => RpcOnHit(position, normal);

	[ClientRpc]
	private void RpcOnHit(Vector3 position, Vector3 normal) {
		if (!isLocalPlayer) Impact(position, normal);
	}

	private void Impact(Vector3 position, Vector3 normal) {
		GameObject impact = Instantiate(weaponManager.GetCurrentModel().impactPrefab, position, Quaternion.LookRotation(normal));
		Destroy(impact, 2f);
	}
}