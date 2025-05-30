using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.Subscriptions;
using Unity.Services.Core;
using UnityEngine;

namespace CloudCode
{
    public class CloudCodePushExample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public static string message = string.Empty;
        public static event Action<IMessageReceivedEvent> OnMessageRecieved;
        private void Awake() 
        {
            UGSAuthenticator.OnAuthFinishedSuccess += Subscribe;
        }

        async void Subscribe()
        {
            AndroidNotifcations.RequestPermission();
            AndroidNotifcations.RegisterNotificationChannel();
            await SubscribeToPlayerMessages();
        }

        // This method creates a subscription to project messages and logs out the messages received,
        // the state changes of the connection, when the player is kicked and when an error occurs.
        private Task SubscribeToPlayerMessages()
        {
            var callbacks = new SubscriptionEventCallbacks();
            callbacks.MessageReceived += @event =>
            {
                Debug.Log(DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK"));
                Debug.Log($"Got project subscription Message: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
                if (textMeshProUGUI) textMeshProUGUI.text = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK") + "\n" +
                $"Got project subscription Message: {JsonConvert.SerializeObject(@event, Formatting.Indented)}";

                TriggerMessageRecieved(@event);
            };
            callbacks.ConnectionStateChanged += @event => 
            {
                Debug.Log($"Got project subscription ConnectionStateChanged: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
                if (textMeshProUGUI) textMeshProUGUI.text = $"Got project subscription Message: {JsonConvert.SerializeObject(@event, Formatting.Indented)}";
            };
            callbacks.Kicked += () =>
            {
                Debug.Log($"Got project subscription Kicked");
                if (textMeshProUGUI) textMeshProUGUI.text = $"Got project subscription Kicked";
            };
            callbacks.Error += @event =>
            {
                Debug.Log($"Got project subscription Error: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");
                if (textMeshProUGUI) textMeshProUGUI.text = $"Got project subscription Error: {JsonConvert.SerializeObject(@event, Formatting.Indented)}";
            };
            return CloudCodeService.Instance.SubscribeToPlayerMessagesAsync(callbacks);
        }

        private void TriggerMessageRecieved(IMessageReceivedEvent message)
        {
            // Make sure we're on the main thread for UI updates
            if (OnMessageRecieved != null)
            {
                OnMessageRecieved.Invoke(message);
            }

        }
    }

}
