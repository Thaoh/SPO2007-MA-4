using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using System.Threading.Tasks;
using System;

namespace LeaderboardsNotification;

public class LeaderboardsNotification
{
    private readonly ILogger<LeaderboardsNotification> _logger;

    public LeaderboardsNotification(ILogger<LeaderboardsNotification> logger)
    {
        _logger = logger;
    }

    [CloudCodeFunction("SendPlayerMessage")]
    public async Task SendPlayerMessage(IExecutionContext context, IGameApiClient gameApiClient, PushClient pushClient, string playerId, string playerName,
        string leaderboardId, double score)
    {
        try
        {
            var closestPlayers = await gameApiClient.Leaderboards.GetLeaderboardScoresPlayerRangeAsync(context,
                context.ServiceToken, new Guid(context.ProjectId), leaderboardId, playerId, 1);

            if (closestPlayers.Data.Results.Count != 0)
            {
                var player = closestPlayers.Data.Results[^1];
                string message =
                    $"The player {playerName} has just beaten your score of {player.Score} on the {leaderboardId} leaderboard by {score - player.Score} points!";

                await pushClient.SendPlayerMessageAsync(context, message, "Information", player.PlayerId);
            }
        }
        catch (ApiException e)
        {
            _logger.LogError("Failed to send push notification to player {playerId}. Error: {Error}", playerId, e.Message);
            throw new Exception($"Failed to send push notification to player {playerId}. Error: {e.Message}");
        }
    }
}


public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(PushClient.Create());
        config.Dependencies.AddSingleton(GameApiClient.Create());
    }
}