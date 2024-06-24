using Azure;
using Azure.Messaging.EventHubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;
using Pgotchi.Shared;

namespace Pgotchi.Functions.Functions
{
    public static class ConsumerGroups
    {
        public const string Default = "$Default";
        public const string WebClients = "webclients";
    }

    [SignalRConnection("AzureSignalR:ConnectionString")]
    public class DeviceTelemetryHub(ILogger<DeviceTelemetryHub> logger, IServiceProvider serviceProvider) : ServerlessHub(serviceProvider)
    {
        [Function(nameof(Negotiate))]
        public async Task<IActionResult> Negotiate(
            [HttpTrigger(
#if DEBUG
                AuthorizationLevel.Anonymous,
#else
                AuthorizationLevel.Function,
#endif
            "post")]
            HttpRequest request)
        {

            if (!request.Headers.TryGetValue("x-user-id", out var userId) || string.IsNullOrEmpty(userId))
            {
                return new BadRequestResult();
            }

            var negotiateResponse = await NegotiateAsync(new() { UserId = userId, });
            var json = (object)negotiateResponse.ToDynamicFromJson();
            return new JsonResult(json);
        }

        [Function(nameof(BroadcastToWebClients))]
        public async Task BroadcastToWebClients(
            [EventHubTrigger("%AzureIotHubEventHub:Name%", Connection = "AzureIotHubEventHub:ConnectionString", ConsumerGroup = ConsumerGroups.WebClients, IsBatched = false)]
            EventData eventData,
            CancellationToken cancellationToken)
        {
            if (!eventData.Properties.TryGetValue(nameof(TwinTags.UserId), out var userIdObj))
            {
                logger.LogWarning("Cannot broadcast device messages because the Device userId property is not set. Skipping message broadcast");
                return;
            }

            var userId = (string)userIdObj;
            var userExists = await ClientManager.UserExistsAsync(userId, cancellationToken);
            if (!userExists)
            {
                logger.LogError("User '{userId}' does not exist. Skipping message broadcast", userId);
                return;
            }
#if DEBUG
            // Simulate data
            var readings = new Dictionary<string, object>()
            {
                { "$timestamp", DateTime.UtcNow },
                { "soilMoisture", Random.Shared.Next(100) },
                { "lightLevel", Random.Shared.Next(100) },
            };
            await Clients.User(userId).SendAsync(EventMethods.NewMessage, readings, cancellationToken);
#else
            var eventBody = (object)eventData.EventBody.ToDynamicFromJson();
            await Clients.User(userId).SendAsync(EventMethods.NewMessage, eventBody, cancellationToken);
#endif
        }
    }

    public class NewMessage(SignalRInvocationContext context, string message)
    {
        public string ConnectionId { get; } = context.ConnectionId;
        public string Sender { get; } = string.IsNullOrEmpty(context.UserId) ? string.Empty : context.UserId;
        public string Text { get; } = message;
    }
}
