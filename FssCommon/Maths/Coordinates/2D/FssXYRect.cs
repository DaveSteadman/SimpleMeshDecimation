using System;

// Class defining a simple 2D rectangle, with no rotation

public class FssXYRect : IFssXY
{
    // Main attributes
    public FssXYPoint TopLeft { get; }
    public FssXYPoint BottomRight { get; }

    // Derived attributes
    public double Width  => BottomRight.X - TopLeft.X;
    public double Height => BottomRight.Y - TopLeft.Y;
    public double Left   => TopLeft.X;
    public double Right  => BottomRight.X;
    public double Top    => TopLeft.Y;
    public double Bottom => BottomRight.Y;
    public double Area   => Width * Height;

    public FssXYPoint TopRight     => new FssXYPoint(Right, Top);
    public FssXYPoint BottomLeft   => new FssXYPoint(Left, Bottom);

    public FssXYPoint Center       => new FssXYPoint((Left + Right) / 2, (Top + Bottom) / 2);
    public FssXYPoint TopCenter    => new FssXYPoint((Left + Right) / 2, Top);
    public FssXYPoint BottomCenter => new FssXYPoint((Left + Right) / 2, Bottom);
    public FssXYPoint LeftCenter   => new FssXYPoint(Left, (Top + Bottom) / 2);
    public FssXYPoint RightCenter  => new FssXYPoint(Right, (Top + Bottom) / 2);

    public FssXYLine TopLine       => new FssXYLine(TopLeft, TopRight);
    public FssXYLine BottomLine    => new FssXYLine(BottomLeft, BottomRight);
    public FssXYLine LeftLine      => new FssXYLine(TopLeft, BottomLeft);
    public FssXYLine RightLine     => new FssXYLine(TopRight, BottomRight);

    public static FssXYRect Zero 
    {
        get { return new FssXYRect(0, 0, 0, 0); }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Floating Point Accessors
    // --------------------------------------------------------------------------------------------

    public float WidthF  => (float)Width;
    public float HeightF => (float)Height;

    public float LeftF   => (float)Left;
    public float RightF  => (float)Right;
    public float TopF    => (float)Top;
    public float BottomF => (float)Bottom;

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYRect(double x1, double y1, double x2, double y2)
    {
        TopLeft     = new (x1, y1);
        BottomRight = new (x2, y2);
    }

    public FssXYRect(FssXYPoint topLeft, FssXYPoint bottomRight)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;
    }

    public FssXYRect(FssXYRect rect)
    {
        TopLeft     = rect.TopLeft;
        BottomRight = rect.BottomRight;
    }

    public FssXYRect(FssXYPoint centerPoint, double sideLength)
    {
        double halfSideLen = sideLength / 2;
        
        TopLeft     = new FssXYPoint(centerPoint.X - halfSideLen, centerPoint.Y - halfSideLen);
        BottomRight = new FssXYPoint(centerPoint.X + halfSideLen, centerPoint.Y + halfSideLen);       
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Standard Shape Operations
    // --------------------------------------------------------------------------------------------

    public FssXYRect? AABB()
    {
        if ((Height > FssConsts.ArbitraryMinDouble) && (Width > FssConsts.ArbitraryMinDouble))
            return this; // The rectangle itself is already axis-aligned
        return null;
    }

    public bool Contains(FssXYPoint xy)
    {
        return xy.X >= Left && xy.X <= Right && xy.Y >= Top && xy.Y <= Bottom;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Intersections
    // --------------------------------------------------------------------------------------------

    // Function checks that the other box is separate across each axis/limit.
    // If it fails that check (the not operator), then the boxes intersect.
    public bool DoesIntersect(FssXYRect other)
    {
        return !(other.Left > Right ||     // Check if B is completely to the right of A
                 other.Right < Left ||     // Check if B is completely to the left of A
                 other.Top > Bottom ||     // Check if B is completely below A
                 other.Bottom < Top);      // Check if B is completely above A
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Inset
    // --------------------------------------------------------------------------------------------
    
    public FssXYRect Inset(double d)
    {
        return new FssXYRect(Left + d, Top + d, Right - d, Bottom - d);
    }

    public FssXYRect Inset(double dx, double dy)
    {
        return new FssXYRect(Left + dx, Top + dy, Right - dx, Bottom - dy);
    }

    public FssXYRect Inset(double left, double top, double right, double bottom)
    {
        return new FssXYRect(Left + left, Top + top, Right - right, Bottom - bottom);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Offset
    // --------------------------------------------------------------------------------------------

    public FssXYRect Offset(double dx, double dy)
    {
        return new FssXYRect(Left + dx, Top + dy, Right + dx, Bottom + dy);
    }

    public FssXYRect Offset(FssXYPoint offset)
    {
        return new FssXYRect(Left + offset.X, Top + offset.Y, Right + offset.X, Bottom + offset.Y);
    }

}
