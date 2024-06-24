using Microsoft.Azure.Devices;

namespace Pgotchi.Shared.Models;

public class DeviceSummary
{
    public required string Id { get; set; }
    public required string UserId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public DeviceConnectionState ConnectionState { get; set; }
    public DeviceStatus Status { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
    public string? SymmetricPrimaryKey { get; set; }
    public string? SymmetricSecondaryKey { get; set; }
    public string? X509PrimaryThumbprint { get; set; }
    public string? X509SecondaryThumbprint { get; set; }
    public DateTime LastActivityTime {  get; set; }
}
