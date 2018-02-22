using UnityEngine;

public class Scoreboard : MonoBehaviour {
	[SerializeField] private ScoreboardItem scoreboardItemPrefab;
	[SerializeField] private Transform scoreboardParent;

	private void OnEnable() {
		foreach (Player player in GameManager.PlayerList) {
			ScoreboardItem scoreboardItem = Instantiate(scoreboardItemPrefab, scoreboardParent);
			scoreboardItem.Set(player);
		}
	}

	private void OnDisable() {
		foreach (Transform child in scoreboardParent) {
			Destroy(child.gameObject);
		}
	}
}