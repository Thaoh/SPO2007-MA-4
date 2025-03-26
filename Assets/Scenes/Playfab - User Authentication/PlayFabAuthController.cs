using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayFabAuthController : MonoBehaviour {
	public string TitleId = "1C503D"; // Replace with your Title ID
	public TMP_InputField EmailInputField;
	public TMP_InputField PasswordInputField;
	public TextMeshProUGUI FeedbackText; // Optional:  For displaying messages
	[SerializeField] private TMP_Text _loadLevelText;
	
	private float _sceneLoadStartWhen;

	public static Action<LoginResult> OnUserLoggedIn;
	
	private void Start() {
		PlayFabSettings.staticSettings.TitleId = TitleId;
		FeedbackText.text = $"User not logged in.";
	}

	public void RegisterUser() {
		var registerRequest = new RegisterPlayFabUserRequest {
			Email = EmailInputField.text,
			Password = PasswordInputField.text,
			RequireBothUsernameAndEmail = false // Set to true if you want to force username creation
		};

		PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterError);
	}

	public void LoginUser() {
		var loginRequest = new LoginWithEmailAddressRequest {
			Email = EmailInputField.text,
			Password = PasswordInputField.text,
			InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
				GetUserAccountInfo = true
			}
		};

		PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnLoginError);
	}

	private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
		Debug.Log("Registration successful!");
		FeedbackText.text = "Registration Successful!"; // Update UI
		Debug.Log("PlayFab ID: " + result.PlayFabId);

		// Optionally automatically log the user in after registration
		LoginUser();
	}

	private void OnRegisterError(PlayFabError error) {
		Debug.LogError("Registration error: " + error.GenerateErrorReport());
		FeedbackText.text = "Registration Error: " + error.ErrorMessage; // Update UI
	}


	private void OnLoginSuccess(LoginResult result) {
		Debug.Log("Login successful!");
		FeedbackText.text = "Login Successful!"; // Update UI
		Debug.Log("PlayFab ID: " + result.PlayFabId);

		// Access player account info (example)
		if (result.InfoResultPayload != null && result.InfoResultPayload.AccountInfo != null) {
			Debug.Log("User Email: " + result.InfoResultPayload.AccountInfo.PrivateInfo.Email);
		}
		GameManager.PlayFabLoginResult = result;
		OnUserLoggedIn?.Invoke(result);

		StartSceneLoad();
	}

	private void StartSceneLoad() {
		_sceneLoadStartWhen = Time.realtimeSinceStartup + 5f;
	}

	private void Update() {
		if (!Mathf.Approximately(_sceneLoadStartWhen, 0) && _sceneLoadStartWhen > Time.realtimeSinceStartup) {
			_loadLevelText.text = $"Next level in: { (_sceneLoadStartWhen-Time.realtimeSinceStartup):00.00}";
		} else if (!Mathf.Approximately(_sceneLoadStartWhen, 0) && _sceneLoadStartWhen < Time.realtimeSinceStartup) {
			// Load game scene, etc.
			SceneManager.LoadScene("Playfab - Leaderboards");
		}
	}

	private void OnLoginError(PlayFabError error) {
		Debug.LogError("Login error: " + error.GenerateErrorReport());
		RegisterUser();
		//feedbackText.text = "Login Error: " + error.ErrorMessage; // Update UI
	}

	public void LoginWithDeviceID() {
		var deviceId = SystemInfo.deviceUniqueIdentifier; // Unique device identifier

		var request = new LoginWithCustomIDRequest {
			CustomId = deviceId,
			CreateAccount = true, // Automatically create a PlayFab account if one doesn't exist
			InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
				GetUserAccountInfo = true
			}
		};

		PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
	}
}