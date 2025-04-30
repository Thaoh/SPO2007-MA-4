using TMPro;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text scoreText, highScoreText, worldRecordScoreText;

    public void EndGame(double currentScore)
    {
        canvas.enabled = true;
        string leaderboardID = "NoNameGame";
        double personalBest = LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardID).Result.Score;
        scoreText.text = currentScore.ToString();
        if (personalBest < currentScore)
        {
            personalBest = currentScore;
        }
        highScoreText.text = personalBest.ToString();
        double worldRecord = LeaderboardsService.Instance.GetScoresAsync(leaderboardID).Result.Results[0].Score;
        if (worldRecord < currentScore)
        {
            worldRecord = currentScore;
        }
        worldRecordScoreText.text = worldRecord.ToString();
        LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, currentScore);
    }



    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
