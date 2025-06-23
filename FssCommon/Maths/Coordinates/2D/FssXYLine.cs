using System;

public class FssXYLine : IFssXY
{
    public FssXYPoint P1 { get; }
    public FssXYPoint P2 { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Length => P1.DistanceTo(P2);
    public FssXYVector Direction => P1.VectorTo(P2); // return a position as the vector from 0,0

    public FssXYVector DirectionUnitVector => P1.VectorTo(P2).Normalize();

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYLine(double x1, double y1, double x2, double y2)
    {
        P1 = new FssXYPoint(x1, y1);
        P2 = new FssXYPoint(x2, y2);
    }

    public FssXYLine(FssXYPoint p1, FssXYPoint p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public FssXYLine(FssXYLine line)
    {
        P1 = line.P1;
        P2 = line.P2;
    }

    public static FssXYLine Zero => new FssXYLine(0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // #MARK: IFssXY Operations
    // --------------------------------------------------------------------------------------------

    public FssXYRect? AABB()
    {
        if (Length < FssConsts.ArbitraryMinDouble) return null;

        double minX = Math.Min(P1.X, P2.X);
        double minY = Math.Min(P1.Y, P2.Y);
        double maxX = Math.Max(P1.X, P2.X);
        double maxY = Math.Max(P1.Y, P2.Y);
        return new FssXYRect(minX, minY, maxX, maxY);
    }

    public bool Contains(FssXYPoint xy) => false; // line has no area to contain a point

    // --------------------------------------------------------------------------------------------
    // Position methods
    // --------------------------------------------------------------------------------------------

    // Get the centre point of the line

    public FssXYPoint MidPoint()
    {
        return new FssXYPoint((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
    }

    // --------------------------------------------------------------------------------------------
    // Line methods
    // --------------------------------------------------------------------------------------------

    // Return a new line object, will all points offset by an XY amount.

    public FssXYLine Offset(double x, double y)
    {
        return new FssXYLine(P1.Offset(x, y), P2.Offset(x, y));
    }

    public FssXYLine Offset(FssXYPoint xy)
    {
        return new FssXYLine(P1.Offset(xy), P2.Offset(xy));
    }

    // --------------------------------------------------------------------------------------------
    // Misc methods
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"({P1.X:F3}, {P1.Y:F3}) -> ({P2.X:F3}, {P2.Y:F3})";
    }
}
