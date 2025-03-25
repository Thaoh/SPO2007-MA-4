using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public static string UserId;
	
	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
		if (PlayerPrefs.HasKey("GUID")) {
        	// 0f8fad5b-d9cb-469f-a165-70867728950e
			GameManager.UserId = PlayerPrefs.GetString("GUID");
        } else {
			GameManager.UserId = System.Guid.NewGuid().ToString();
        	PlayerPrefs.SetString("GUID", GameManager.UserId);
        }
	}
}