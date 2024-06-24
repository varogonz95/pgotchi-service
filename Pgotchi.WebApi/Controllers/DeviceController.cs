using Microsoft.AspNetCore.Mvc;
using Pgotchi.Shared.Models;
using Pgotchi.Shared.Requests;
using Pgotchi.Shared.Services;

namespace Pgotchi.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DeviceController(ILogger<DeviceController> logger, IDeviceService deviceService) : ControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> ListDevices(CancellationToken cancellationToken, [FromQuery] int? pageSize = 100)
    {
        var devices = await deviceService.GetDeviceTwinSummariesAsync(pageSize, cancellationToken);

        return Ok(devices);
    }


    [HttpGet("{deviceId}")]
    public async Task<IActionResult> FindById([FromRoute] string deviceId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(deviceId))
            throw new ArgumentException($"'{nameof(deviceId)}' cannot be null or empty.", nameof(deviceId));

        var summary = await deviceService.GetDeviceTwinSummaryAsync(deviceId, cancellationToken);

        return Ok(summary);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validator = new RegisterDeviceRequestValidator();
        var validation = validator.Validate(request);

        if (!validation.IsValid)
        {
            var errorDetails = validation.Errors
                .GroupBy(err => err.PropertyName, err => err.ErrorMessage)
                .ToDictionary(err => err.Key, err => err.ToArray());

            var problemDetails = new ValidationProblemDetails(errorDetails);
            return ValidationProblem(problemDetails);
        }

        var summary = await deviceService.RegisterDeviceAsync(request, cancellationToken);

        return Ok(summary);
    }
}
