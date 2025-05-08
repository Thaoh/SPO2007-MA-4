using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudCode;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Apis.Leaderboards;
using Unity.Services.Leaderboards; // Recommended for handling JSON responses

public class NotificationSender : MonoBehaviour {
	[SerializeField] private string _leaderBoardId = "NoNameGame"; // Replace with your leaderboard ID
	
	private void Awake() {
		FriendsListService.OnFriendChallengeRequest += ChallengeYourFriend;
	}

	private void OnDestroy() {
		FriendsListService.OnFriendChallengeRequest -= ChallengeYourFriend;
	}

	private const string CloudCodeFunctionName = "NotificationOfChallenge"; // Match the name of your Cloud Code script
	
	private async void ChallengeYourFriend(string playerId) {
		// Get the player's current score from the leaderboard'
		double score = 0;
		try
		{
			// Attempt to get the player's score
			var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(_leaderBoardId);
			score = scoreResponse.Score;
			// If the code reaches here, the player's score was found
			Debug.Log($"Successfully retrieved score for player {playerId}: {scoreResponse.Score}");
		}
		catch (System.Exception e)
		{
			// Catch any other unexpected exceptions
			Debug.LogError($"An unexpected error occurred while getting player score: {e.Message}");
		}
		
		var notificationMessage = $"{AuthenticationService.Instance.PlayerName} challenges you to beat their score of {score} points. Click here to accept the challenge.!";
		SendNotificationAsync(playerId, notificationMessage);
	}

	// Call this method from your game whenever you want to send a notification
	public async Task SendNotificationAsync(string targetUserId, string notificationMessage) {
		if (!AuthenticationService.Instance.IsSignedIn) {
			Debug.LogError("User is not signed in. Cannot send notification.");
			return;
		}

		try {
			// Prepare the arguments to pass to the Cloud Code function
			var args = new Dictionary<string, object> {
				{ "challengedPlayerId", targetUserId },
				{ "message", notificationMessage }
			};

			Debug.Log($"Calling Cloud Code function: {CloudCodeFunctionName} with args: {JsonConvert.SerializeObject(args)}");

			// Call the Cloud Code function
			var resultJson = await CloudCodeService.Instance.CallEndpointAsync(CloudCodeFunctionName, args);

			Debug.Log($"Cloud Code response: {resultJson}");

			// Parse the JSON response from Cloud Code
			var response = JsonConvert.DeserializeObject<CloudCodeResponse>(resultJson);

			if (response.success) {
				Debug.Log($"Notification request successful: {response.message}");

				// **Handle the notification data received from Cloud Code**
				// This is where you would trigger your in-app notification UI
				// using the data in response.notificationData.
				if (response.notificationData != null) {
					Debug.Log($"Received notification data: Message = {response.notificationData.message}, Timestamp = {response.notificationData.timestamp}");
					// Example: Trigger a UI element to display the message
					DisplayInAppNotification(response.notificationData.message);
				}
			} else {
				Debug.LogError($"Notification request failed: {response.message}");
			}
		} catch (CloudCodeException e) {
			Debug.LogError($"Cloud Code error: {e.ErrorCode} - {e.Message}");
		} catch (Exception e) {
			Debug.LogError($"Error calling Cloud Code: {e.Message}");
		}
	}

	// Helper class to deserialize the Cloud Code response
	[Serializable]
	public class CloudCodeResponse {
		public bool success;
		public string message;
		public NotificationData notificationData;
	}

	[Serializable]
	public class NotificationData {
		public string message;
		public long timestamp;
	}

	// **Implement this method to display the notification in your game**
	private void DisplayInAppNotification(string message) {
		Debug.Log($"Displaying in-app notification: {message}");
		// Your UI logic to show the notification goes here.
		// This could be showing a popup, adding to a chat feed, etc.
	}

	// Example usage:
	// Call this from another script or a button click
	public void ExampleSendNotification() {
		// Replace with the actual target user ID and message
		var targetUser = AuthenticationService.Instance.PlayerId; // Sending to self for testing
		var messageToSend = "Hello from Cloud Code!";
		SendNotificationAsync(targetUser, messageToSend);
	}
}