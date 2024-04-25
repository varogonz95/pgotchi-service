using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Pgotchi.Functions.Functions;

public sealed class RegisterDeviceRequest
{
    public required string DeviceId { get; set; }
}

public sealed class DeviceSummary
{
    public required string Id { get; set; }
    public DeviceConnectionState ConnectionState { get; set; }
    public DeviceStatus Status { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
    public string? SymmetricPrimaryKey { get; set; }
    public string? SymmetricSecondaryKey { get; set; }
    public string? X509PrimaryThumbprint { get; set; }
    public string? X509SecondaryThumbprint { get; set; }

    public Task<string> ToStringAsync() => JsonContent.Create(this).ReadAsStringAsync();
}

public class RegisterDevice(ILogger<RegisterDevice> logger, IOptions<AzureIotHubOptions> options)
{

    [Function("RegisterDevice")]
    public async Task<IActionResult> Run(
#if DEBUG
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
#else
        [HttpTrigger(AuthorizationLevel.Function, "post")]
#endif 
        [FromBody]
        RegisterDeviceRequest request,
        [FromQuery]
        string namingStrategy,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var configOptions = options.Value;
        using var registryManager = RegistryManager.CreateFromConnectionString(configOptions.ConnectionString);

        Device device;

        try
        {
            device =
                await registryManager.GetDeviceAsync(request.DeviceId, cancellationToken) ??
                await registryManager.AddDeviceAsync(
                    new Device(request.DeviceId)
                    {
                        Authentication = new()
                        {
                            Type = AuthenticationType.Sas,
                            SymmetricKey = new()
                            {
                                PrimaryKey = GenerateKey(32),
                                SecondaryKey = GenerateKey(32),
                            },
                        }
                    }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not add {device} with {deviceId} of '{requestDeviceId}'", nameof(Device), nameof(Device.Id), request.DeviceId);
            throw;
        }

        var summary = new DeviceSummary
        {
            Id = device.Id,
            ConnectionState = device.ConnectionState,
            Status = device.Status,
            AuthenticationType = device.Authentication.Type,
            SymmetricPrimaryKey = device.Authentication.SymmetricKey?.PrimaryKey,
            SymmetricSecondaryKey = device.Authentication.SymmetricKey?.SecondaryKey,
            X509PrimaryThumbprint = device.Authentication.X509Thumbprint?.PrimaryThumbprint,
            X509SecondaryThumbprint = device.Authentication.X509Thumbprint?.SecondaryThumbprint,
        };

        var jsonSettings = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            PropertyNamingPolicy = ResolveJsonNamingPolicy(namingStrategy),
        };

        return new JsonResult(summary, jsonSettings);
    }

    private static JsonNamingPolicy ResolveJsonNamingPolicy(string namingStrategy)
    {
        return string.IsNullOrWhiteSpace(namingStrategy) ? JsonNamingPolicy.CamelCase : namingStrategy switch
        {
            "snakeCaseUpper" => JsonNamingPolicy.SnakeCaseUpper,
            "snakeCaseLower" => JsonNamingPolicy.SnakeCaseLower,
            "kebabCaseUpper" => JsonNamingPolicy.KebabCaseUpper,
            "kebabCaseLower" => JsonNamingPolicy.KebabCaseLower,
            "capitalCase" => new JsonCapitalCaseNamingPolicy(),
            _ => JsonNamingPolicy.CamelCase,
        };
    }

    private static string GenerateKey(int keySize)
    {
        byte[] keyBytes = new byte[keySize];
        using var cyptoProvider = RandomNumberGenerator.Create();
        while (keyBytes.Contains(byte.MinValue))
        {
            cyptoProvider.GetBytes(keyBytes);
        }

        return Convert.ToBase64String(keyBytes);
    }
}
