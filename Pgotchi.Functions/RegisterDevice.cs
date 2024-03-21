using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Pgotchi.Functions;

public sealed class RegisterDeviceRequest
{
    public required string DeviceId { get; set; }
}

public class RegisterDevice(ILogger<RegisterDevice> logger, IConfiguration configuration)
{
    
    [Function("RegisterDevice")]
    public async Task<IActionResult> Run(
#if DEBUG
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
#else
        [HttpTrigger(AuthorizationLevel.Function, "post")]
#endif 
        [FromBody]
        RegisterDeviceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var connectionString = configuration.GetConnectionString("AzureIoTHub") ?? throw new Exception("Could not get AzureIoTHub ConnectionString");

        using var registryManager = RegistryManager.CreateFromConnectionString(connectionString);

        var deviceToAdd = new Device(request.DeviceId)
        {
            Authentication = new()
            {
                Type = AuthenticationType.Sas,
                SymmetricKey = new()
                {
                    PrimaryKey = GenerateKey(32),
                    SecondaryKey = GenerateKey(32),
                },
            }
        };

        try
        {
            await registryManager.AddDeviceAsync(deviceToAdd);
        }
        catch(DeviceAlreadyExistsException dae)
        {
            logger.LogInformation(dae, "A {device} with an {deviceId} of '{requestDeviceId}' already exists", nameof(Device), nameof(Device.Id), request.DeviceId);
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not add {device} with {deviceId} of '{requestDeviceId}'", nameof(Device), nameof(Device.Id), request.DeviceId);
            throw;
        }

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    private static string GenerateDeviceId()
    {
        return Guid.NewGuid().ToString();
    }

    private static string GenerateKey(int keySize)
    {
        byte[] keyBytes = new byte[keySize];
        using var cyptoProvider = RandomNumberGenerator.Create();
        while (keyBytes.Contains(byte.MinValue))
        {
            cyptoProvider.GetBytes(keyBytes);
        }

        return Convert.ToBase64String(keyBytes);
    }
}
