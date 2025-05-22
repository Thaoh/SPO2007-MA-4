using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudCode;

public class NotificationPropagator : MonoBehaviour {
	[SerializeField] private double _notifDelay = 0.002d;
	[SerializeField] private float _checkDelay = 5f;

	private float _nextCheckTime = 0f;
	private bool _currentlyLoadingData;

	private void Awake() {
		CloudCodePushExample.OnMessageRecieved += OnMessageRecieved;
		UGSAuthenticator.OnAuthFinishedSuccess += SetupToNotifications;
	}

	private void OnDestroy() {
		CloudCodePushExample.OnMessageRecieved -= OnMessageRecieved;
	}
	private async void SetupToNotifications() {
		AndroidNotifcations.RequestPermission();
		AndroidNotifcations.RegisterNotificationChannel();
		_nextCheckTime = Time.realtimeSinceStartup + _checkDelay;
		await LoadData();
	}

	private async void Update() {
		if (_nextCheckTime == 0) {
			SetupToNotifications();
		}

		if (_nextCheckTime != 0 && Time.realtimeSinceStartup > _nextCheckTime) {
			await LoadData();
		}
	}

	public async Task LoadData() {
		if (!UGSAuthenticator.IsAuthenticated || _currentlyLoadingData) {
			return;
		}

		_currentlyLoadingData = true;
		
		var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
			"incomingChallenge"
		});
		
		if (playerData.TryGetValue("incomingChallenge", out var incomingMessage)) {
			var notificationMessage = incomingMessage.Value.GetAs<string>();
			Debug.Log($"incomingChallenge value: {notificationMessage}");
			SendNotificationToPhone(notificationMessage);
			DeleteData("incomingChallenge");
		}
		
		_nextCheckTime = Time.realtimeSinceStartup + _checkDelay;
		_currentlyLoadingData = false;
	}
	public async void DeleteData(string key) {
		await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
	}
	private void SendNotificationToPhone(string notificationMessage) {
		AndroidNotifcations.SendNotification("Beat The Tunnel", notificationMessage, _notifDelay);
	}
	
	private void OnMessageRecieved(Unity.Services.CloudCode.Subscriptions.IMessageReceivedEvent obj) {
		AndroidNotifcations.SendNotification("Beat The Tunnel", obj.Message, _notifDelay);
	}
}