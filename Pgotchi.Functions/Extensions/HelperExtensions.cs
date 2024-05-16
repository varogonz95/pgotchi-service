using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Pgotchi.Functions.Models;

namespace Pgotchi.Functions.Extensions;

internal static class HelperExtensions
{
    public static DeviceSummary ToSummary(this Device device) => new()
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

    public static DeviceWithTwinSummary ToSummaryWithTwin(this Device device, Twin twin) => new(twin)
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

    public static async Task<DeviceWithTwinSummary?> GetDeviceWithTwinAsync(this RegistryManager registryManager, string deviceId, CancellationToken cancellationToken = default)
    {
        var device = await registryManager.GetDeviceAsync(deviceId, cancellationToken);

        if (device is null)
            return null;

        var twin = await registryManager.GetTwinAsync(deviceId, cancellationToken);

        return device.ToSummaryWithTwin(twin);
    }
}
