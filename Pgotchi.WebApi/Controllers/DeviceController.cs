using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using Pgotchi.WebApi.Models;

namespace Pgotchi.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController(ILogger<DeviceController> logger) : ControllerBase
{
    private const string AzureIotHubConnectionString = "HostName=pgotchi-dev-east-iothub.azure-devices.net;SharedAccessKeyName=registryRead;SharedAccessKey=OEPQ3Mby0lmdkoeYtIuq5IAud+nmqVrbDAIoTD0wZI4=";

    [HttpGet()]
    public async Task<IActionResult> ListDevices([FromQuery] int? pageSize = 100)
    {
        using var registryManager = RegistryManager.CreateFromConnectionString(AzureIotHubConnectionString);
        var devicesQuery = registryManager.CreateQuery("select * from Devices", pageSize);
        var jsons = await devicesQuery.GetNextAsJsonAsync();
        var devices = jsons.Select(json => JsonConvert.DeserializeObject<DeviceSummary>(json));
        return Ok(devices);
    }


    [HttpGet("{deviceId}")]
    public async Task<IActionResult> FindById([FromRoute] string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));

        using var registryManager = RegistryManager.CreateFromConnectionString(AzureIotHubConnectionString);
        var devicesQuery = registryManager.CreateQuery($"select * from Devices where deviceId = '{deviceId}'", 1);
        var jsons = await devicesQuery.GetNextAsJsonAsync();
        var device = jsons.SingleOrDefault();

        return Ok(device);
    }
}
