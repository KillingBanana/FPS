using UnityEngine;
using UnityEngine.UI;

public class ScoreboardItem : MonoBehaviour {
	[SerializeField] private Text nameText, killsText, deathsText, assistsText, pingText;

	private Player player;

	public void Set(Player player) {
		this.player = player;
	}

	private void Update() {
		nameText.text = player.username;
		killsText.text = player.kills.ToString();
		deathsText.text = player.deaths.ToString();
		assistsText.text = player.assists.ToString();
		pingText.text = player.ping.ToString();
	}
}