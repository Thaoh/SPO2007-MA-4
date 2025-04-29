using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;
using Unity.Services.CloudCode.Core;
using Unity.Services.CloudCode.Shared;
using System.Threading.Tasks;
using System;

namespace WishHappyNewYear;

public class HappyNewYear
{
    private readonly ILogger<HappyNewYear> _logger;

    public HappyNewYear(ILogger<HappyNewYear> logger)
    {
        _logger = logger;
    }

    [CloudCodeFunction("SendProjectMessage")]
    public async Task SendProjectMessage(IExecutionContext context, PushClient pushClient, string message, string messageType)
    {
        try
        {
            await pushClient.SendProjectMessageAsync(context, message, messageType);
        }
        catch (ApiException e)
        {
            _logger.LogError("Failed to send project message. Error: {Error}", e.Message);
            throw new Exception($"Failed to send project message. Error: {e.Message}");
        }
    }

}

public class ModuleConfig : ICloudCodeSetup
{
    public void Setup(ICloudCodeConfig config)
    {
        config.Dependencies.AddSingleton(PushClient.Create());

    }
}


