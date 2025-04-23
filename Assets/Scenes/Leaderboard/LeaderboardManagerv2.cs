using System;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManagerv2 : MonoBehaviour
{
    public static LeaderboardManagerv2 Instance;
    
    public string LeaderboardId = "NoNameGame";
    
    [Header("Leaderboard Attributes")]
    [SerializeField] private TMP_Text playerName, playerScore, playerRank;
    [SerializeField] private TMP_Text listNames, listScores;
    
    [Header("Change Score Attributes")]
    [SerializeField] private TMP_InputField changeScore;
    
    [Header("HighScore Attributes")]
    [SerializeField] private TMP_Text highScoreName, highScoreScore;
    [SerializeField] private TMP_Text highScoreplayerName, highScoreplayerScore, highScoreplayerRank;
	
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}
	
	//Call this when the round ends, will update score to better one if better score.
	public async void SetPlayerScore(int score) 
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            GetScores();
        }
        
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
	
	//Call this when viewing leaderboard.
	public async void GetScores()
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, 0);
            
            var scoresPlayer =
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            
            var scoresResponses = 
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions{ Offset = 0, Limit = 20 });
            
            playerName.text = scoresPlayer.PlayerName;
            playerScore.text = scoresPlayer.Score.ToString("N0");
            playerRank.text = scoresPlayer.Rank + 1 + ". ";
            
            int loopCount = Mathf.Min(20, scoresResponses.Results.Count);
            
            listNames.text = "";
            listScores.text = "";
            
            for (int i = 0; i < loopCount; i++)
            {
                listNames.text += scoresResponses.Results[i].PlayerName + "\n";
                listScores.text += scoresResponses.Results[i].Score.ToString("N0") + "\n";
            }
        }

        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    //For changing score from an inputfield
    public void ChangeScore() 
    {
        int.TryParse(changeScore.text, out int result); 
        SetPlayerScore(result);
    }
    
    //Call this when entering a round to display best rank and your own best rank.
    public async void GetHighScore() 
    {
        var scoresPlayer =
        await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        
        var scoresResponses = 
        await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions{ Offset = 0, Limit = 1 });
        
        highScoreplayerName.text = scoresPlayer.PlayerName;
        highScoreplayerScore.text = scoresPlayer.Score.ToString("N0");
        highScoreplayerRank.text = scoresPlayer.Rank + 1 + ". ";
        
        highScoreName.text = scoresResponses.Results[0].PlayerName;
        highScoreScore.text = scoresResponses.Results[0].Score.ToString("N0");
    }
    
    public async void GetLeaderboard() 
    {
        var scoresResponses = 
        await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions{ Offset = 0, Limit = 20 });
        
        DataStore.Leaderboard = scoresResponses;
    }
}