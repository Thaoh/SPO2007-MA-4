using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UGSFriend : MonoBehaviour {
	public UnityAction<string> OnFriendChallengeRequest = delegate { };
	public UnityAction<string> OnBefriendPerson = delegate { };
	public UnityAction<string> OnUnfriend = delegate { };
	public UnityAction<string,string> OnCancelFriendRequest = delegate { };
	
	[SerializeField] private TMP_Text _friendNameElement;
	[SerializeField] private Button _interactionButton;
	[SerializeField] private TMP_Text _interactionButtonText;
	[SerializeField] private Button _challengeButton;
	[SerializeField] private TMP_Text _challengeButtonText;
	[Header("Background Colours")]
	[SerializeField] private Image _backgroundImage;
	[SerializeField] private Color _friendColor = new Color(12, 112, 192, 255);
	[SerializeField] private Color _notFriendColor = new Color(192, 112, 12, 130);
	[SerializeField] private Color _requestedColor = new Color(192, 112, 12, 255);
	[SerializeField] private Color _receivedColor = new Color(12, 112, 192, 130);
	
	private string _friendName;
	private FriendMode _friendMode;
	
	public string Name {
		get {
			return _friendName;
		}
		
		set {
			_friendName = value; _friendNameElement.text = _friendName;
		}
	}
	
	public string PlayerID { get; set; }

	public void SetBackgroundColor(Color color) {
		
		_backgroundImage.color = color;
	}
	
	public void SetFriendMode(FriendMode mode) {
		
		_friendMode = mode;
		switch (mode) {
			case FriendMode.Friends:
				SetBackgroundColor(_friendColor);
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Unfriend";
				_challengeButton.gameObject.SetActive(true);
				break;
			case FriendMode.Requested:
				SetBackgroundColor(_requestedColor);
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Delete Friend Request";
				_challengeButton.gameObject.SetActive(false);
				break;
			case FriendMode.Received:
				SetBackgroundColor(_receivedColor);
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Accept Request";
				_challengeButton.gameObject.SetActive(false);
				break;
			default:
			case FriendMode.None:
			case FriendMode.NotFriends:
				SetBackgroundColor(_notFriendColor);
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Add Friend";
				_challengeButton.gameObject.SetActive(false);
				break;
		}
	}
	
	public void ChallengeFriend() {
		OnFriendChallengeRequest?.Invoke(PlayerID);
	}
	
	public void FriendRequest() {
		switch ( _friendMode ) {
			case FriendMode.Friends:
				OnUnfriend?.Invoke(PlayerID);
				break;
			case FriendMode.NotFriends:
				OnBefriendPerson?.Invoke(Name);
				break;
			case FriendMode.Requested:
				OnCancelFriendRequest?.Invoke(PlayerID, Name);;
				break;
			case FriendMode.Received:
				OnBefriendPerson?.Invoke(Name);
				break;
		}
	}
}

public enum FriendMode {
	None,
	Friends,
	NotFriends,
	Requested,
	Received
}
