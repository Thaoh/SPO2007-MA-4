using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using Unity.Services.CloudSave;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private string leaderboardId = "Scores";

    [SerializeField] private TMP_Text scoreText;


    private void Start()
    {
        Debug.Log("I Exist");
    }

    public void AddScoreToLeaderboard()
    {
        AddScore(leaderboardId);
    }
    
    public async void AddScore(string _leaderboardId)
    {
        string rawInput = scoreText.text;
        string cleanedInput = Regex.Replace(rawInput, @"[^\d.]", "");
        Debug.Log(cleanedInput);
        Debug.Log($"scoreText.text raw: [{cleanedInput}] (Length: {cleanedInput.Length})");
        foreach (char c in cleanedInput)
        {
            Debug.Log($"Char: '{c}' (Unicode: {(int)c})");
        }
        if (double.TryParse(cleanedInput, out double score))
        {
            var playerEntry = await LeaderboardsService.Instance
                .AddPlayerScoreAsync(_leaderboardId, score);
            //Debug.Log(JsonConvert.SerializeObject(playerEntry));
            Debug.Log("Score added");
        }
        else
        {
            Debug.Log("Something went wrong");
        }
        
    }


    public async void GetScores(string _leaderboardId)
    {
        var scoresResponse = await LeaderboardsService.Instance
            .GetScoresAsync(_leaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
}
