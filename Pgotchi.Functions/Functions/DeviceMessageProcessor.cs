using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Pgotchi.Functions.Functions
{
    public class DeviceMessageProcessor
    {
        private readonly ILogger<DeviceMessageProcessor> _logger;

        public DeviceMessageProcessor(ILogger<DeviceMessageProcessor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(DeviceMessageProcessor))]
        public void Run(
            [EventHubTrigger("messages/events", Connection = "AzureIotHub:ConnectionString", IsBatched = false, ConsumerGroup = "$Default")] EventData evt)
        {

            byte[] utf8Json = evt.Body.ToArray();
            var obj = JsonSerializer.Deserialize<IDictionary<string, object>>(utf8Json);

            _logger.LogInformation("Event Body: {body}", evt.Body);
            _logger.LogInformation("Event Content-Type: {contentType}", evt.ContentType);
        }
    }
}
