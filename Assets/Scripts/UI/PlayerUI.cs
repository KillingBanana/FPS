using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	private PlayerController controller;
	private WeaponManager weaponManager;
	[SerializeField] private RectTransform fuelBarFrame, fuelBar;
	[SerializeField] private PauseMenu pauseMenu;
	[SerializeField] private Scoreboard scoreboard;
	[SerializeField] private Image reloadIndicator;
	[SerializeField] private Text currentAmmoText, maxAmmoText;

	public void Init(PlayerController controller, WeaponManager weaponManager) {
		this.controller = controller;
		this.weaponManager = weaponManager;
	}

	private void Update() {
		if (controller != null) SetFuelAmount();
		if (weaponManager != null) {
			SetReloadBar();
			SetAmmoAmount();
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			pauseMenu.Toggle();
		}

		scoreboard.gameObject.SetActive(Input.GetKey(KeyCode.Tab));
	}

	private void SetFuelAmount() {
		fuelBarFrame.gameObject.SetActive(controller.UseJetpack);

		fuelBar.localScale = new Vector3(1, controller.FuelAmount, 1);
	}

	private void SetReloadBar() {
		reloadIndicator.gameObject.SetActive(weaponManager.IsReloading);

		reloadIndicator.fillAmount = weaponManager.ReloadProgress;
	}

	private void SetAmmoAmount() {
		currentAmmoText.text = weaponManager.CurrentWeapon.Bullets.ToString();
		maxAmmoText.text = weaponManager.CurrentWeapon.MaxBullets.ToString();
	}
}