using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgotchi.Functions.Extensions;
using Pgotchi.Functions.Json;
using Pgotchi.Functions.Models;
using System.Security.Cryptography;
using System.Text.Json;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Pgotchi.Functions.Functions;

[Serializable]
public sealed class RegisterDeviceRequest(string deviceId, IDictionary<string, DevicePropertyValue> properties)
{
    public string DeviceId { get; } = deviceId;
    public IDictionary<string, DevicePropertyValue> Properties { get; } = properties ?? new Dictionary<string, DevicePropertyValue>();
}

public class RegisterDevice(ILogger<RegisterDevice> logger, IOptions<AzureIotHubOptions> azureIotHubOptions)
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
            var device = await GetOrCreateDevice(registryManager, request, primaryKey, secondaryKey, cancellationToken);
            var summary = device.ToSummary();

            return new DynamicJsonPropertyNamingResult(summary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not add {device} with {deviceId} of '{requestDeviceId}'", nameof(Device), nameof(Device.Id), request.DeviceId);
            throw;
        }
    }

    private static async Task<Device> GetOrCreateDevice(RegistryManager registryManager, RegisterDeviceRequest request, string primaryKey, string secondaryKey, CancellationToken cancellationToken = default)
    {
        var deviceId = request.DeviceId;
        var device = await registryManager.GetDeviceAsync(deviceId, cancellationToken);

        if (device == null)
        {
            device = new Device(deviceId)
            {
                Authentication = new()
                {
                    Type = AuthenticationType.Sas,
                    SymmetricKey = new()
                    {
                        PrimaryKey = primaryKey,
                        SecondaryKey = secondaryKey,
                    },
                },
            };

            if (request.Properties.Count > 0)
            {
                var twinProperties = new TwinCollection();

                //var mem = new MemoryStream();
                //var reader = new StreamReader(mem);

                foreach (var item in request.Properties)
                {
                    //await JsonSerializer.SerializeAsync(mem, item.Value, cancellationToken: cancellationToken);
                    //twinProperties[item.Key] = await reader.ReadToEndAsync(cancellationToken);
                    twinProperties[item.Key] = item.Value;
                }

                var twin = new Twin(deviceId);
                twin.Properties.Desired = twinProperties;

                var operation = await registryManager.AddDeviceWithTwinAsync(device, twin, cancellationToken);

                if (operation.IsSuccessful)
                {
                    return device;
                }

                throw new Exception("An error occured registering a device with a twin");
            }

            return await registryManager.AddDeviceAsync(device, cancellationToken);
        }

        return device;
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
