using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Pgotchi.Functions;

public sealed class SasTokenRequest
{
    public string Uri { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? PolicyName { get; set; }
}

public sealed class SasTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public int Expiry { get; set; }

    public SasTokenResponse() { }

    public SasTokenResponse(string token, int expiry)
    {
        Token = token;
        Expiry = expiry;
    }
}

public class GenerateSasToken(ILogger<GenerateSasToken> logger)
{
    const int ExpiryInSeconds = 3600;

    [Function(nameof(GenerateSasToken))]
    public HttpResponseMessage Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        [FromBody]
        SasTokenRequest request)
    {
        var fromEpochStart = DateTime.UtcNow - new DateTime(1970, 1, 1);
        var expiry = (int)fromEpochStart.TotalSeconds + ExpiryInSeconds;
        var encodedUri = WebUtility.UrlEncode(request.Uri);
        var stringToSign = encodedUri + "\n" + expiry;

        var hmac = new HMACSHA256(Convert.FromBase64String(request.Key));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
        var token = string.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}", encodedUri, WebUtility.UrlEncode(signature), expiry);

        if (!string.IsNullOrEmpty(request.PolicyName))
        {
            token += "&skn=" + request.PolicyName;
        }

        return new HttpResponseMessage {
            Content = JsonContent.Create<SasTokenResponse>(new(token, expiry)),
            StatusCode = HttpStatusCode.OK,
        };
    }
}
