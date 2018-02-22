using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

public class WeaponManager : NetworkBehaviour {
	[SerializeField] private Transform weaponParent, viewmodelWeaponParent;
	[SerializeField] private Weapon primary;
	private Weapon currentWeapon;
	private WeaponModel currentModel;

	private void Start() {
		EquipWeapon(primary);
	}

	public Weapon GetCurrentWeapon() => currentWeapon;
	public WeaponModel GetCurrentModel() => currentModel;

	private void EquipWeapon(Weapon newWeapon) {
		currentWeapon = newWeapon;
		currentModel = Instantiate(newWeapon.model, weaponParent.position, weaponParent.rotation, weaponParent);

		if (isLocalPlayer) {
			currentModel.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			currentModel = Instantiate(newWeapon.model, viewmodelWeaponParent.position, viewmodelWeaponParent.rotation, viewmodelWeaponParent);
			Utility.SetLayerRecursively(currentModel.gameObject, LayersManager.ViewmodelLayer);
		}
	}
}