using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public static string LocalUserID;
	
	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
		
		if (PlayerPrefs.HasKey("GUID")) {
        	// 0f8fad5b-d9cb-469f-a165-70867728950e
			LocalUserID = PlayerPrefs.GetString("GUID");
        } else {
			LocalUserID = System.Guid.NewGuid().ToString();
			
        	PlayerPrefs.SetString("GUID", LocalUserID);
        }
	}
}