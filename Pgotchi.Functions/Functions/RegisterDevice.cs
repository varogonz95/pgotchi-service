using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgotchi.Functions.Extensions;
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

public class RegisterDevice(ILogger<RegisterDevice> logger, IOptions<AzureIotHubOptions> azureIotHubOptions, IOptions<JsonSerializerOptions> jsonSerializerOptions)
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
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            using var registryManager = RegistryManager.CreateFromConnectionString(azureIotHubOptions.Value.ConnectionString);
            var primaryKey = GenerateKey(32);
            var secondaryKey = GenerateKey(32);
            var device = await GetOrCreateDevice(registryManager, request.DeviceId, primaryKey, secondaryKey, cancellationToken);
            var summary = device.ToSummary();

            return new JsonResult(summary, jsonSerializerOptions.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not add {device} with {deviceId} of '{requestDeviceId}'", nameof(Device), nameof(Device.Id), request.DeviceId);
            throw;
        }
    }

    private static async Task<Device> GetOrCreateDevice(RegistryManager registryManager, string deviceId, string primaryKey, string secondaryKey, CancellationToken cancellationToken = default) =>
        await registryManager.GetDeviceAsync(deviceId, cancellationToken) ??
        await registryManager.AddDeviceAsync(
            new Device(deviceId)
            {
                Authentication = new()
                {
                    Type = AuthenticationType.Sas,
                    SymmetricKey = new()
                    {
                        PrimaryKey = primaryKey,
                        SecondaryKey = secondaryKey,
                    },
                }
            }, cancellationToken);

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
