using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgotchi.Shared.Extensions;
using Pgotchi.Shared.Models;
using Pgotchi.Shared.Options;
using Pgotchi.Shared.Requests;
using System.Security.Cryptography;

namespace Pgotchi.Shared.Services;

public interface IDeviceService
{
    Task<IEnumerable<DeviceTwinSummary>> GetDeviceTwinSummariesAsync(int? pageSize, CancellationToken cancellationToken);
    Task<DeviceTwinSummary> GetDeviceTwinSummaryAsync(string deviceId, CancellationToken cancellationToken = default);
    Task<DeviceTwinSummary> RegisterDeviceAsync(RegisterDeviceRequest request, CancellationToken cancellation = default);
}

public class DeviceService(
    ILogger<DeviceService> logger, 
    IMapper mapper, 
    IOptions<AzureIotHubOptions> iotHubOptions) : IDeviceService
{
    public async Task<DeviceTwinSummary> RegisterDeviceAsync(RegisterDeviceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            using var registryManager = RegistryManager.CreateFromConnectionString(iotHubOptions.Value.ConnectionString);
            var primaryKey = GenerateKey(32);
            var secondaryKey = GenerateKey(32);
            var deviceTwin = await registryManager.GetOrCreateTwinDevice(request, new()
            {
                PrimaryKey = primaryKey,
                SecondaryKey = secondaryKey
            },
            cancellationToken);
            var summary = mapper.Map<DeviceTwinSummary>(deviceTwin);

            return summary;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not add {device} with {deviceId} of '{requestDeviceId}'", nameof(Device), nameof(Device.Id), request.DeviceId);
            throw;
        }
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

    public async Task<DeviceTwinSummary> GetDeviceTwinSummaryAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        using var registryManager = RegistryManager.CreateFromConnectionString(iotHubOptions.Value.ConnectionString);
        var devicesQuery = registryManager.CreateQuery($"select * from Devices where deviceId = '{deviceId}'", 1);
        var twins = await devicesQuery.GetNextAsTwinAsync();
        var summary = mapper.Map<DeviceTwinSummary>(twins.SingleOrDefault());
        return summary;
    }

    public async Task<IEnumerable<DeviceTwinSummary>> GetDeviceTwinSummariesAsync(int? pageSize, CancellationToken cancellationToken)
    {
        using var registryManager = RegistryManager.CreateFromConnectionString(iotHubOptions.Value.ConnectionString);

        var devicesQuery = registryManager.CreateQuery("select * from Devices", pageSize);
        var twins = await devicesQuery.GetNextAsTwinAsync();
        var summaries = twins.Select(mapper.Map<DeviceTwinSummary>);

        return summaries;
    }
}
