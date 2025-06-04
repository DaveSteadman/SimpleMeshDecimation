using System;

// FssXYPolarOffset: Class representing an angle and distance in 2D space.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Object achieves some level of immuatbility by being a struct.
 
public struct FssXYPolarOffset : IFssXY
{
    // Main attributes
    public double AngleRads { set; get; }
    public double Distance { set; get; } 

    // Derived attributes
    public double AngleDegs => AngleRads * FssConsts.RadsToDegsMultiplier;


    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssXYPolarOffset(double angleRads, double distance)
    {
        this.AngleRads = angleRads;
        this.Distance = distance;
    }

    public FssXYVector ToXYVector()
    {
        return new FssXYVector(
            Distance * Math.Cos(AngleRads),
            Distance * Math.Sin(AngleRads)
        );
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: IFssXY Operations
    // --------------------------------------------------------------------------------------------

    public FssXYRect? AABB() => null;

    public bool Contains(FssXYPoint xy) => false; 

}
