namespace Pgotchi.Data.Models;

public class RoutineConfig
{
    public IDictionary<ConditionType, ICollection<KeyValuePair<ComparisonType, object>>> Conditions { get; set; } = new Dictionary<ConditionType, ICollection<KeyValuePair<ComparisonType, object>>>();
    public IDictionary<RoutineActionType, RoutineAction> Actions { get; set; } = new Dictionary<RoutineActionType, RoutineAction>();
}

public class ScheduledRoutineConfig : RoutineConfig
{
    public Schedule Schedule { get; set; } = new();
}

public class Schedule
{
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public ScheduleRepeatOptions? Repeats { get; set; }
}

public class ScheduleRepeatOptions
{
    public RepetitionFrequency Frequency { get; set; }
    //public Condition<object>? Until { get; set; }
}

public enum RepetitionFrequency
{
    Yearly = 0,
    Monthly = 1,
    Daily = 2,
}