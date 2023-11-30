using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Pgotchi.Functions.Funcs;

public class Functions
{
    private readonly ILogger _logger;

    public Functions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Functions>();
    }

    [Function("UserMessageFunction")]
    [WebPubSubOutput(Hub = "simplechat")]
    public SendToAllAction UserMessageFunction(
        [WebPubSubTrigger("simplechat", WebPubSubEventType.User, "message")] UserEventRequest request)
    {
        return new SendToAllAction
        {
            Data = BinaryData.FromString($"[{request.ConnectionContext.UserId}] aaaaaaaaaaa"),
            DataType = request.DataType
        };
    }

    [Function(nameof(WebPubSubConnectionInputBindingFunction))]
    public HttpResponseData WebPubSubConnectionInputBindingFunction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [WebPubSubConnectionInput(Hub = "LocalTestHub")] WebPubSubConnection connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteAsJsonAsync(connectionInfo);
        return response;
    }
}
