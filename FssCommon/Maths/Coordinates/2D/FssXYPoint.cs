using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

public class FssXYPoint : IFssXY
{
    public double X { get; set; }
    public double Y { get; set; }

    public double Magnitude {
        get
        {
            return Math.Sqrt((X * X) + (Y * Y));
        }
        set
        {
            double currMag = Magnitude; // store, avoid caclulating twice
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

    public FssXYPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public FssXYPoint(FssXYPoint xy)
    {
        X = xy.X;
        Y = xy.Y;
    }

    public static FssXYPoint Zero
    {
        get { return new FssXYPoint(0, 0); }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: IFssXY Operations
    // --------------------------------------------------------------------------------------------

    public FssXYRect? AABB() => null;

    public bool Contains(FssXYPoint xy) => false;

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Absolute value of the magnitude of the distance from this point, to the specified point.

    public double DistanceTo(double x, double y)
    {

        return Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
    }

    public double DistanceTo(FssXYPoint xy)
    {
        return Math.Sqrt(Math.Pow(X - xy.X, 2) + Math.Pow(Y - xy.Y, 2));
    }

    // Usage: FssXYPoint.PointsEqual(p1, p2)
    public static bool PointsEqual(FssXYPoint u, FssXYPoint v)
        => FssValueUtils.EqualsWithinTolerance(u.X, v.X) &&  FssValueUtils.EqualsWithinTolerance(u.Y, v.Y);


    // --------------------------------------------------------------------------------------------

    // return the angle FROM this point TO the given point - East / Positve X axis is zero
    public double AngleToRads(double x, double y)
    {
        if (FssValueUtils.EqualsWithinTolerance(x, X) && FssValueUtils.EqualsWithinTolerance(y, Y))
            return 0;

        return Math.Atan2(y - Y, x - X);
    }

    public double AngleToRads(FssXYPoint xy)
    {
        if (EqualsWithinTolerance(this, xy))
            return 0;

        // return the angle FROM this point TO the given point
        return Math.Atan2(xy.Y - Y, xy.X - X);
    }

    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public FssXYPoint Offset(double x, double y) => new FssXYPoint(X + x, Y + y);
    public FssXYPoint Offset(FssXYPoint xy) => new FssXYPoint(X + xy.X, Y + xy.Y);
    public FssXYPoint Offset(FssXYPolarOffset o) => Offset(FssXYPolarOffsetOperations.ToXY(o));

    public FssXYVector VectorTo(FssXYPoint remoteXY) => new FssXYVector(X - remoteXY.X, Y - remoteXY.Y);

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static FssXYPoint Sum(FssXYPoint a, FssXYPoint b) => new FssXYPoint(a.X + b.X, a.Y + b.Y);
    public static FssXYPoint Diff(FssXYPoint a, FssXYPoint b) => new FssXYPoint(a.X - b.X, a.Y - b.Y);
    public static FssXYPoint Scale(FssXYPoint a, double b) => new FssXYPoint(a.X * b, a.Y * b);

    public static bool EqualsWithinTolerance(FssXYPoint a, FssXYPoint b, double tolerance = FssConsts.ArbitraryMinDouble)
    {
        return FssValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance) && FssValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static FssXYPoint operator +(FssXYPoint a, FssXYPoint b)  { return new FssXYPoint(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static FssXYPoint operator -(FssXYPoint a, FssXYPoint b)  { return new FssXYPoint(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    // public static FssXYPoint operator *(FssXYPoint a, double b)        { return new FssXYPoint(a.X * b, a.Y * b); }
    // public static FssXYPoint operator *(double b, FssXYPoint a)        { return new FssXYPoint(a.X * b, a.Y * b); }

    // // / operator overload for scaling point in relation to origin
    // public static FssXYPoint operator /(FssXYPoint a, double b)        { return new FssXYPoint(a.X / b, a.Y / b); }
    // public static FssXYPoint operator /(double b, FssXYPoint a)        { return new FssXYPoint(a.X / b, a.Y / b); }

    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static FssXYPoint operator +(FssXYPoint a, FssXYVector b)  { return new FssXYPoint(a.X + b.X, a.Y + b.Y); }

    // - operator overload
    public static FssXYPoint operator -(FssXYPoint a, FssXYVector b)  { return new FssXYPoint(a.X - b.X, a.Y - b.Y); }

    // --------------------------------------------------------------------------------------------



    public override string ToString()
    {
        return $"({X:F3}, {Y:F3})";
    }
}