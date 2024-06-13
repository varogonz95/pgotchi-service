using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Pgotchi.Shared.Models;
using Pgotchi.Shared.Services;

namespace Pgotchi.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController(ILogger<DeviceController> logger, IMapper mapper, IDeviceService deviceService) : ControllerBase
{
    private const string AzureIotHubConnectionString = "HostName=pgotchi-dev-east-iothub.azure-devices.net;SharedAccessKeyName=registryRead;SharedAccessKey=OEPQ3Mby0lmdkoeYtIuq5IAud+nmqVrbDAIoTD0wZI4=";

    [HttpGet()]
    public async Task<IActionResult> ListDevices([FromQuery] int? pageSize = 100)
    {
        using var registryManager = RegistryManager.CreateFromConnectionString(AzureIotHubConnectionString);

        var devicesQuery = registryManager.CreateQuery("select * from Devices", pageSize);
        var twins = await devicesQuery.GetNextAsTwinAsync();
        var devices = twins.Select(mapper.Map<DeviceTwinSummary>);

        return Ok(devices);
    }


    [HttpGet("{deviceId}")]
    public async Task<IActionResult> FindById([FromRoute] string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));

        using var registryManager = RegistryManager.CreateFromConnectionString(AzureIotHubConnectionString);

        var devicesQuery = registryManager.CreateQuery($"select * from Devices where deviceId = '{deviceId}'", 1);
        var twins = await devicesQuery.GetNextAsTwinAsync();
        var summary = mapper.Map<DeviceTwinSummary>(twins.SingleOrDefault());

        return Ok(summary);
    }

    //[HttpPost]
    //public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
    //{
    //    ArgumentNullException.ThrowIfNull(request);

    //    DeviceSummary summary = await deviceService.RegisterDeviceAsync(request);
    //}
}
