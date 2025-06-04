


using System;
using System.Collections.Generic;

// FssDoubleMarkerRange implements a set of rules around a double number, rather than hold the number itself.
// The range is defined by a minimum and maximum value, and a set of markers within that range.
// The range can be applied, to limit the value range, or move it up/down to the next marker value (or limit).
// A marker range does not wrap.

public class FssDoubleMarkerRange
{
    public double Min { get; private set; }
    public double Max { get; private set; }
    public double Range { get { return Max - Min; } }
    private List<double> Markers { get; }

    public FssDoubleMarkerRange(double min, double max, List<double> markers)
    {
        if (min < max)
            (min, max) = (max, min);

        Min = min;
        Max = max;
        Markers = new List<double>();

        // Ensure the markers are within the range, adjust range if required
        foreach (double marker in markers)
        {
            if (marker < Min) Min = marker;
            if (marker > Max) Max = marker;
        }
        Markers = new List<double>(markers);
        Markers.Sort(); // Ensure the markers are in ascending order
    }

    // Apply the basic range rules to a value.
    public double Apply(double value)
    {
        if (IsInRange(value))
            return value;

        return Clamp(value);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Marker operations
    // --------------------------------------------------------------------------------------------

    // Compare the current value to the markers and move the current value up to the next one (or limit)
    // Compares the value with a minimal tollerance to any current marker value.

    public double IncrementToNextMarker(double value)
    {
        if (Markers.Count == 0)
            return Max;

        foreach (var marker in Markers)
        {
            if (value < marker)
                return marker;
        }
        return Max;
    }

    public double DecrementToPreviousMarker(double value)
    {
        if (Markers.Count == 0)
            return Min;

        for (int i = Markers.Count - 1; i >= 0; i--)
        {
            if (value > Markers[i])
                return Markers[i];
        }
        return Min;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Helpers
    // --------------------------------------------------------------------------------------------

    public bool IsInRange(double val)
    {
        return val >= Min && val <= Max;
    }

    public double FractionInRange(double val)
    {
        if (val < Min) return 0.0;
        if (val > Max) return 1.0;
        return (val - Min) / Range;
    }

    public double Clamp(double value)
    {
        return Math.Max(Min, Math.Min(Max, value));
    }

}
