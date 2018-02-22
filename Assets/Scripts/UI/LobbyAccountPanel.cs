using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAccountPanel : MonoBehaviour {
	[SerializeField] private Text usernameText;

	private void Start() {
		if (!AccountManager.LoggedIn) {
			Debug.LogWarning("Error: entering lobby without being logged in");
			usernameText.text = "Error: Not logged in";
			return;
		}

		usernameText.text = AccountManager.CurrentAccount.Username;
	}

	public void ChangeUsername(InputField newUsername) {
		AccountManager.CurrentAccount.SetUsername(newUsername.text);
		AccountManager.SetData();
	}

	[UsedImplicitly]
	public void LogOut() => AccountManager.LogOut();
}