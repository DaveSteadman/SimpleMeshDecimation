using System;

// FssXYArc: class representing a 2D arc, which can be clockwise or anti-clockwise in direction.
// Will use radians natively and the distance units are abstract.
// Will use FssValueUtils.Angle to ensure angles are wrapped and differences are calculated correctly.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

public class FssXYArc : IFssXY
{
    public FssXYPoint Center { get; }
    public double Radius { get; }
    public double StartAngleRads { get; }
    public double DeltaAngleRads { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double EndAngleRads  { get { return StartAngleRads + DeltaAngleRads; } }

    public double Diameter           { get { return 2 * Radius; } }
    public double AngleSpanRads      { get { return FssValueUtils.AngleDiffRads(StartAngleRads, EndAngleRads); } }
    public double LengthCurved       { get { return Radius * AngleSpanRads; } } 
    public double LengthStraightLine { get { return Math.Sqrt(Diameter * Diameter + LengthCurved * LengthCurved); } }

    public FssXYPoint StartPoint  { get { return FssXYPointOperations.OffsetPolar(Center, Radius, StartAngleRads); } }
    public FssXYPoint EndPoint    { get { return FssXYPointOperations.OffsetPolar(Center, Radius, EndAngleRads); } }

    public double StartAngleDegs  { get { return FssValueUtils.RadsToDegs(StartAngleRads); } }
    public double EndAngleDegs    { get { return FssValueUtils.RadsToDegs(EndAngleRads); } }
    public double AngleSpanDegs   { get { return FssValueUtils.RadsToDegs(AngleSpanRads); } }

    public FssXYCircle Circle     { get { return new FssXYCircle(Center, Radius); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // The delta angle can be positive or negative, allowing us to describe clockwise and anti-clockwise arcs,
    // as well s those going wrapping around.

    public FssXYArc(FssXYPoint center, double radius, double startAngleRads, double deltaAngleRads)
    {
        Center         = center;
        Radius         = radius;
        StartAngleRads = startAngleRads;
        DeltaAngleRads = deltaAngleRads;
    }

    public FssXYArc(FssXYArc arc)
    {
        Center         = arc.Center;
        Radius         = arc.Radius;
        StartAngleRads = arc.StartAngleRads;
        DeltaAngleRads = arc.DeltaAngleRads;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public FssXYArc Offset(double dx, double dy)
    {
        return new FssXYArc(Center.Offset(dx, dy), Radius, StartAngleRads, DeltaAngleRads);
    }

    // return if the angle is within the arc

    public bool ContainsAngle(double angleRads)
    {
        // return FssValueUtils.IsAngleInRangeRads(angleRads, StartAngleDegs, EndAngleDegs);

        return FssValueUtils.IsAngleInRangeRadsDelta(angleRads, StartAngleDegs, DeltaAngleRads);
    }

    // List the start, the end and any angles at the extremes of the arc that are within the arc.
    public List<FssXYPoint> ExtremePointsList()
    {
        var angles = new[] { 0, Math.PI / 2, Math.PI, 3 * Math.PI / 2 };

        List<FssXYPoint> points = new List<FssXYPoint> { StartPoint, EndPoint };

        foreach (var angle in angles)
        {
            if (ContainsAngle(angle))
            {
                points.Add(FssXYPointOperations.OffsetPolar(Center, Radius, angle));
            }
        }

        return points;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: IFssXY Operations
    // --------------------------------------------------------------------------------------------

    public FssXYRect? AABB()
    {
        // Get the extreme points of the arc
        List<FssXYPoint> pointList = ExtremePointsList();

        // Check the list contains something valid
        if (pointList.Count < 2) return null;

        // Return the bounding box of the list of points
        return FssXYPointOperations.AABBFromList(pointList);
    }

    public bool Contains(FssXYPoint xy)
    {
        // Arc is a line with no area, return false.
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------


}
