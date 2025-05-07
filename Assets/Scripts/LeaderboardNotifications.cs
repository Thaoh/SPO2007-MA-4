
using CloudCode;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardNotifications : MonoBehaviour
{
    [SerializeField] private double notifDelay = 0.002d;

    private void Start()
    {
        CloudCodePushExample.OnMessageRecieved += OnMessageRecieved;
        FriendsListService.OnFriendChallengeRequest += SendChallenge;
    }

    private void OnMessageRecieved(Unity.Services.CloudCode.Subscriptions.IMessageReceivedEvent obj)
    {
        AndroidNotifcations.SendNotification("Beat The Tunnel", obj.Message, notifDelay);

    }

    private void SendChallenge(string playerId)
    {
        Debug.Log("oh nothing happens on sending challenge great");
        //string message = AuthenticationService.Instance.PlayerName + " challenges you!";
        //Dictionary<string, object> payload = new Dictionary<string, object>
        //    {
        //        { "targetPlayerId", playerId },
        //        { "message", message }
        //    };

        //CloudCodeService.Instance.CallEndpointAsync("SendMessageToPlayer", payload);
    }
    private void OnDestroy()
    {
        FriendsListService.OnFriendChallengeRequest -= SendChallenge;
    }
}