namespace Pgotchi.Data.Requests;

public sealed class GetConfigRequest
{
    public string DeviceIdentifier { get; set; } = null!;
    public string UserKey { get; set; } = null!;
}
