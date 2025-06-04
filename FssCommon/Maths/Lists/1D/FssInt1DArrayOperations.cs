using System;
using System.Collections.Generic;

/// <summary>
/// Utility class for creating 1D integer ranges.
/// </summary>
public static class FssInt1DArrayOperations
{
    /// <summary>
    /// Creates a range of integers from start to end (inclusive), incrementing by a fixed step.
    /// Ensures the end value is included even if not aligned with the step.
    /// </summary>
    /// Usage: FssInt1DArray range = FssInt1DArrayOperations.CreateRangeByStep(0, 10, 2);
    public static FssInt1DArray CreateRangeByStep(int start, int end, int step)
    {
        if (step <= 0)
            throw new ArgumentException("Step must be greater than zero.");

        var result = new FssInt1DArray();

        for (int value = start; value <= end; value += step)
        {
            result.Add(value);
        }

        if ((end - start) % step != 0 && result[^1] != end)
        {
            result.Add(end);
        }

        return result;
    }

    /// <summary>
    /// Creates a range of integers from start to end (inclusive), spaced as evenly as possible over a fixed number of divisions.
    /// Includes both start and end. Optionally suppresses duplicate values if step < 1.
    /// </summary>
    /// Usage: FssInt1DArray range = FssInt1DArrayOperations.CreateRangeByDivision(0, 10, 5);
    public static FssInt1DArray CreateRangeByDivision(int start, int end, int divisions, bool allowDuplicates = true)
    {
        if (divisions <= 0)
            throw new ArgumentException("Divisions must be greater than zero.");

        var result = new FssInt1DArray();

        if (divisions == 1)
        {
            result.Add(start);
            if (end != start || allowDuplicates)
                result.Add(end);
            return result;
        }

        float step = (float)(end - start) / divisions;
        int? lastValue = null;

        for (int i = 0; i <= divisions; i++)
        {
            int value = (int)Math.Round(start + i * step);

            if (allowDuplicates || lastValue != value)
            {
                result.Add(value);
                lastValue = value;
            }
        }

        return result;
    }
}
