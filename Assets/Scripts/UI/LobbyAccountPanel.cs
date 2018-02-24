using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAccountPanel : MonoBehaviour {
	[SerializeField] private Text usernameText, statsText;

	private void Start() {
		if (!AccountManager.LoggedIn) {
			Debug.LogWarning("Error: entering lobby without being logged in");
			usernameText.text = "Error: Not logged in";
			return;
		}

		usernameText.text = AccountManager.CurrentAccount.Username;
		statsText.text = $"Kills: {AccountManager.CurrentAccount.Kills}\nDeaths: {AccountManager.CurrentAccount.Deaths}";
	}

	[UsedImplicitly]
	public void LogOut() => AccountManager.LogOut();
}