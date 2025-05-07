

using CloudCode;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardNotifications : MonoBehaviour
{
    [SerializeField] private double notifDelay = 0.002d;
    [SerializeField] private TMP_InputField inputField = null;

    private void Start()
    {
        CloudCodePushExample.OnMessageRecieved += OnMessageRecieved;
    }

    private void OnMessageRecieved(Unity.Services.CloudCode.Subscriptions.IMessageReceivedEvent obj)
    {
        AndroidNotifcations.SendNotification("Beat The Tunnel", obj.Message, notifDelay);

    }

    public void SubmitScoreLeaderboard()
    {
        SubmitScore("Test123", double.Parse(inputField.text));
    }
    public async void SubmitScore(string leaderboardId, double score)
    {
        var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);

    }
}