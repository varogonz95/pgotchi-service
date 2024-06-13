using Microsoft.Azure.Devices.Shared;

namespace Pgotchi.Shared.Models;

public class DeviceTwinSummary : DeviceSummary
{
    public TwinCollection? Properties { get; set; } = new();
}
