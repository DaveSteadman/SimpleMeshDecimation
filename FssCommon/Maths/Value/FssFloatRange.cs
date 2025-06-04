using System;

public class FssFloatRange
{
    // Attributes, read-only after creation, and calc a range.
    public float Min { get; private set; }
    public float Max { get; private set; }
    public float Range { get { return Max - Min; } }
    public RangeBehavior Behavior { get; private set; }

    public FssFloatRange(float min, float max, RangeBehavior behavior = RangeBehavior.Limit)
    {
        if (min > Max)
            (min, max) = (max, min);

        Min = min;
        Max = max;
        Behavior = behavior;
    }

    // Predefined constant ranges
    public static readonly FssFloatRange ZeroToOne            = new FssFloatRange(0f, 1f);
    public static readonly FssFloatRange ZeroTo360Degrees     = new FssFloatRange(0f, 360f);
    public static readonly FssFloatRange Minus180To180Degrees = new FssFloatRange(-180f, 180f);
    public static readonly FssFloatRange ZeroToTwoPiRadians   = new FssFloatRange(0f, (float)(Math.PI * 2));
    public static readonly FssFloatRange MinusPiToPiRadians   = new FssFloatRange((float)(-Math.PI), (float)Math.PI);

    public bool IsInRange(float val)
    {
        if (val > Max) return false;
        if (val < Min) return false;
        return true;
    }

    public float FractionInRange(float val)
    {
        if (val < Min) return 0f;
        if (val > Max) return 1f;
        return (val - Min) / Range;
    }

    public float Apply(float value)
    {
        if (IsInRange(value))
            return value;

        switch (Behavior)
        {
            case RangeBehavior.Wrap: 
                return Wrap(value);
            case RangeBehavior.Limit:
                return Clamp(value);
            default:
                throw new InvalidOperationException("Unsupported range behavior.");
        }
    }

    public float Clamp(float value)
    {
        if (value < Min) return Min;
        if (value > Max) return Max;
        return value;
    }

    public float Wrap(float value)
    {
        float offset = (value - Min) % Range;
        return (offset >= 0) ? Min + offset : Max + offset;
    }

    public override string ToString()
    {
        return $"Range: [{Min:F3}, {Max:F3}]";
    }
}
