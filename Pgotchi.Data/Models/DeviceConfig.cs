namespace Pgotchi.Data.Models;

public class DeviceConfig
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public IDictionary<string, PeripheralConfig> Peripherals { get; set; } = new Dictionary<string, PeripheralConfig>();
    public IDictionary<string, RoutineConfig>? Routines { get; set; }
}
