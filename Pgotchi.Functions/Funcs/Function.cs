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

    [Function("Function")]
    public UserEventResponse Run(
        [WebPubSubTrigger("TestHub", WebPubSubEventType.User, "message")] UserEventRequest request)
    {
        _logger.LogDebug("UserEventRequest: {request}", request);
        return new("Hello world!");
    }
}
