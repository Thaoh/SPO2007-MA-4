using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabGameDataLoaded : MonoBehaviour {
	[SerializeField] private TMP_Text _dataLoaded;

	private void Awake() {
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

}