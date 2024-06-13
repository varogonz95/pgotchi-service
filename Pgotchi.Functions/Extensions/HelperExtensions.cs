using AutoMapper;
using Microsoft.Azure.Devices;
using Pgotchi.Shared.Models;
using System.Text.Json;

namespace Pgotchi.Functions.Extensions;

internal static class HelperExtensions
{
    public static async Task<DeviceTwinSummary?> GetDeviceWithTwinAsync(this RegistryManager registryManager, string deviceId, IMapper mapper, CancellationToken cancellationToken = default)
    {
        var device = await registryManager.GetDeviceAsync(deviceId, cancellationToken);

        if (device is null)
            return null;

        var twin = await registryManager.GetTwinAsync(deviceId, cancellationToken);

        return mapper.Map<DeviceTwinSummary>(twin);
    }


    public static string ToJson(this DevicePropertyValue devicePropertyValue) => JsonSerializer.Serialize(devicePropertyValue);
}
