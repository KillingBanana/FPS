﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DatabaseControl;
using JetBrains.Annotations;

// << Remember to add this reference to your scripts which use DatabaseControl

public class LoginManager : MonoBehaviour {
	//All public variables bellow are assigned in the Inspector

	[SerializeField] private int minUsernameLength = 5, minPasswordLength = 8;

	//These are the GameObjects which are parents of groups of UI elements. The objects are enabled and disabled to show and hide the UI elements.
	[SerializeField] private GameObject loginParent, registerParent, loggedInParent, loadingParent;

	//These are all the InputFields which we need in order to get the entered usernames, passwords, etc
	[SerializeField] private InputField loginUsernameField, loginPasswordField, registerUsernameField, registerPasswordField, registerConfirmPasswordField;

	//These are the UI Texts which display errors
	[SerializeField] private Text loginErrorText, registerErrorText;

	//Called at the very start of the game
	private void Awake() {
		ResetAllUIElements();
	}

	//Called by Button Pressed Methods to Reset UI Fields
	private void ResetAllUIElements() {
		//This resets all of the UI elements. It clears all the strings in the input fields and any errors being displayed
		loginUsernameField.text = "";
		loginPasswordField.text = "";
		registerUsernameField.text = "";
		registerPasswordField.text = "";
		registerConfirmPasswordField.text = "";
		loginErrorText.text = "";
		registerErrorText.text = "";
	}

	//Called by Button Pressed Methods. These use DatabaseControl namespace to communicate with server.
	private IEnumerator LoginUser(string username, string password) {
		IEnumerator e = DCF.Login(username, password); // << Send request to login, providing username and password
		while (e.MoveNext()) {
			yield return e.Current;
		}

		string response = e.Current as string; // << The returned string from the request

		if (response == "Success") {
			//Username and Password were correct. Stop showing 'Loading...' and transition to new scenew
			AccountManager.LogIn(username, password);
		} else {
			//Something went wrong logging in. Stop showing 'Loading...' and go back to LoginUI
			loadingParent.gameObject.SetActive(false);
			loginParent.gameObject.SetActive(true);
			switch (response) {
				case "UserError":
					//The Username was wrong so display relevent error message
					loginErrorText.text = "Error: Username not found";
					break;
				case "PassError":
					//The Password was wrong so display relevent error message
					loginErrorText.text = "Error: Wrong Password";
					break;
				default:
					loginErrorText.text = "Error: Unknown Error. Please try again later.";
					break;
			}
		}
	}

	private IEnumerator RegisterUser(string username, string password) {
		Account account = new Account(username);
		IEnumerator e = DCF.RegisterUser(account.LoginUsername, password, account.ToJson()); // << Send request to register a new user, providing submitted username and password. It also provides an initial value for the data string on the account, which is "Hello World".
		while (e.MoveNext()) {
			yield return e.Current;
		}

		string response = e.Current as string; // << The returned string from the request

		if (response == "Success") {
			//Username and Password were valid. Account has been created. Stop showing 'Loading...' and show the loggedIn UI and set text to display the username.
			AccountManager.LogIn(account.LoginUsername, password);
		} else {
			//Something went wrong logging in. Stop showing 'Loading...' and go back to RegisterUI
			loadingParent.gameObject.SetActive(false);
			registerParent.gameObject.SetActive(true);
			if (response == "UserError") {
				//The username has already been taken. Player needs to choose another. Shows error message.
				registerErrorText.text = "Error: Username Already Taken";
			} else {
				//There was another error. This error message should never appear, but is here just in case.
				loginErrorText.text = "Error: Unknown Error. Please try again later.";
			}
		}
	}

	//UI Button Pressed Methods
	[UsedImplicitly]
	public void Login_LoginButtonPressed() {
		//Called when player presses button to Login

		//Check the lengths of the username and password. (If they are wrong, we might as well show an error now instead of waiting for the request to the server)
		if (loginUsernameField.text.Length >= minUsernameLength) {
			if (loginPasswordField.text.Length >= minPasswordLength) {
				//Username and password seem reasonable. Change UI to 'Loading...'. Start the Coroutine which tries to log the player in.
				loginParent.gameObject.SetActive(false);
				loadingParent.gameObject.SetActive(true);
				StartCoroutine(LoginUser(loginUsernameField.text.ToLower(), loginPasswordField.text));
			} else {
				//Password too short so it must be wrong
				loginErrorText.text = $"Error: Password too short (must be at least {minPasswordLength} characters)";
			}
		} else {
			//Username too short so it must be wrong
			loginErrorText.text = $"Error: Username too short (must be at least {minUsernameLength} characters)";
		}
	}

	[UsedImplicitly]
	public void Login_RegisterButtonPressed() {
		//Called when the player hits register on the Login UI, so switches to the Register UI
		ResetAllUIElements();
		loginParent.gameObject.SetActive(false);
		registerParent.gameObject.SetActive(true);
	}

	[UsedImplicitly]
	public void Register_RegisterButtonPressed() {
		//Called when the player presses the button to register

		//Make sure username and password are long enough
		if (registerUsernameField.text.Length >= minUsernameLength) {
			if (registerPasswordField.text.Length >= minPasswordLength) {
				//Check the two passwords entered match
				if (registerPasswordField.text == registerConfirmPasswordField.text) {
					//Username and passwords seem reasonable. Switch to 'Loading...' and start the coroutine to try and register an account on the server
					registerParent.gameObject.SetActive(false);
					loadingParent.gameObject.SetActive(true);
					StartCoroutine(RegisterUser(registerUsernameField.text, registerPasswordField.text));
				} else {
					//Passwords don't match, show error
					registerErrorText.text = "Error: Passwords don't match";
				}
			} else {
				//Password too short so show error
				registerErrorText.text = $"Error: Password too short (must be at least {minPasswordLength} characters)";
			}
		} else {
			//Username too short so show error
			registerErrorText.text = $"Error: Username too short (must be at least {minUsernameLength} characters)";
		}
	}

	[UsedImplicitly]
	public void Register_BackButtonPressed() {
		//Called when the player presses the 'Back' button on the register UI. Switches back to the Login UI
		ResetAllUIElements();
		loginParent.gameObject.SetActive(true);
		registerParent.gameObject.SetActive(false);
	}

	[UsedImplicitly]
	public void LoggedIn_LogoutButtonPressed() {
		//Called when the player hits the 'Logout' button. Switches back to Login UI and forgets the player's username and password.
		//Note: Database Control doesn't use sessions, so no request to the server is needed here to end a session.
		AccountManager.LogOut();

		ResetAllUIElements();
		loginParent.gameObject.SetActive(true);
		loggedInParent.gameObject.SetActive(false);
	}
}