using UnityEngine;

[System.Serializable]
public class Account {
	private readonly string userId, username;
	public string Username => username;

	public int Kills { get; private set; }
	public int Deaths { get; private set; }

	public Account(string accountData) {
		string[] data = accountData.Split(';');

		userId = data[0];
		username = data[1];

		int kills, deaths;
		if (int.TryParse(data[2], out kills)) {
			Kills = kills;
		} else {
			Debug.LogError($"Could not parse kills ({data[2]})");
		}

		if (int.TryParse(data[3], out deaths)) {
			Deaths = deaths;
		} else {
			Debug.LogError($"Could not parse deaths ({data[3]})");
		}
	}
}