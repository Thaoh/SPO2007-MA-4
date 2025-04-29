using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Friends.Exceptions;
using Unity.Services.Leaderboards.Models;
using UnityEditor.VersionControl;
using UnityEngine;
using Task = System.Threading.Tasks.Task;
using Newtonsoft.Json;
using Unity.Services.Apis.Friends;
using Unity.Services.Core;
using Unity.Services.Friends.Internal;

public class FriendsListService : MonoBehaviour {
	[SerializeField] private GameObject _friendPrefab;
	[SerializeField] private GameObject _friendsContainer;
	[SerializeField] private TMP_Text _playerNameContainer;
	
	private Dictionary<string, UGSFriend> _friendObjects = new ();
	
	public async void Intialize () {
		_playerNameContainer.text = AuthenticationService.Instance.PlayerName;

		await UpdateFriendsList();
	}
	
	// Event handler for received friend requests
    void OnFriendRequestsReceived(List<Relationship> friendRequests) {
        Debug.Log($"[Friend Request] Received {friendRequests.Count} new friend requests.");
        foreach (var request in friendRequests) {
            // Debug.Log($"- Request from User ID: {request.RequesterId}, Status: {request.Status}, Type: {request.Type}");
            // You'll typically update your UI to display these incoming requests
            // The 'request.RequesterId' is the ID of the user who sent the request.
        }
    }

    // Event handler for relationship changes
    void OnRelationshipChanged(Relationship relationship) {
        Debug.Log($"[Relationship Changed] Relationship changed: User ID: {relationship.Id}, Members: {relationship.Members}, Type: {relationship.Type}");
        // This event is triggered for various relationship changes:
        // - When a friend request is accepted or declined
        // - When a friend's status changes (online, offline, in game, etc.)
        // - When a user is unfriended

        // You'll likely want to update your UI based on the 'relationship.Status' and 'relationship.Type'
        // 'relationship.Id' is the User ID of the friend/requester whose relationship changed.
    }

    // Keep your existing GetAndDisplayFriendsList method
    
	void OnDisable() {
		//IFriendsServiceInternal friendsServiceInternal = (IFriendsServiceInternal) FriendsService.Instance;
		//friendsServiceInternal.MessageReceived -= OnMessageReceived;
		
		ClearFriendsListContainer();
	}

    void OnMessageReceived(Message message) {
        Debug.Log($"[Message Received] Received a raw message object: {message}");
        Debug.Log($"[Message Received] Type of the raw message object: {message.GetType()}");

        // Attempt to serialize the message object to JSON to inspect its structure
        try
        {
            string messageJson = JsonConvert.SerializeObject(message, Formatting.Indented);
            Debug.Log($"[Message Received] Raw message JSON: {messageJson}");

            // Based on the JSON structure, you'll need to determine how to access
            // the topic and payload data.
            // It's highly likely that the "Topic" and "Payload" are properties
            // of the message object, but they might not be directly exposed publicly
            // in the 'Message' class itself.

            // You might need to parse the JSON or cast the message object
            // to a more specific internal type if you can identify it.
            // This is where debugging and potentially looking at the package's source
            // code (if available) is crucial.

            // --- Example of how you might try to access data after inspecting JSON ---
            // This is a speculative example based on potential internal structures.
            // Replace this with how the JSON structure actually looks.

            // If the JSON shows a "Topic" property:
            // string topic = ??? // Access the topic value based on the JSON structure

            // If the JSON shows a "Payload" property:
            // object payload = ??? // Access the payload value based on the JSON structure

            // After identifying the topic and payload:
            // if (topic == "FRIEND_REQUEST_RECEIVED")
            // {
            //     Debug.Log("Identified FRIEND_REQUEST_RECEIVED message from raw data.");
            //     // Now, try to access the payload data based on the JSON structure
            //     // For example, if the payload is a dictionary with a "Relationship" key:
            //     // if (payload is Dictionary<string, object> payloadData)
            //     // {
            //     //     // ... access Relationship and RequesterId as before ...
            //     // }
            // }
            // else if (topic == "RELATIONSHIP_UPDATED")
            // {
            //     Debug.Log("Identified RELATIONSHIP_UPDATED message from raw data.");
            //     // Handle relationship updates
            // }
            // ... handle other topics ...

        }
        catch (Exception ex)
        {
            Debug.LogError($"[Message Received] Error serializing or processing raw message: {ex.Message}");
        }
    }

	private async Task UpdateFriendsList() {
		ClearFriendsListContainer();
		
		// Initialize friends service
		await FriendsService.Instance.InitializeAsync();

		// Start using the Friends SDK functionalities.
		var friends = FriendsService.Instance.Friends;

		var requests = FriendsService.Instance.IncomingFriendRequests;

		foreach (Unity.Services.Friends.Models.Relationship relationship in requests) {
			CreateFriendEntry(relationship.Member.Profile.Name, FriendMode.Requested);
		}
		
		foreach (var friend in friends) {
			CreateFriendEntry(friend.Member.Profile.Name, FriendMode.Friends);
		}
	}

	private void CreateFriendEntry(string friend, FriendMode friendMode) {
		GameObject friendGO = Instantiate(_friendPrefab, _friendsContainer.transform,false);
		
		if (friendGO.TryGetComponent(out UGSFriend friendComponent )) {
			friendComponent.Name = friend;
			friendComponent.PlayerID = friend;
			friendComponent.SetFriendMode(friendMode);
			
			friendComponent.OnBefriendPerson += SendFriendRequest;
			friendComponent.OnFriendChallengeRequest += ChallengeFriend;
		}
	}

	private void SendFriendRequest(string userID) {
		Debug.LogError($"[Friends Requests] Player {AuthenticationService.Instance.PlayerName} wants to change the relationship with {userID}.");
		
		AddFriend(userID);
	}
	
	private void ChallengeFriend(string userID) {
		Debug.LogError($"[Friends Challenge] Player {userID} challenged.");
	}

	public async void FriendsDiscovery() {
		ClearFriendsListContainer();
		Debug.Log("[Friends Discovery] Getting leaderboards...");
		
		LeaderboardScoresPage leaderboardTask = await LeaderboardManagerv2.Instance.GetLeaderboard();
		var leaderboard = leaderboardTask.Results;
		
		if (leaderboard == null || leaderboard.Count == 0) {
			Debug.Log("[Friends Discovery] No leaderboard found.");
			return;
		}
		
		Debug.Log("[Friends Discovery] Building list of possible future friends.");
		leaderboard.ForEach(friend => {
			CreateFriendEntry(friend.PlayerName, FriendMode.NotFriends);
		}); 
	}

	private async void RemoveFriend(string playerId) {
		try {
			await FriendsService.Instance.DeleteFriendAsync(playerId);
		} catch (ArgumentException e) {
			Debug.Log("[Friend Removal] An argument exception occurred with message: " + e.Message);
		} catch (FriendsServiceException e) {
			Debug.Log("[Friend Removal] An error occurred while performing the action. Code: " + e + ", Message: " + e.Message);
		}
	}

	private async void AddFriend(string playerId) {
		try {
			await FriendsService.Instance.AddFriendAsync(playerId);
		} catch (ArgumentException e) {
			Debug.Log("[Friend Add] An argument exception occurred with message: " + e.Message);
		} catch (FriendsServiceException e) {
			Debug.Log("[Friend Add] An error occurred while performing the action. Code: " + e + ", Message: " + e.Message);
		}
	}
	
	private void ClearFriendsListContainer() {
		if (_friendsContainer.transform.childCount == 0) {
			return;
		}
		
		foreach (Transform child in _friendsContainer.transform) {
			if (child.TryGetComponent(out UGSFriend friendComponent )) {
				friendComponent.OnBefriendPerson -= SendFriendRequest;
				friendComponent.OnFriendChallengeRequest -= ChallengeFriend;
			}
			
			Destroy(child.gameObject);
		}
	}
}
