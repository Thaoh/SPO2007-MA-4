using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UGSFriend : MonoBehaviour {
	public UnityAction<string> OnFriendChallengeRequest = delegate { };
	public UnityAction<string> OnBefriendPerson = delegate { };
	
	[SerializeField] private TMP_Text _friendNameElement;
	[SerializeField] private Button _interactionButton;
	[SerializeField] private TMP_Text _interactionButtonText;
	[SerializeField] private Button _challengeButton;
	[SerializeField] private TMP_Text _challengeButtonText;
	
	private string _friendName;

	public string Name {
		get {
			return _friendName;
		}
		
		set {
			_friendName = value; _friendNameElement.text = _friendName;
		}
	}
	
	public string PlayerID { get; set; }

	public void SetFriendMode(FriendMode mode) {
		switch (mode) {
			case FriendMode.Friends:
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Unfriend";
				_challengeButton.gameObject.SetActive(true);
				break;
			case FriendMode.Requested:
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Requested";
				_challengeButton.gameObject.SetActive(false);
				break;
			default:
			case FriendMode.None:
			case FriendMode.NotFriends:
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Befriend";
				_challengeButton.gameObject.SetActive(false);
				break;
		}
	}
	
	public void ChallengeFriend() {
		OnFriendChallengeRequest?.Invoke(PlayerID);
	}
	
	public void FriendRequest() {
		OnBefriendPerson?.Invoke(PlayerID);
	}
}

public enum FriendMode {
	None,
	Friends,
	NotFriends,
	Requested
}
