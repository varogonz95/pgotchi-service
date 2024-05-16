using Microsoft.Azure.Devices;
using System.Net.Http.Json;

namespace Pgotchi.Functions.Models;

public class DeviceSummary
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
