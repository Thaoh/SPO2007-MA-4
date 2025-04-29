using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScoreBoardUi : MonoBehaviour
{
    [Header("Data")] [SerializeField] private string MainMenuScene = "MainMenu";
    [SerializeField] private string LeaderBoardID = "NoNameGame";

    [Header("Ui Elements")]
    [SerializeField] private TMP_Text scoreBoard;
    [SerializeField] private Button _toMainMenuButton;

    private void Start()
    {
        _toMainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(MainMenuScene);
        });
    }

    void OnEnable()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            SceneManager.LoadScene(MainMenuScene);
            return;
        }
        ShowLeaderboardAsync();
        RefreshUi();
    }

    private async Task ShowLeaderboardAsync()
    {
        scoreBoard.text = "Loading leaderboard...";

        var response = await LeaderboardsService.Instance.GetPlayerRangeAsync(LeaderBoardID, new GetPlayerRangeOptions()
        {
            RangeLimit = 10,
            IncludeMetadata = true
        });

        string display = "Leaderboard:\n";
        foreach (var entry in response.Results)
        {
            string youTag = entry.PlayerId == AuthenticationService.Instance.PlayerId ? " <font-weight=\"900\">(You)<font-weight=\"400\">" : "";
            display += $"<font-weight=\"800\"><size=70%>{entry.Rank+1}<size=70%>. <color=\"yellow\">{entry.Score}P<font-weight=\"400\"><size=70%> <color=\"white\">{entry.PlayerName ?? ("ID" + entry.PlayerId) ?? "Anonymous"} {youTag}\n";
        }

        scoreBoard.text = display;
        scoreBoard.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scoreBoard.rectTransform);
    }

    void RefreshUi()
    {
    }
}