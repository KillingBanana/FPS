using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	private PlayerController controller;
	private WeaponManager weaponManager;
	[SerializeField] private RectTransform fuelBarFrame, fuelBar;
	[SerializeField] private PauseMenu pauseMenu;
	[SerializeField] private Scoreboard scoreboard;
	[SerializeField] private Image reloadIndicator;

	public void Init(PlayerController controller, WeaponManager weaponManager) {
		this.controller = controller;
		this.weaponManager = weaponManager;
	}

	private void Update() {
		if (controller != null) SetFuelAmount(controller.FuelAmount);
		if (weaponManager != null) SetReloadBar(weaponManager.ReloadProgress);

		if (Input.GetKeyDown(KeyCode.Escape)) {
			pauseMenu.Toggle();
		}

		scoreboard.gameObject.SetActive(Input.GetKey(KeyCode.Tab));
	}

	private void SetFuelAmount(float amount) {
		fuelBarFrame.gameObject.SetActive(controller.UseJetpack);

		fuelBar.localScale = new Vector3(1, amount, 1);
	}

	private void SetReloadBar(float amount) {
		reloadIndicator.gameObject.SetActive(weaponManager.IsReloading);

		reloadIndicator.fillAmount = amount;
	}
}