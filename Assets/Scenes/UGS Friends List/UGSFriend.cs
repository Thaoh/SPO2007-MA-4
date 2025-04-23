using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UGSFriend : MonoBehaviour {
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

	public void SetFriendMode(FriendMode mode) {
		switch (mode) {
			case FriendMode.Friends:
				_interactionButton.gameObject.SetActive(true);
				_interactionButtonText.text = "Unfriend";
				_challengeButton.gameObject.SetActive(true);
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
}

public enum FriendMode {
	None,
	Friends,
	NotFriends
}
