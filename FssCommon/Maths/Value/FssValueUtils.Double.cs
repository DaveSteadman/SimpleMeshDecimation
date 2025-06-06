using System;

// FssValueUtils: A static class for common (double precision) math routines, useful as helper routines for higher-level functionality.

public static partial class FssValueUtils
{
    // ------------------------------------------------------------------------
    // Function to essentially allow the % operator on double precision numbers.

    public static double Modulo(double value, double rangesize)
    {
        if (Math.Abs(rangesize) < FssConsts.ArbitraryMinDouble)
            throw new ArgumentException("rangeSize too small", nameof(rangesize));

        double wrappedvalue = value - rangesize * Math.Floor(value / rangesize);
        if (wrappedvalue < 0) wrappedvalue += rangesize;
        return wrappedvalue;
    }

    // ------------------------------------------------------------------------

    public static double ClampD(double val, double rangemin, double rangemax)
    {
        if (rangemin > rangemax)
            (rangemin, rangemax) = (rangemax, rangemin);

        return (val < rangemin) ? rangemin : ((val > rangemax) ? rangemax : val);
    }

    public static double LimitToRange(double val, double rangemin, double rangemax)
    {
        if (rangemin > rangemax)
            (rangemin, rangemax) = (rangemax, rangemin);

        return (val < rangemin) ? rangemin : ((val > rangemax) ? rangemax : val);
    }

    public static double WrapToRange(double val, double rangemin, double rangemax)
    {
        if (rangemin > rangemax)
            (rangemin, rangemax) = (rangemax, rangemin);

        double diff = rangemax - rangemin;
        double wrappedvalue = Modulo(val - rangemin, diff);
        return wrappedvalue + rangemin;
    }

    // ------------------------------------------------------------------------

    // Determine the difference between two values that wrap (ie longitude or angle values).

    public static double DiffInWrapRange(double rangemin, double rangemax, double val1, double val2)
    {
        if (rangemin > rangemax)
            (rangemin, rangemax) = (rangemax, rangemin);

        // First, wrap both values to the range.
        double wrappedVal1 = WrapToRange(val1, rangemin, rangemax);
        double wrappedVal2 = WrapToRange(val2, rangemin, rangemax);

        // Compute the difference.
        double diff = wrappedVal2 - wrappedVal1;

        // Here's the key: we want to adjust this difference so that it represents the shortest turn
        // from val1 to val2, taking into account the possibility of wrapping around from rangemax to rangemin.
        // We will first compute the size of the range.
        double rangeSize = rangemax - rangemin;

        // If the absolute difference is greater than half of the range size, we know that it would be
        // shorter to go the other way around the circle to get from val1 to val2.
        if (Math.Abs(diff) > rangeSize / 2)
        {
            // There are two cases to consider.
            if (diff > 0)
            {
                // If diff is positive, then val2 is counterclockwise from val1 and it would be shorter to
                // go clockwise from val1 to get to val2. So we subtract the rangeSize from diff.
                diff -= rangeSize;
            }
            else
            {
                // If diff is negative, then val2 is clockwise from val1 and it would be shorter to go
                // counterclockwise from val1 to get to val2. So we add the rangeSize to diff.
                diff += rangeSize;
            }
        }
        return diff;
    }

    // ------------------------------------------------------------------------

    // Take an input fraction (0..1) and return the index of the value in the range minval..maxval
    // that corresponds to that fraction.

    public static int IndexFromFraction(double fraction, int minval, int maxval)
    {
        double limitedFraction = LimitToRange(fraction, 0, 1);
        int diff = maxval - minval;
        return (int)(limitedFraction * diff) + minval;
    }

    public static int IndexFromIncrement(double minLimit, double increment, double val)
    {
        int retInc = 0;
        double workingVal = (minLimit + increment);

        while (workingVal < val)
        {
            workingVal += increment;
            retInc++;
        }
        return retInc;
    }

    // ------------------------------------------------------------------------

    public static bool IsInRange(double val, double rangemin, double rangemax)
    {
        if (rangemin > rangemax)
            (rangemin, rangemax) = (rangemax, rangemin);

        return val >= rangemin && val <= rangemax;
    }

    // ------------------------------------------------------------------------
    // Uses a y=mx+c mechanism to convert a value between an input and output range.

    public static double ScaleVal(double inval, double inrangemin, double inrangemax, double outrangemin, double outrangemax)
    {
        // Check ranges in order
        if (inrangemin  > inrangemax)  (inrangemin,  inrangemax)  = (inrangemax,  inrangemin);
        if (outrangemin > outrangemax) (outrangemin, outrangemax) = (outrangemax, outrangemin);

        // Check in the input value is in range
        inval = LimitToRange(inval, inrangemin, inrangemax);

        // determine the different ranges to multiply the values by
        double indiff  = inrangemax  - inrangemin;
        double outdiff = outrangemax - outrangemin;

        // check in range and out range are not too small to function
        if (Math.Abs(indiff)  < FssConsts.ArbitraryMinDouble) throw new ArgumentException("ScaleVal input range too small", nameof(indiff));
        if (Math.Abs(outdiff) < FssConsts.ArbitraryMinDouble) throw new ArgumentException("ScaleVal output range too small", nameof(outdiff));

        double diffratio = outdiff / indiff;

        double outval = ((inval - inrangemin) * diffratio) + outrangemin;
        return LimitToRange(outval, outrangemin, outrangemax);
    }

    // ------------------------------------------------------------------------

    public static double Interpolate(double rangemin, double rangemax, double fraction)
    {
        if (rangemin > rangemax)
            (rangemin, rangemax) = (rangemax, rangemin);

        // fraction = LimitToRange(fraction, 0, 1); // Deactivating the limit check, so extrapolations are also available to the caller.
        double diffVal = rangemax - rangemin;
        return rangemin + (diffVal * fraction);
    }

    // ------------------------------------------------------------------------
    // Useful way to test floating point numbers.

    // usage: FssValueUtils.EqualsWithinTolerance(0.1, 0.1, 0.01);
    public static bool EqualsWithinTolerance(double val, double matchval, double tolerance = FssConsts.ArbitraryMinDouble)
    {
        return Math.Abs(val - matchval) <= tolerance;
    }

    // ------------------------------------------------------------------------

    // Generates a random float value between minVal and maxVal (inclusive)
    public static double RandomInRange(double minVal, double maxVal)
    {
        double range = maxVal - minVal;
        double sample = random.NextDouble();
        double scaled = (sample * range) + minVal;
        return scaled;
    }
}
