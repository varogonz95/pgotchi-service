using Microsoft.Azure.Devices;
using Pgotchi.Functions.Functions;

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
}
