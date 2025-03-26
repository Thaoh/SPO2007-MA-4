using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabGameState : MonoBehaviour {
	private static string _playFabId;
	public static PlayFabGameState Instance;
	public static GetUserDataResult LoadedUserData;

	public static Action<GetUserDataResult> OnGameStateLoaded;
	public static Action<bool> OnGameStateSaved;
	
	
	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(gameObject);
		}
		
	}

	public static string GetData(string searchValue) {
		if (Instance != null) {
			if (LoadedUserData != null && LoadedUserData.Data.Count > 0) {
				foreach (var entry in LoadedUserData.Data) {
					if (searchValue == entry.Key) {
						return entry.Value.Value;
					}
				}
			} else {
				Debug.LogError("No Loaded User Data!");
			}
			
			return null;
		}
		return null;
	}
	
#region Save Game State
	public static void SaveGame(string key, string value) {
		if (Instance != null) {
			var request = new UpdateUserDataRequest {
				Data = new System.Collections.Generic.Dictionary<string, string> {
					{ key, value }
				},
				Permission = UserDataPermission.Public // Or Private if it's sensitive data
			};

			PlayFabClientAPI.UpdateUserData(request, Instance.OnSaveDataSuccess, Instance.OnSaveDataError);
		}
	}

	private void OnSaveDataSuccess(UpdateUserDataResult result) {
		Debug.Log("Successfully saved user data.");
		OnGameStateSaved?.Invoke(true);
	}

	private void OnSaveDataError(PlayFabError error) {
		Debug.LogError("Failed to save user data: " + error.GenerateErrorReport());
		OnGameStateSaved?.Invoke(false);
	}
#endregion
	
#region Load Game State
	public static void LoadGame() {
		if (Instance != null) {
			PlayFabClientAPI.GetUserData(new GetUserDataRequest(), Instance.OnLoadDataSuccess, Instance.OnLoadDataError);
		}
	}
	private void OnLoadDataSuccess(GetUserDataResult result) {
		Debug.Log("Successfully retrieved user data.");

		if (result.Data != null) {
			LoadedUserData = result;
			//foreach (var entry in result.Data) {
			//	Debug.Log("Key: " + entry.Key + ", Value: " + entry.Value.Value);
				// Process the data as needed. For example:
				// if (entry.Key == "Level") { playerLevel = int.Parse(entry.Value.Value); }
			//}

			OnGameStateLoaded?.Invoke(result);
		} else {
			Debug.Log("No user data found.");
		}
	}

	private void OnLoadDataError(PlayFabError error) {
		Debug.LogError("Failed to load user data: " + error.GenerateErrorReport());
	}
#endregion
}