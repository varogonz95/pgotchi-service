namespace Pgotchi.Data.Models;

public class PeripheralConfig
{
    public string Name { get; set; } = null!;
    public PeripheralType Type { get; set; }
    public bool? Display { get; set; }
    //public int? DisplayRow { get; set; }
    public bool? Reverse { get; set; }
    public bool? Autostart { get; set; }
}

public enum PeripheralType
{
    LightLevel = 0,
    SoilMoisture = 1,
    Relay = 2,
}