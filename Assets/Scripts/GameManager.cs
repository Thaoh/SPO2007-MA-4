using PlayFab.ClientModels;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public static string LocalUserID;
	public static LoginResult PlayFabLoginResult;
	
	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(gameObject);
		}
		
		LocalUserID = System.Guid.NewGuid().ToString();
	}
}