using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pgotchi.WebApi.Models;

internal class DeviceSummary
{
    public string DeviceId { get; set; } = string.Empty;
    public string ETag { get; set; } = string.Empty;
    public string DeviceETag { get; set; } = string.Empty;
    [JsonConverter(typeof(StringEnumConverter))]
    public DeviceStatus Status { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public DeviceConnectionState ConnectionState { get; set; }
    public DateTime LastActivityTime { get; set; }
}
