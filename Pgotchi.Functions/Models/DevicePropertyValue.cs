using System.Text.Json;

namespace Pgotchi.Functions.Models;

public class DevicePropertyValue
{
    public string? Type { get; set; }
    public string? Label { get; set; }

    public string ToJson() => JsonSerializer.Serialize(this);
}
