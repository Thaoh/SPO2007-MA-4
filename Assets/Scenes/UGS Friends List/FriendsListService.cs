using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using UnityEngine;

public class FriendsListService : MonoBehaviour {
	[SerializeField] private GameObject _friendPrefab;
	[SerializeField] private GameObject _friendsContainer;
	[SerializeField] private TMP_Text _playerNameContainer;
	public async void Intialize() {
		
		_playerNameContainer.text = AuthenticationService.Instance.PlayerId;
		
		// Initialize friends service
		await FriendsService.Instance.InitializeAsync();

		// Start using the Friends SDK functionalities.
		var friends = FriendsService.Instance.Friends;
		foreach (var friend in friends) {
			CreateFriend(friend.Member.Profile.Name);
		}
	}

	private void CreateFriend(string friend) {
		GameObject friendGO = Instantiate(_friendPrefab, _friendsContainer.transform,false);
		if (friendGO.TryGetComponent(out UGSFriend friendComponent )) {
			friendComponent.Name = friend;
		}
	}
}