namespace Pgotchi.Data.Models;

public enum RoutineActionType
{
    SetValue = 0,
}

public class RoutineAction
{

}

[Serializable]
public class SetValueAction<TValue> : RoutineAction
{
    public string Target { get; set; } = null!;
    public TValue Value { get; set; } = default!;
    public TValue ? Otherwise { get; set; }
    public DurationOptions<TValue>? DurationOptions { get; set; }
}

public class DurationOptions<TValue>
{
    public decimal Duration { get; set; }
    public TValue FallbackValue { get; set; } = default!;
}