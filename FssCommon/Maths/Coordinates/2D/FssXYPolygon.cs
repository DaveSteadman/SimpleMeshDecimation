using System;
using System.Collections.Generic;

#nullable enable

// Represents an immutable 2D polygon on a Fss. The polygon is considered closed,
// with an implicit final line connecting the last and first points. There's no need
// to repeat the first point at the end of the vertices list.

public class FssXYPolygon : IFssXY
{
    public IReadOnlyList<FssXYPoint> Vertices { get; }

    public FssXYPolygon(IEnumerable<FssXYPoint> vertices)
    {
        Vertices = new List<FssXYPoint>(vertices).AsReadOnly();
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Standard Shape Operations
    // --------------------------------------------------------------------------------------------

    public FssXYRect? AABB()
    {
        if (Vertices.Count == 0) return null;

        double minX = Vertices[0].X;
        double minY = Vertices[0].Y;
        double maxX = Vertices[0].X;
        double maxY = Vertices[0].Y;

        foreach (var vertex in Vertices)
        {
            if (vertex.X < minX) minX = vertex.X;
            if (vertex.Y < minY) minY = vertex.Y;
            if (vertex.X > maxX) maxX = vertex.X;
            if (vertex.Y > maxY) maxY = vertex.Y;
        }

        return new FssXYRect(minX, minY, maxX, maxY);
    }
    
    public bool Contains(FssXYPoint xy)
    {
        if (Vertices.Count < 3) return false;

        // TODO

        return true;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Determine the area of the polygon - for a simple non-intersecting case.

    // Calculation by determining an average center location, then summing the areas of the triangles around it.
    // Uses FssXYPointOperations.TriangleArea(a, b, c) to calculate the area of each triangle around the center and point list.

    public double Area()
    {
        double area = 0;
        FssXYPoint center = Center();
        for (int i = 0; i < Vertices.Count - 1; i++) // Note: the < and -1 avoids the off-by-one error.
        {
            area += FssXYPointOperations.TriangleArea(center, Vertices[i], Vertices[i + 1]);
        }
        return area;
    }

    // --------------------------------------------------------------------------------------------

    // Determine the center of the polygon
    public FssXYPoint Center()
    {
        double x = 0;
        double y = 0;
        foreach (FssXYPoint vertex in Vertices)
        {
            x += vertex.X;
            y += vertex.Y;
        }
        return new FssXYPoint(x / Vertices.Count, y / Vertices.Count); // "numeric promotion" of the ints to doubles.
    }

    // --------------------------------------------------------------------------------------------

    // offset each point by a given x y

    public FssXYPolygon Offset(double x, double y)
    {
        List<FssXYPoint> newVertices = new List<FssXYPoint>();
        foreach (FssXYPoint vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(x, y));
        }
        return new FssXYPolygon(newVertices);
    }    

    // offset each point in the polygon by the given vector

    public FssXYPolygon Offset(FssXYPoint xy)
    {
        List<FssXYPoint> newVertices = new List<FssXYPoint>();
        foreach (FssXYPoint vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(xy));
        }
        return new FssXYPolygon(newVertices);
    }
}
