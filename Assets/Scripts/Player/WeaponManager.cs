using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class WeaponManager : NetworkBehaviour {
	[SerializeField] private Transform weaponParent, viewmodelWeaponParent;
	[SerializeField] private Weapon[] weapons;

	public float ReloadProgress { get; private set; }

	public bool IsReloading { get; private set; }

	public Weapon CurrentWeapon { get; private set; }

	private WeaponModel CurrentModel { get; set; }

	private void Start() {
		Reset();
	}

	private void Reset() {
		foreach (Weapon weapon in weapons) {
			weapon.Reload();
		}

		if (weapons.Length > 0) {
			EquipWeapon(weapons[0]);
		} else {
			Debug.LogError("Error: no weapons set");
		}
	}

	[Command]
	public void CmdOnShoot() {
		MuzzleFlash();
		RpcOnShoot();
	}

	[ClientRpc]
	private void RpcOnShoot() {
		if (!isLocalPlayer) MuzzleFlash();
	}

	private void MuzzleFlash() {
		CurrentModel.OnShoot();
	}

	[Command]
	public void CmdOnHit(Vector3 position, Vector3 normal) {
		Impact(position, normal);
		RpcOnHit(position, normal);
	}

	[ClientRpc]
	private void RpcOnHit(Vector3 position, Vector3 normal) {
		if (!isLocalPlayer) Impact(position, normal);
	}

	private void Impact(Vector3 position, Vector3 normal) {
		GameObject impact = Instantiate(CurrentModel.impactPrefab, position, Quaternion.LookRotation(normal));
		Destroy(impact, 2f);
	}

	private void EquipWeapon(Weapon newWeapon) {
		ClearWeapon();

		CurrentWeapon = newWeapon;
		CurrentModel = Instantiate(newWeapon.model, weaponParent.position, weaponParent.rotation, weaponParent);

		if (isLocalPlayer) {
			CurrentModel.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			CurrentModel = Instantiate(newWeapon.model, viewmodelWeaponParent.position, viewmodelWeaponParent.rotation, viewmodelWeaponParent);
			Utility.SetLayerRecursively(CurrentModel.gameObject, LayersManager.ViewmodelLayer);
		}
	}

	private void ClearWeapon() {
		foreach (Transform child in weaponParent) {
			Destroy(child.gameObject);
		}

		if (isLocalPlayer) {
			foreach (Transform child in viewmodelWeaponParent) {
				Destroy(child.gameObject);
			}
		}
	}

	public void Reload() {
		if (!IsReloading) StartCoroutine(ReloadCoroutine());
	}

	private IEnumerator ReloadCoroutine() {
		IsReloading = true;

		float reloadProgress = 0;

		while (reloadProgress < CurrentWeapon.reloadTime) {
			reloadProgress += Time.deltaTime;
			ReloadProgress = reloadProgress / CurrentWeapon.reloadTime;
			yield return null;
		}

		CurrentWeapon.Reload();

		IsReloading = false;
	}
}