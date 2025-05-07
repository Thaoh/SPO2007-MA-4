using TMPro;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text scoreText, highScoreText, worldRecordScoreText;

    public async void EndGame(double currentScore)
    {
        canvas.enabled = true;
        string leaderboardID = "NoNameGame";
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, 0);
        var playerScore = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardID);
        double personalBest = playerScore.Score;
        scoreText.text = currentScore.ToString();
        if (personalBest < currentScore)
        {
            personalBest = currentScore;
        }

        highScoreText.text = personalBest.ToString();
        var worldRecordEntry = await LeaderboardsService.Instance.GetScoresAsync(leaderboardID);
        double worldRecord = worldRecordEntry.Results[0].Score;
    if (worldRecord < currentScore)
        {
            worldRecord = currentScore;
        }
        worldRecordScoreText.text = worldRecord.ToString();
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardID, currentScore);
    }



    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
