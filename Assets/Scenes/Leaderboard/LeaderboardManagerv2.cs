using System;
using TMPro;
using Unity.Services.Leaderboards;
using UnityEngine;

public class LeaderboardManagerv2 : MonoBehaviour
{
    public static LeaderboardManagerv2 Instance;
    
    public string LeaderboardId = "NoNameGame";
    
    [SerializeField] private TMP_Text playerName, playerScore, playerRank;
    [SerializeField] private TMP_Text listNames, listScores;
    [SerializeField] private TMP_InputField changeScore;
    

	
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
	
	public async void GetScores()
    {
        try
        {
            var scoresPlayer =
            await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            
            var scoresResponses = 
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions{ Offset = 0, Limit = 20 });
            
            string name;
            
            name = scoresPlayer.PlayerName;
            
            if (name[..^5] == "Player") 
            {
                name = scoresPlayer.PlayerName;
            }
            
            else 
            {
                name = scoresPlayer.PlayerName[..^5];
            }
            
            playerName.text = name;
            playerScore.text = scoresPlayer.Score.ToString("N0");
            playerRank.text = scoresPlayer.Rank + 1 + ". ";
            
            int loopCount = Mathf.Min(20, scoresResponses.Results.Count);
            
            listNames.text = "";
            listScores.text = "";
            
            for (int i = 0; i < loopCount; i++)
            {
                if (scoresResponses.Results[i].PlayerName[..^5] == "Player") 
                {
                    name = scoresResponses.Results[i].PlayerName;
                }
                
                else 
                {
                    name = scoresResponses.Results[i].PlayerName[..^5];
                }
                
                listNames.text += name + "\n";
                listScores.text += scoresResponses.Results[i].Score.ToString("N0") + "\n";
            }
        }

        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    
    public void ChangeScore() 
    {
        int.TryParse(changeScore.text, out int result); 
        SetPlayerScore(result);
    }
}