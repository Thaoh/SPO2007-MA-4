using System.Threading.Tasks;
using PlayFab.ClientModels;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class DataStore : MonoBehaviour {
	public static DataStore Instance;
	public static string LocalUserID;
	public static LoginResult PlayFabLoginResult;
	
	public static LeaderboardScoresPage Leaderboard;
	
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