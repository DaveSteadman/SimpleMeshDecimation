using System;

// Represents a triangle in 2D space defined by three points (A, B, C).
// Provides geometric utilities such as area, centroid, containment, and edge access.

public struct FssXYTriangle
{
    public FssXYPoint A { get; set; }
    public FssXYPoint B { get; set; }
    public FssXYPoint C { get; set; }

    public FssXYLine LineAB => new FssXYLine(A, B);
    public FssXYLine LineBC => new FssXYLine(B, C);
    public FssXYLine LineCA => new FssXYLine(C, A);

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Create a triangle from three points.
    public FssXYTriangle(FssXYPoint a, FssXYPoint b, FssXYPoint c)
    {
        A = a;
        B = b;
        C = c;
    }

    public static FssXYTriangle Zero { get => new FssXYTriangle(new FssXYPoint(0, 0), new FssXYPoint(0, 0), new FssXYPoint(0, 0)); }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Properties
    // --------------------------------------------------------------------------------------------

    // Returns the centroid (center point) of the triangle.
    public FssXYPoint CenterPoint() => new FssXYPoint((A.X + B.X + C.X) / 3.0, (A.Y + B.Y + C.Y) / 3.0);

    // Returns the area of the triangle.
    public double Area() => Math.Abs((A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2.0);

    // Returns the perimeter (sum of edge lengths) of the triangle.
    public double Perimeter() => LineAB.Length + LineBC.Length + LineCA.Length;

    // Returns true if the triangle is degenerate (area is zero or nearly zero).
    public bool IsDegenerate() => Area() < 1e-10;

    public FssXYRect AABB()
    {
        double minX = Math.Min(A.X, Math.Min(B.X, C.X));
        double maxX = Math.Max(A.X, Math.Max(B.X, C.X));
        double minY = Math.Min(A.Y, Math.Min(B.Y, C.Y));
        double maxY = Math.Max(A.Y, Math.Max(B.Y, C.Y));

        return new FssXYRect(minX, minY, maxX - minX, maxY - minY);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Utilities
    // --------------------------------------------------------------------------------------------

    // Returns true if the given point lies inside the triangle (or on its edge).
    // This is done by comparing the area of the triangle to the sum of the areas of three sub-triangles
    // formed by the test point and each pair of triangle vertices. If the sum of the sub-areas equals
    // the original area (within a small tolerance for floating-point precision), the point is inside or on the triangle.
    public bool Contains(FssXYPoint point)
    {
        double area = Area();
        double area1 = new FssXYTriangle(point, B, C).Area();
        double area2 = new FssXYTriangle(A, point, C).Area();
        double area3 = new FssXYTriangle(A, B, point).Area();
        return Math.Abs(area - (area1 + area2 + area3)) < 1e-10; // Allow for floating-point precision issues
    }

    public FssXYTriangle Inset(double inset)
    {
        // Construct lines for each edge
        var ab = new FssXYLine(A, B);
        var bc = new FssXYLine(B, C);
        var ca = new FssXYLine(C, A);

        // Inward-offset each line w.r.t. the opposite point
        var abIn = FssXYLineOperations.OffsetInward(ab, C, inset);
        var bcIn = FssXYLineOperations.OffsetInward(bc, A, inset);
        var caIn = FssXYLineOperations.OffsetInward(ca, B, inset);

        // Intersect adjacent pairs
        if (!FssXYLineOperations.TryIntersect(abIn, bcIn, out var i1)) return this;
        if (!FssXYLineOperations.TryIntersect(bcIn, caIn, out var i2)) return this;
        if (!FssXYLineOperations.TryIntersect(caIn, abIn, out var i3)) return this;

        return new FssXYTriangle(i1, i2, i3);
    }

    // Returns a new triangle translated by the given offset.
    public FssXYTriangle Translate(double dx, double dy) => new FssXYTriangle(A.Offset(dx, dy), B.Offset(dx, dy), C.Offset(dx, dy));

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Management
    // --------------------------------------------------------------------------------------------

    // Returns the triangle's vertices as an array.
    public FssXYPoint[] ToArray() => new[] { A, B, C };

    // Returns a string representation of the triangle.
    public override string ToString() => $"Triangle: A={A}, B={B}, C={C}, Area={Area():F2}, Perimeter={Perimeter():F2}, Centroid={CenterPoint()}";


}

