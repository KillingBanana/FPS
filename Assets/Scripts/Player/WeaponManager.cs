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

	public WeaponModel CurrentModel { get; private set; }

	private void Start() {
		Reset();
	}

	public void Reset() {
		foreach (Weapon weapon in weapons) {
			weapon.Reload();
		}

		if (weapons.Length > 0) {
			EquipWeapon(weapons[0]);
		} else {
			Debug.LogError("Error: no weapons set");
		}
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