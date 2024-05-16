using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;

namespace Pgotchi.Functions.Functions
{
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
            await response.WriteBytesAsync(negotiateResponse.ToArray());
            return response;
        }

        //public Task Broadcast([SignalRTrigger("messages", "broadcast", "message")] SignalRInvocationContext invocationContext, CancellationToken cancellationToken)
        //{
        //    return Clients.All.SendAsync("newMessage", new NewMessage(invocationContext, "pong"), cancellationToken: cancellationToken);
        //}
    }

    public class NewMessage(SignalRInvocationContext invocationContext, string message)
    {
        public string ConnectionId { get; } = invocationContext.ConnectionId;
        public string Sender { get; } = string.IsNullOrEmpty(invocationContext.UserId) ? string.Empty : invocationContext.UserId;
        public string Text { get; } = message;
    }
}
