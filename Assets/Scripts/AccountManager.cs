using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AccountManager : MonoBehaviour {
	private static AccountManager instance;

	[SerializeField] private string setDataPhp, getDataPhp;
	[SerializeField] private string loginScene = "Login", loggedInScene = "Lobby";

	public static bool LoggedIn => CurrentAccount != null;

	public static Account CurrentAccount { get; private set; }

	private void Awake() {
		if (instance != null) {
			Debug.LogWarning($"Error: AccountManager instance already set, destroying GameObject {name}");
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public static void LogIn(string userId) {
		instance.GetAccountData(userId, OnLogIn);
	}

	private static void OnLogIn() {
		SceneManager.LoadScene(instance.loggedInScene);
	}

	public static void LogOut() {
		CurrentAccount = null;

		SceneManager.LoadScene(instance.loginScene);
	}

	private void GetAccountData(string userId, Action callback) {
		StartCoroutine(GetAccountDataCoroutine(userId, callback));
	}

	private IEnumerator GetAccountDataCoroutine(string userId, Action callback) {
		WWWForm form = new WWWForm();
		form.AddField("userid", userId);

		WWW www = new WWW(getDataPhp, form);

		yield return www;

		CurrentAccount = new Account(www.text);

		callback.Invoke();
	}
}