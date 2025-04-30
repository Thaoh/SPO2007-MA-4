using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using Unity.Services.Friends.Exceptions;
using Unity.Services.Friends.Models;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class FriendsListService : MonoBehaviour {
	public static FriendsListService Instance;

	[SerializeField] private GameObject _friendPrefab;
	[SerializeField] private GameObject _friendsContainer;
	[SerializeField] private TMP_Text _playerNameContainer;

	private float _nextInitializationTime;
	private readonly Dictionary<string, UGSFriend> _friendElement = new();
	private readonly Dictionary<string, Relationship> _requestSent = new();
	private readonly Dictionary<string, Relationship> _friendRelationships = new();
	private IReadOnlyList<Relationship> _friends;
	private IReadOnlyList<Relationship> _friendRequestsOutgoing;
	private IReadOnlyList<Relationship> _friendRequestsIncoming;

	private void Awake() {
		if ( Instance == null ) {
			Instance = this;
		} else {
			Destroy( gameObject );
		}

		if (!UGSAuthenticator.IsAuthenticated) {
			UGSAuthenticator.OnAuthFinishedSuccess += OnAuthenticationSuccessful;
		} else {
			OnAuthenticationSuccessful();
		}
	}

	/// <summary>
	/// Handles the successful authentication process.
	/// This method is triggered upon successful user authentication and initiates the asynchronous
	/// initialization of the Friends List Service to prepare the UI and populate the user's friends list.
	/// </summary>
	private void OnAuthenticationSuccessful() {
		_ = InitializeAsync();
	}

	/// <summary>
	/// Initializes the Friends List Service asynchronously.
	/// Handles service initialization, updates the player's name in the UI, refreshes the friends list,
	/// and logs any potential errors during the initialization process.
	/// </summary>
	/// <returns>A task representing the asynchronous initialization operation.</returns>
	private async Task InitializeAsync() {
		Debug.Log( "Initialising Friends List Service" );

		// Initialize friends service
		try {
			await FriendsService.Instance.InitializeAsync();

			// Update UI on main thread
			_playerNameContainer.text = AuthenticationService.Instance.PlayerName;

			// Update friends list
			UpdateFriendsList();
		} catch ( Exception e ) {
			Debug.LogError( $"Friends service initialization error: {e}" );
			_nextInitializationTime = Time.time + 0.5f;
		}
	}

	/// <summary>
	/// Handles the cleanup process when the FriendsListService is disabled.
	/// Unsubscribes from the authentication success event and clears the friends list container to release resources.
	/// </summary>
	void OnDisable() {
		UGSAuthenticator.OnAuthFinishedSuccess -= OnAuthenticationSuccessful;
		ClearFriendsListContainer();
	}

	/// <summary>
	/// Handles the periodic update of the Friends List Service initialization process.
	/// If re-initialization is needed based on a time threshold, attempts to re-initialize the service.
	/// </summary>
	private void Update() {
		if ( _nextInitializationTime != 0 && Time.time > _nextInitializationTime ) {
			_nextInitializationTime = 0; // Reset to avoid repeated attempts
			_ = InitializeAsync();
		}
	}

	/// <summary>
	/// Updates the friends list by clearing the existing UI entries, retrieving the latest relationships from the FriendsService,
	/// and creating new UI entries for outgoing friend requests, incoming friend requests, and established friendships.
	/// </summary>
	public void UpdateFriendsList() {
		ClearFriendsListContainer();

		GetUpdatedRelationships();

		foreach ( Unity.Services.Friends.Models.Relationship relationship in _friendRequestsOutgoing ) {
			Debug.Log( $"[Friends List] Request sent to {relationship.Member.Profile.Name} id: {relationship.Member.Id}, ID: {relationship.Id}." );
			CreateFriendEntry( relationship.Member.Profile.Name, FriendMode.Requested, relationship.Member.Id );
		}

		foreach ( Unity.Services.Friends.Models.Relationship relationship in _friendRequestsIncoming ) {
			CreateFriendEntry( relationship.Member.Profile.Name, FriendMode.Received, relationship.Member.Id );
		}
		
		_friendElement.Clear();
		foreach ( var friend in _friends ) {
			Debug.Log( $"[Friends List] Friendship established with {friend.Member.Profile.Name} id: {friend.Member.Id}, ID: {friend.Id}." );
			CreateFriendEntry( friend.Member.Profile.Name, FriendMode.Friends, friend.Member.Id );
		}
	}

	/// <summary>
	/// Retrieves the latest relationship data from the FriendsService, including the lists of friends,
	/// incoming friend requests, and outgoing friend requests. Updates internal dictionaries to manage
	/// outgoing requests and existing friendships for easier reference and processing.
	/// </summary>
	private void GetUpdatedRelationships() {
		_friends = FriendsService.Instance.Friends;

		_friendRequestsIncoming = FriendsService.Instance.IncomingFriendRequests;
		_friendRequestsOutgoing = FriendsService.Instance.OutgoingFriendRequests;

		Debug.Log( $"[Friends List] Friends: {_friends.Count}, Requests: inc - {_friendRequestsIncoming.Count}/ out - {_friendRequestsOutgoing.Count}." );
		
		_requestSent.Clear();
		foreach ( Unity.Services.Friends.Models.Relationship relationship in _friendRequestsOutgoing ) {
			_requestSent.Add( relationship.Member.Profile.Name, relationship );
		}
		
		_friendRelationships.Clear();
		foreach ( var friend in _friends ) {
			_friendRelationships.Add( friend.Member.Profile.Name, friend );
		}
	}

	/// <summary>
	/// Creates a new friend entry in the Friends List UI with the specified details.
	/// This method initializes a UI element representing a friend and configures its
	/// associated actions such as sending requests, canceling requests, and removing friends.
	/// </summary>
	/// <param name="friend">The name of the friend to be displayed.</param>
	/// <param name="friendMode">The current friendship mode (e.g., Friends, Requested, Received).</param>
	/// <param name="friendId">The unique identifier of the friend.</param>
	private void CreateFriendEntry( string friend, FriendMode friendMode, string friendId ) {
		GameObject friendGO = Instantiate( _friendPrefab, _friendsContainer.transform, false );

		if ( _requestSent.ContainsKey( friend ) ) {
			friendMode = FriendMode.Requested;
		}

		if ( friendGO.TryGetComponent( out UGSFriend friendComponent ) ) {
			friendComponent.Name = friend;
			friendComponent.PlayerID = friend;
			friendComponent.SetFriendMode( friendMode );

			friendComponent.OnBefriendPerson += SendFriendRequest;
			friendComponent.OnFriendChallengeRequest += ChallengeFriend;
			friendComponent.OnUnfriend += RemoveFriend;
			friendComponent.OnCancelFriendRequest += CancelFriendRequest;
			
			
			if (friendMode == FriendMode.NotFriends) {
				_friendElement.Add( friend, friendComponent );
			}
		}
	}

	private void UpdateFriendEntry(string friend, FriendMode friendMode) {
		if (_friendElement.ContainsKey(friend)) {
			_friendElement[friend].SetFriendMode(friendMode);
		}
	}

	/// <summary>
	/// Sends a friend request to a specified user.
	/// This method verifies if a request has not already been sent or the user is not already a friend,
	/// then initiates the process to add the user as a friend.
	/// </summary>
	/// <param name="userID">The unique identifier of the user to whom the friend request will be sent.</param>
	private void SendFriendRequest( string userID ) {
		Debug.LogError( $"[Friends Requests] Player {AuthenticationService.Instance.PlayerName} requests friendship with {userID}." );

		if ( !_requestSent.ContainsKey( userID ) && !_friendRelationships.ContainsKey( userID ) ) {
			AddFriend( userID );
		}
	}

	private void IgnoreFriendRequest( string userID ) {
		FriendsService.Instance.DeleteIncomingFriendRequestAsync( userID );
	}

	private void CancelFriendRequest( string userID ) {
		Debug.LogError( $"[Friends Requests] Player {AuthenticationService.Instance.PlayerName} cancels friend request with {userID}." );
		FriendsService.Instance.DeleteOutgoingFriendRequestAsync( userID );
		UpdateFriendEntry(userID, FriendMode.NotFriends);
	}

	private void ChallengeFriend( string userID ) {
		Debug.LogError( $"[Friends Challenge] Player {userID} challenged." );
	}

	/// <summary>
	/// Executes the Friends Discovery process by retrieving leaderboard information and processing it
	/// to build a list of potential future friends. This method asynchronously fetches leaderboard scores,
	/// iterates through the results, and generates friend entries in the UI for users not currently in the friend list.
	/// </summary>
	public async void FriendsDiscovery() {
		ClearFriendsListContainer();
		Debug.Log( "[Friends Discovery] Getting leaderboards..." );

		LeaderboardScoresPage leaderboardTask = await LeaderboardManagerv2.Instance.GetLeaderboard();
		var leaderboard = leaderboardTask.Results;

		if ( leaderboard == null || leaderboard.Count == 0 ) {
			Debug.Log( "[Friends Discovery] No leaderboard found." );

			return;
		}

		Debug.Log( "[Friends Discovery] Building list of possible future friends." );
		leaderboard.ForEach( friend => {
			if (friend.PlayerName != AuthenticationService.Instance.PlayerName) {
				//Debug.Log($"[FriendsDiscovery] Built fd list {friend.PlayerName} id: {friend.Metadata}, ID: {friend.PlayerId}.");
				CreateFriendEntry( friend.PlayerName, FriendMode.NotFriends, friend.PlayerId );
			}
		} );
	}

	/// <summary>
	/// Removes a friend from the user's friends list by sending a request to the Friends Service.
	/// This method handles any exceptions that may occur during the removal process and logs relevant messages for debugging.
	/// </summary>
	/// <param name="playerId">The unique identifier of the friend to be removed.</param>
	private async void RemoveFriend( string playerId ) {
		try {
			await FriendsService.Instance.DeleteFriendAsync( playerId );
			if (_friendElement.ContainsKey(playerId)) {
				Destroy(_friendElement[playerId].gameObject);
				_friendElement.Remove(playerId);
			}
		} catch ( ArgumentException e ) {
			Debug.Log( "[Friend Removal] An argument exception occurred with message: " + e.Message );
		} catch ( FriendsServiceException e ) {
			Debug.Log( "[Friend Removal] An error occurred while performing the action. Code: " + e + ", Message: " + e.Message );
		}
	}

	/// <summary>
	/// Adds a friend to the user's friends list by sending a friend request to the specified player ID.
	/// This method attempts to initiate the addition of a friend and handles exceptions that may occur
	/// during the process, including argument exceptions and errors specific to the Friends Service.
	/// </summary>
	/// <param name="playerId">The unique identifier of the player to be added as a friend.</param>
	private async void AddFriend( string playerId ) {
		try {
			await FriendsService.Instance.AddFriendByNameAsync( playerId );
			if (_friendRelationships.ContainsKey(playerId)) {
				UpdateFriendEntry(playerId, FriendMode.Friends);
			} else {
				UpdateFriendEntry(playerId, FriendMode.Requested);
			}
		} catch ( ArgumentException e ) {
			Debug.Log( "[Friend Add] An argument exception occurred with message: " + e.Message );
		} catch ( FriendsServiceException e ) {
			Debug.Log( "[Friend Add] An error occurred while performing the action. Code: " + e + ", Message: " + e.Message );
		}
	}

	/// <summary>
	/// Clears all child entries in the friends list container within the UI.
	/// This method ensures that all UI elements representing friends, outgoing requests,
	/// and incoming requests are removed from the container. Additionally, it unregisters
	/// any event handlers associated with the friend entries and destroys the corresponding GameObjects.
	/// </summary>
	private void ClearFriendsListContainer() {
		if ( _friendsContainer.transform.childCount == 0 ) {
			return;
		}

		foreach ( Transform child in _friendsContainer.transform ) {
			if ( child.TryGetComponent( out UGSFriend friendComponent ) ) {
				friendComponent.OnBefriendPerson -= SendFriendRequest;
				friendComponent.OnFriendChallengeRequest -= ChallengeFriend;
				friendComponent.OnUnfriend -= RemoveFriend;
				friendComponent.OnCancelFriendRequest -= CancelFriendRequest;
			}

			Destroy( child.gameObject );
		}
	}
}