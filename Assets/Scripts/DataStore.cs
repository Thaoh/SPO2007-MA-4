using UnityEngine;

public class DataStore : MonoBehaviour {
	public static DataStore Instance;
	public static string LocalUserID;
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
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