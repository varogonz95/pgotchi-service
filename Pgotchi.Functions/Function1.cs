using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Pgotchi.Functions
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData Run(
            [WebPubSubTrigger(WebPubSubEventType.User, "deviceConfigChanged")] WebPubSubEventRequest request,
            [WebPubSubConnectionInput])
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //var response = request.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //response.WriteString("Welcome to Azure Functions!");

            return null;
        }
    }
}
