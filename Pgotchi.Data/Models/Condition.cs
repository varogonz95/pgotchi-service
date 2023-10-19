namespace Pgotchi.Data.Models;

public enum ConditionType
{
    AllOf = 0,
    AnyOf = 1,
}

public enum ComparisonType
{
    GreaterThan = 0,
    GreaterOrEqualsTo = 1,
    LessThan = 2,
    LessOrEqualsTo = 3,
    EqualsSomthingHey_asdasdas_HELO = 4,
    NotEquals = 5,
    Between = 6,
}

public interface ICondition<T>
{
    T Value { get; set; }
}

public abstract class Condition<TValue> : ICondition<TValue>
{
    public ComparisonType Comparison { get; }
    public TValue Value { get; set; } = default!;

    public Condition(ComparisonType comparisonType)
    {
        Comparison = comparisonType;
    }
}

public class GreaterThanCondition<T> : Condition<T>
{
    public GreaterThanCondition() : base(ComparisonType.GreaterThan)
    {
    }

    public GreaterThanCondition(T value) : this()
    {
        Value = value;
    }
}

public class BetweenCondition<T> : Condition<(T, T)>
{
    public BetweenCondition() : base(ComparisonType.Between)
    {
    }

    public BetweenCondition(T left, T right) : this()
    {
        Value = (left, right);
    }
}