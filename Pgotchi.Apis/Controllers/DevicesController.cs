using Microsoft.AspNetCore.Mvc;
using Pgotchi.Data.Models;
using Pgotchi.Data.Requests;

namespace Pgotchi.Apis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(string deviceIdentifier)
    {
        throw new NotImplementedException();
    }

    [HttpPost("config")]
    public async Task<IActionResult> SetConfig([FromQuery] GetConfigRequest request)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Gets corresponding DeviceConfig for Device identified with {deviceId}
    /// </summary>
    /// <param name="deviceId">Device Identifier</param>
    /// <returns>DeviceConfig</returns>
    [HttpGet("config/{deviceId}")]
    public async Task<IActionResult> GetConfig([FromRoute] string deviceId)
    {
        var config = new DeviceConfig()
        {
            Name = "Test Device",
            Description = "Test Device Description",
            Peripherals = new Dictionary<string, PeripheralConfig>
            {
                {
                    "lightLevel",
                    new PeripheralConfig
                    {
                        Name = "Light",
                        Type = PeripheralType.LightLevel,
                        Display = true,
                    }
                },
            },
            Routines = new Dictionary<string, RoutineConfig>
            {
                {
                    "lightLevel",
                    new RoutineConfig()
                    {
                        Conditions = new Dictionary<ConditionType, ICollection<KeyValuePair<ComparisonType, object>>>
                        {
                            {
                                ConditionType.AllOf, 
                                new KeyValuePair<ComparisonType, object>[]
                                {
                                    new(ComparisonType.EqualsSomthingHey_asdasdas_HELO, 3)
                                }
                            }
                        },
                        Actions = new Dictionary<RoutineActionType, RoutineAction>
                        {
                            {
                                RoutineActionType.SetValue,
                                new SetValueAction<decimal>()
                                {
                                    Target = "qweqwe",
                                    Value = 0,
                                }
                            },
                        }
                    }
                }
            }
        };

        return new JsonResult(config);
    }


}
