using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Pgotchi.Shared.Requests;

namespace Pgotchi.Shared.Extensions;

public static class RegistryManagerExtensions
{
    public static async Task<Twin> GetOrCreateTwinDevice(this RegistryManager registryManager, RegisterDeviceRequest request, SymmetricKey symmetricKey, CancellationToken cancellationToken = default)
    {
        var success = true;
        var deviceId = request.DeviceId;
        var device = await registryManager.GetDeviceAsync(deviceId, cancellationToken);

        Twin twin;

        if (device is null)
        {
            device = CreateDevice(deviceId, symmetricKey);

            var tags = new TwinCollection();
            tags[TwinTags.UserId] = Guid.NewGuid().ToString();
#if DEBUG
            tags[TwinTags.DeviceType] = "simulated";
#endif

            if (request.Properties is not null)
                foreach (var item in request.Properties)
                    tags[item.Key] = item.Value;

            twin = new Twin(deviceId) { Tags = tags };
            var operation = await registryManager.AddDeviceWithTwinAsync(device, twin, cancellationToken);
            success = operation.IsSuccessful;
        }
        else
        {
            twin = await registryManager.GetTwinAsync(deviceId, cancellationToken);
        }

        if (!success || twin is null)
        {
            throw new Exception("An error occured registering a device with a twin");
        }

        return twin;
    }

    private static Device CreateDevice(string deviceId, SymmetricKey symmetricKey) =>
        new(deviceId)
        {
            Authentication = new()
            {
                Type = AuthenticationType.Sas,
                SymmetricKey = new()
                {
                    PrimaryKey = symmetricKey.PrimaryKey,
                    SecondaryKey = symmetricKey.SecondaryKey,
                },
            },
        };
}
