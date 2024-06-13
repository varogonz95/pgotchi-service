using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgotchi.Shared.Models;
using Pgotchi.Shared.Options;
using Pgotchi.Shared.Requests;
using System.Security.Cryptography;

namespace Pgotchi.Shared.Services;

public interface IDeviceService
{
    Task<DeviceSummary> RegisterDeviceAsync(RegisterDeviceRequest request, CancellationToken cancellation);
}

public class DeviceService(ILogger<DeviceService> logger, IMapper mapper, IOptions<AzureIotHubOptions> options) : IDeviceService
{
    public async Task<DeviceSummary> RegisterDeviceAsync(RegisterDeviceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            using var registryManager = RegistryManager.CreateFromConnectionString(options.Value.ConnectionString);
            var primaryKey = GenerateKey(32);
            var secondaryKey = GenerateKey(32);
            var device = await GetOrCreateDevice(registryManager, request, primaryKey, secondaryKey, cancellationToken);
            var summary = mapper.Map<DeviceSummary>(device);

            return summary;
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

        if (device is null)
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

            var twinCollection = new TwinCollection();
            foreach (var item in request.Properties)
            {
                twinCollection[item.Key] = item.Value;
            }

            var twinProperties = new TwinProperties
            {
                Desired = twinCollection
            };
            var twin = new Twin(deviceId)
            {
                Properties = twinProperties
            };

            var operation = await registryManager.AddDeviceWithTwinAsync(device, twin, cancellationToken);

            if (!operation.IsSuccessful)
            {
                throw new Exception("An error occured registering a device with a twin");
            }
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
