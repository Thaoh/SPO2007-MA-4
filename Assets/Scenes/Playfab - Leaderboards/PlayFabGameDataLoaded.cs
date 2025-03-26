using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabGameDataLoaded : MonoBehaviour {
	[SerializeField] private TMP_Text _dataLoaded;
	[SerializeField] private GameObject _authButton; 
	
	private void Awake() {
		if (PlayFabGameState.Instance == null) {
			_dataLoaded.text = $"In order to load data, you must be logged in. Please return to User Authentication to log in.";
			_authButton.gameObject.SetActive(true);
			return;
		}
		PlayFabGameState.OnGameStateLoaded += HandleGameStateLoaded;
		PlayFabGameState.LoadGame();
	}

	private void HandleGameStateLoaded(GetUserDataResult obj) {
		string dataFromInternet = PlayFabGameState.GetData("LocalUserID");

		if (dataFromInternet is not (null or "")) {
			_dataLoaded.text = $"Loaded Game State: \n{dataFromInternet}";
		}
		PlayFabGameState.OnGameStateLoaded -= HandleGameStateLoaded;
	}

	public void ReturnToAuthentication() {
		SceneManager.LoadScene("Playfab - User Authentication");
	}
}