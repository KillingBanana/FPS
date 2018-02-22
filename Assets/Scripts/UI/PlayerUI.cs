using UnityEngine;

public class PlayerUI : MonoBehaviour {
	[HideInInspector] public PlayerController controller;
	[SerializeField] private RectTransform fuelBarFrame, fuelBar;
	[SerializeField] private PauseMenu pauseMenu;
	[SerializeField] private Scoreboard scoreboard;

	private void Update() {
		SetFuelAmount(controller.FuelAmount);
		if (Input.GetKeyDown(KeyCode.Escape)) {
			pauseMenu.Toggle();
		}

		scoreboard.gameObject.SetActive(Input.GetKey(KeyCode.Tab));
	}

	private void SetFuelAmount(float amount) {
		fuelBarFrame.gameObject.SetActive(controller.UseJetpack);
		fuelBar.localScale = new Vector3(1, amount, 1);
	}
}