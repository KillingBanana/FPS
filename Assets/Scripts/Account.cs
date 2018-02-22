using UnityEngine;

[System.Serializable]
public class Account {
	[SerializeField] private string username;
	public string Username => username;
	public string LoginUsername => username.ToLower();
	[SerializeField] private int killCount;
	[SerializeField] private int deathCount;

	public Account(string username) {
		this.username = username;
		killCount = 0;
		deathCount = 0;
	}

	public void SetUsername(string newUsername) {
		username = newUsername;
	}

	public static Account FromJson(string json) => JsonUtility.FromJson<Account>(json);

	public string ToJson() => JsonUtility.ToJson(this);
}