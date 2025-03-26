using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private string leaderboardId = "Scores";


    public void AddScoreToLeaderboard()
    {
        _ = AddScore();
    }
    
    public async Task AddScore()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
            "Name", "Score"
        });
        if (playerData.TryGetValue("Score", out var score));
        Debug.Log("LeaderboardManager test");
        if (score != null)
        {
            var playerEntry = await LeaderboardsService.Instance
                .AddPlayerScoreAsync(leaderboardId, score.Value.GetAs<double>());
            Debug.Log(JsonConvert.SerializeObject(playerEntry));
        }
    }


    public async void GetScores()
    {
        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(leaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}
