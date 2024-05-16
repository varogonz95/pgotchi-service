using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pgotchi.Functions.Functions;

public class MessageReceived(ILogger<MessageReceived> logger, IHostEnvironment environment)
{
    [Function(nameof(MessageReceived))]
    [SignalROutput(HubName = nameof(DeviceTelemetryHub), ConnectionStringSetting = "AzureSignalR:ConnectionString")]
    public SignalRMessageAction Run([EventHubTrigger("%AzureIotHubEventHub:Name%", Connection = "AzureIotHubEventHub:ConnectionString", IsBatched = false)] EventData evt)
    {
        logger.LogInformation("Event Body: {body}", evt.Body);
        logger.LogInformation("Event Content-Type: {contentType}", evt.ContentType);

        var payload = new { };

        if (environment.IsDevelopment())
        {
        }

        var action = new SignalRMessageAction("message", [payload]);

        return action;
    }
}
