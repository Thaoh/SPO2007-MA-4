using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class FriendsListService : MonoBehaviour {
	[SerializeField] private GameObject _friendPrefab;
	[SerializeField] private GameObject _friendsContainer;
	[SerializeField] private TMP_Text _playerNameContainer;
	
	public async void Intialize() {
		_playerNameContainer.text = AuthenticationService.Instance.PlayerId;

		await UpdateFriendsList();
	}

	private async Task UpdateFriendsList() {
		ClearFriendsListContainer();
		
		// Initialize friends service
		await FriendsService.Instance.InitializeAsync();

		// Start using the Friends SDK functionalities.
		var friends = FriendsService.Instance.Friends;
		foreach (var friend in friends) {
			CreateFriendEntry(friend.Member.Profile.Name);
		}
	}

	private void CreateFriendEntry(string friend, bool isFriend = true) {
		GameObject friendGO = Instantiate(_friendPrefab, _friendsContainer.transform,false);
		
		if (friendGO.TryGetComponent(out UGSFriend friendComponent )) {
			friendComponent.Name = friend;
			friendComponent.SetFriendMode((isFriend) ? FriendMode.Friends : FriendMode.NotFriends);
		}
	}

	public async void LookForFriendsList() {
		ClearFriendsListContainer();
		Debug.Log("Looking for friends list");
		
		LeaderboardScoresPage leaderboardTask = await LeaderboardManagerv2.Instance.GetLeaderboard();
		var leaderboard = leaderboardTask.Results;
		
		if (leaderboard == null || leaderboard.Count == 0) {
			Debug.Log("No possible found.");
			return;
		}
		
		leaderboard.ForEach(friend => {
			CreateFriendEntry(friend.PlayerName, false);
		}); 
	}

	private void ClearFriendsListContainer() {
		if (_friendsContainer.transform.childCount == 0) {
			return;
		}
		
		foreach (Transform child in _friendsContainer.transform) {
			Destroy(child.gameObject);
		}
	}
}