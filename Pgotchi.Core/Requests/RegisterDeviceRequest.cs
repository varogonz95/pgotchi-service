using Pgotchi.Shared.Models;

namespace Pgotchi.Shared.Requests;

public sealed class RegisterDeviceRequest
{
    public required string DeviceId { get; set; }
    public IDictionary<string, DevicePropertyValue> Properties { get; set; } = new Dictionary<string, DevicePropertyValue>();
}
