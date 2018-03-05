using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player)), RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : MonoBehaviour {
	[SerializeField] private new Camera camera;
	public LayerMask hittableLayers;

	private Player player;

	private WeaponManager weaponManager;
	private Weapon Weapon => weaponManager.CurrentWeapon;

	private float shootCooldown;

	private void Start() {
		player = GetComponent<Player>();
		weaponManager = GetComponent<WeaponManager>();
	}

	private void Update() {
		if (!player.Dead && !PauseMenu.Paused) ProcessInput();
	}

	private void ProcessInput() {
		bool shootInput = Weapon.auto && Input.GetButton("Fire1") || Input.GetButtonDown("Fire1");

		shootCooldown -= Time.deltaTime;

		if (!weaponManager.IsReloading && shootCooldown <= 0 && shootInput) {
			shootCooldown = Weapon.ShootCooldown;

			if (Weapon.Bullets > 0) {
				Shoot();
				Weapon.Shoot();
			}
		}

		bool reload = !weaponManager.IsReloading && (Weapon.Bullets == 0 || Input.GetKeyDown(KeyCode.R));

		if (reload) {
			weaponManager.Reload();
		}
	}

	private void Shoot() {
		weaponManager.CmdOnShoot();
		RaycastHit hit;
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, hittableLayers)) {
			Player hitPlayer = hit.transform.GetComponent<Player>();
			if (hitPlayer) {
				player.CmdHit(new HitInfo(player.netId, hitPlayer.netId, Weapon.damage, Weapon.name));
			} else {
				weaponManager.CmdOnHit(hit.point, hit.normal);
			}
		}
	}
}