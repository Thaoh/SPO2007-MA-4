using TMPro;
using UnityEngine;

public class UGSFriend : MonoBehaviour {
	[SerializeField] private TMP_Text _friendNameElement;
	private string _friendName;

	public string Name {
		get {
			return _friendName;
		}
		
		set {
			_friendName = value; _friendNameElement.text = _friendName;
		}
	}
}
