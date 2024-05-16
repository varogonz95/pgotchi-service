using Microsoft.Azure.Devices.Shared;

namespace Pgotchi.Functions.Models;

public class DeviceWithTwinSummary(Twin? twin) : DeviceSummary
{
    public TwinProperties? TwinProperties { get; } = twin?.Properties;
}
