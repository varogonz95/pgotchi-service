using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Pgotchi.Functions.Functions;

public sealed class SasTokenRequest
{
    public string Uri { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? PolicyName { get; set; }

    public Task<string> ToStringAsync() => JsonContent.Create(this).ReadAsStringAsync();
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

    public Task<string> ToStringAsync() => JsonContent.Create(this).ReadAsStringAsync();
}

public class GenerateSasToken(ILogger<GenerateSasToken> logger)
{
    const int ExpiryInSeconds = 3600;

    [Function(nameof(GenerateSasToken))]
    public async Task<IActionResult> Run(
#if DEBUG
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
#else
        [HttpTrigger(AuthorizationLevel.Function, "post")]
#endif 
        [FromBody]
        SasTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        logger.LogDebug("Request: {request}", await request.ToStringAsync());

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

        var content = new SasTokenResponse(token, expiry);
        logger.LogDebug("Response content: {response}", await content.ToStringAsync());

        return new ContentResult
        {
            StatusCode = StatusCodes.Status200OK,
            Content = await content.ToStringAsync(),
            ContentType = "application/json; charset=utf-8"
        };
    }
}
