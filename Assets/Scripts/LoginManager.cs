using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JetBrains.Annotations;

// << Remember to add this reference to your scripts which use DatabaseControl

public class LoginManager : MonoBehaviour {
	//All public variables bellow are assigned in the Inspector

	[SerializeField] private int minUsernameLength = 5, minPasswordLength = 8;
	[SerializeField] private string loginPhp, registerPhp;

	//These are the GameObjects which are parents of groups of UI elements. The objects are enabled and disabled to show and hide the UI elements.
	[SerializeField] private GameObject loginParent, registerParent, loadingParent;

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

	private IEnumerator RegisterUser(string username, string password) {
		WWWForm form = new WWWForm();
		form.AddField("username", username);
		form.AddField("password", password);

		WWW www = new WWW(registerPhp, form);

		yield return www;

		string result = www.text;

		if (result == "Success") {
			StartCoroutine(LoginUser(username, password));
		} else {
			loadingParent.SetActive(false);
			registerParent.SetActive(true);
			switch (result) {
				case "UsernameTaken":
					registerErrorText.text = "Error: Username already taken";
					break;
				default:
					registerErrorText.text = "Error: Unknown Error. Please try again later.";
					break;
			}
		}
	}

	//Called by Button Pressed Methods. These use DatabaseControl namespace to communicate with server.
	private IEnumerator LoginUser(string userid, string password) {
		WWWForm form = new WWWForm();
		form.AddField("username", userid);
		form.AddField("password", password);

		WWW www = new WWW(loginPhp, form);

		yield return www;

		string result = www.text;

		if (result == "Success") {
			//Username and Password were correct. Stop showing 'Loading...' and transition to new scenew
			AccountManager.LogIn(userid);
		} else {
			//Something went wrong logging in. Stop showing 'Loading...' and go back to LoginUI
			loadingParent.gameObject.SetActive(false);
			loginParent.gameObject.SetActive(true);
			switch (result) {
				case "UsernameError":
					//The Username was wrong so display relevent error message
					loginErrorText.text = "Error: Username not found";
					break;
				case "PasswordError":
					//The Password was wrong so display relevent error message
					loginErrorText.text = "Error: Wrong Password";
					break;
				default:
					loginErrorText.text = "Error: Unknown Error. Please try again later.";
					break;
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
}