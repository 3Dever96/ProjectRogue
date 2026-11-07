/// <summary>
/// Represents a single modifier applied to a character stat.
/// This tracks the modifier value, how it should be applied, and the order in which it should be processed.
/// </summary>
public class StatModifier
{
    // The numeric effect of the modifier.
    public readonly float Value;

    // The type of modifier, such as flat bonus or percentage-based adjustment.
    public readonly StatModType Type;

    // Priority used to sort modifiers before applying them.
    public readonly int Order;

    // Optional source object that created the modifier, useful for removing modifiers by source.
    public readonly object Source;

    public StatModifier(float value, StatModType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    // Creates a modifier using the stat type value as the sort order.
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }

    // Creates a modifier with a custom execution order and no source reference.
    public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

    // Creates a modifier using the default order for its type.
    public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }
}

/// <summary>
/// Defines the different ways a stat modifier can affect a value.
/// </summary>
public enum StatModType
{
    // Adds a flat amount to the stat.
    Flat = 100,

    // Adds a percentage bonus on top of the current value.
    PercentAdd = 200,

    // Multiplies the current value by a percentage modifier.
    PercentMult = 300
}
