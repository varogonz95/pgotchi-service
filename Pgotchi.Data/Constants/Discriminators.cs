using Pgotchi.Data.Models;

namespace Pgotchi.Data.Constants;

internal class DiscriminatorNames
{
    public const string ForRoutineAction = "type";
}

internal class DiscriminatorValues
{
    public const string SetValueAction = nameof(RoutineActionType.SetValue);
}