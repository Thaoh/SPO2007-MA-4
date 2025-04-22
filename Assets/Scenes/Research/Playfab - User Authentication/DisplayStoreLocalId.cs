using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class DisplayStoreLocalId : MonoBehaviour {
	[SerializeField] private TMP_Text infoGraphicText;

	bool _storedOnline = false;
	private void Start() {
		infoGraphicText.text = $"Local Id: {DataStore.LocalUserID}\n Stored Online: {_storedOnline}";
		PlayFabAuthController.OnUserLoggedIn += UserLoggedInTimeToSave;
	}

	private void OnDisable() {
		PlayFabAuthController.OnUserLoggedIn -= UserLoggedInTimeToSave;
	}

	private void UserLoggedInTimeToSave(LoginResult obj) {
		PlayFabGameState.OnGameStateSaved += GameStateSaved;
		PlayFabGameState.SaveGame("LocalUserID", DataStore.LocalUserID);
		
	}

	private void GameStateSaved(bool gameSaveSuccess) {
		_storedOnline = gameSaveSuccess;
		Debug.Log($"[DisplayStoreLocalID.GameStateSaved] Game saved: {_storedOnline}");
		infoGraphicText.text = $"Local Id: {DataStore.LocalUserID}\n Stored Online: {_storedOnline}";
		PlayFabGameState.OnGameStateSaved -= GameStateSaved;
	}
}
