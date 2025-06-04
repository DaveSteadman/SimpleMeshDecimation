using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

public class FssXYVector
{
    public double X { get; set; }
    public double Y { get; set; }

    public double Magnitude
    {
        get
        {
            return Math.Sqrt((X * X) + (Y * Y));
        }
        set
        {
            double currMag = Magnitude; // store, avoid calculating twice
            if (currMag < FssConsts.ArbitraryMinDouble) // if too close to a div0
            {
                X = value; Y = 0;
            }
            else
            {
                double scaleFactor = value / currMag;
                X *= scaleFactor;
                Y *= scaleFactor;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYVector(double x, double y)
    {
        X = x;
        Y = y;
    }

    public FssXYVector(FssXYVector vector)
    {
        X = vector.X;
        Y = vector.Y;
    }

    public static FssXYVector Zero
    {
        get { return new FssXYVector(0, 0); }
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Return the angle FROM this vector TO the given vector - East / Positive X axis is zero
    public double AngleToRads(double x, double y)
    {
        if (FssValueUtils.EqualsWithinTolerance(x, X) && FssValueUtils.EqualsWithinTolerance(y, Y))
            return 0;

        return Math.Atan2(y - Y, x - X);
    }

    public double AngleToRads(FssXYVector vector)
    {
        if (EqualsWithinTolerance(this, vector))
            return 0;

        return Math.Atan2(vector.Y - Y, vector.X - X);
    }

    // Return a new vector offset by an XY amount
    public FssXYVector Offset(double x, double y)
    {
        return new FssXYVector(X + x, Y + y);
    }

    public FssXYVector Offset(FssXYVector vector)
    {
        return new FssXYVector(X + vector.X, Y + vector.Y);
    }

    public FssXYVector Offset(FssXYPolarOffset o)
    {
        FssXYVector vector = o.ToXYVector();
        return Offset(vector);
    }

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static FssXYVector Sum(FssXYVector a, FssXYVector b)
    {
        return new FssXYVector(a.X + b.X, a.Y + b.Y);
    }

    public static FssXYVector Diff(FssXYVector a, FssXYVector b)
    {
        return new FssXYVector(a.X - b.X, a.Y - b.Y);
    }

    public static FssXYVector Scale(FssXYVector a, double b)
    {
        return new FssXYVector(a.X * b, a.Y * b);
    }

    public static bool EqualsWithinTolerance(FssXYVector a, FssXYVector b, double tolerance = FssConsts.ArbitraryMinDouble)
    {
        return FssValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance) && FssValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload  
    public static FssXYVector operator +(FssXYVector a, FssXYVector b) { return new FssXYVector(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting vectors
    public static FssXYVector operator -(FssXYVector a, FssXYVector b) { return new FssXYVector(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling vector in relation to origin
    public static FssXYVector operator *(FssXYVector a, double b) { return new FssXYVector(a.X * b, a.Y * b); }
    public static FssXYVector operator *(double b, FssXYVector a) { return new FssXYVector(a.X * b, a.Y * b); }

    // / operator overload for scaling vector in relation to origin
    public static FssXYVector operator /(FssXYVector a, double b) { return new FssXYVector(a.X / b, a.Y / b); }
    public static FssXYVector operator /(double b, FssXYVector a) { return new FssXYVector(a.X / b, a.Y / b); }

    // --------------------------------------------------------------------------------------------
    // Vector-specific methods
    // --------------------------------------------------------------------------------------------

    public static double CrossProduct(FssXYVector a, FssXYVector b)
    {
        return (a.X * b.Y) - (a.Y * b.X);
    }

    public double DotProduct(FssXYVector vector)
    {
        return (X * vector.X) + (Y * vector.Y);
    }

    public FssXYVector Normalize()
    {
        double mag = Magnitude;
        return new FssXYVector(X / mag, Y / mag);
    }

    public FssXYVector Rotate(double angleRads)
    {
        double cos = Math.Cos(angleRads);
        double sin = Math.Sin(angleRads);
        return new FssXYVector((X * cos) - (Y * sin), (X * sin) + (Y * cos));
    }

    public FssXYVector Negate()
    {
        return Rotate(Math.PI);
    }

    public double AngleBetween(FssXYVector vector)
    {
        double dot = DotProduct(vector);
        double mags = Magnitude * vector.Magnitude;
        return Math.Acos(dot / mags);
    }

    public override string ToString()
    {
        return $"({X:F3}, {Y:F3})";
    }
}
