using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Pgotchi.Functions.Funcs;

public class Function
{
    private readonly ILogger _logger;

    public Function(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Function>();
    }

    [Function("message")]
    [WebPubSubOutput(Hub = "TestHub")]
    public SendToAllAction Run(
    [WebPubSubTrigger("TestHub", WebPubSubEventType.User, "message")] UserEventRequest request)
    {
        return new SendToAllAction
        {
            Data = BinaryData.FromString($"[{request.ConnectionContext.UserId}] {request.Data}"),
            DataType = request.DataType
        };
    }
}
