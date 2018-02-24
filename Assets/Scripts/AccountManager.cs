using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AccountManager : MonoBehaviour {
	private static AccountManager instance;

	[SerializeField] private string loginScene = "Login", loggedInScene = "Lobby";

	public static bool LoggedIn => CurrentAccount != null;

	public static Account CurrentAccount { get; private set; }

	private static string accountPassword;

	private void Awake() {
		if (instance != null) {
			Debug.LogWarning($"Error: AccountManager instance already set, destroying GameObject {name}");
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public static void LogIn(string username, string password) {
		instance.GetDataRequest(username, password, OnLogIn);
		accountPassword = password;
	}

	private static void OnLogIn(Account account) {
		CurrentAccount = account;

		SceneManager.LoadScene(instance.loggedInScene);
	}

	private static void OnAccountDataReceived(Account account) => CurrentAccount = account;

	public static void LogOut() {
		CurrentAccount = null;
		accountPassword = "";

		SceneManager.LoadScene(instance.loginScene);
	}

	public static void SetData(Action<Account> callback = null) {
		if (LoggedIn) {
			instance.SetDataRequest(callback);
		} else {
			Debug.LogWarning("Error: trying to set data without being logged in");
		}
	}

	private void SetDataRequest(Action<Account> callback = null) => StartCoroutine(SetDataCoroutine(callback));

	private static IEnumerator SetDataCoroutine(Action<Account> callback = null) {
		yield return null;
		/*IEnumerator e = DCF.SetUserData(CurrentAccount.LoginUsername, accountPassword, CurrentAccount.ToJson()); // << Send request to set the player's data string. Provides the username, password and new data string

		while (e.MoveNext()) {
			yield return e.Current;
		}

		string response = e.Current as string; // << The returned string from the request

		if (response == "Success") {
			//The data string was set correctly. Goes back to LoggedIn UI
			Debug.Log($"Data set successfully for account {CurrentAccount.Username}.");
			callback?.Invoke(CurrentAccount);
		} else {
			//There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
			Debug.LogWarning($"Error setting data for account {CurrentAccount.Username}.");
		}*/
	}

	public static void GetData(Action<Account> callback) {
		if (LoggedIn) {
			instance.GetDataRequest(CurrentAccount.LoginUsername, accountPassword, callback);
		} else {
			Debug.LogWarning("Error: trying to get data without being logged in");
		}
	}

	private void GetDataRequest(string username, string password, Action<Account> callback) => StartCoroutine(GetDataCoroutine(username, password, callback));

	private static IEnumerator GetDataCoroutine(string username, string password, Action<Account> callback) {
		yield return null;
		/*IEnumerator e = DCF.GetUserData(username, password); // << Send request to get the player's data string. Provides the username and password

		while (e.MoveNext()) {
			yield return e.Current;
		}

		string response = e.Current as string; // << The returned string from the request

		if (response == "Error") {
			//There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
			Debug.LogWarning($"Error getting data for account {username}.");
		} else {
			//The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField

			callback.Invoke(Account.FromJson(response));
		}*/
	}
}