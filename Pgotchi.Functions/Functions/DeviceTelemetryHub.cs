using Azure;
using Azure.Messaging.EventHubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;

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
        public async Task<HttpResponseData> Negotiate(
            [HttpTrigger(
#if DEBUG
                AuthorizationLevel.Anonymous,
#else
                AuthorizationLevel.Function,
#endif
            "post")] HttpRequestData req)
        {
            var bodyStr = await req.ReadAsStringAsync();
            logger.LogDebug("Request body:\n{0}", bodyStr);

             var negotiateResponse = await NegotiateAsync(new() { UserId = "test-user-id", });
            
            var response = req.CreateResponse();
            response.Headers.Add("Content-Type", "application/json");
            
            await response.WriteBytesAsync(negotiateResponse.ToArray());
            
            return response;
        }

        [Function(nameof(Broadcast))]
        public Task Broadcast(
            [EventHubTrigger("%AzureIotHubEventHub:Name%", Connection = "AzureIotHubEventHub:ConnectionString", ConsumerGroup = ConsumerGroups.WebClients, IsBatched = false)]
            EventData eventData,
            CancellationToken cancellationToken)
        {
            var data = eventData.EventBody.ToObjectFromJson<IDictionary<string, object>>();

            return Clients.All.SendAsync("newMessage", data, cancellationToken: cancellationToken);
        }
    }

    public class NewMessage(SignalRInvocationContext context, string message)
    {
        public string ConnectionId { get; } = context.ConnectionId;
        public string Sender { get; } = string.IsNullOrEmpty(context.UserId) ? string.Empty : context.UserId;
        public string Text { get; } = message;
    }
}
