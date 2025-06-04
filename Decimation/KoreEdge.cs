


using System;


// A class holding two point index values, that represent the two ends of an edge, either on a triangle or part of a mesh.
// Increaing index values are important in ensuring we don't have duplicate edges.

public struct KoreEdge : IEquatable<KoreEdge>
{
    public int P1 { get; }
    public int P2 { get; }
    public double Length { get; set; } // Length of the edge, used for triangulation

    public FssXYLine mathLine { get; set; }

    public KoreEdge(int u, int v)
    {
        // sort so U < V, giving a unique representation for the undirected edge
        if (u < v)
        {
            P1 = u;
            P2 = v;
        }
        else
        {
            P1 = v;
            P2 = u;
        }
        Length = 0.0; // Default length, can be set later
    }

    // IEquatable<Edge>
    public bool Equals(KoreEdge other) => P1 == other.P1 && P2 == other.P2;

    public override bool Equals(object? obj) => obj is KoreEdge e && Equals(e);

    // a simple combination that avoids collisions for small indices
    public override int GetHashCode() => (P1 * 397) ^ P2;

    public static bool operator ==(KoreEdge left, KoreEdge right) => left.Equals(right);

    public static bool operator !=(KoreEdge left, KoreEdge right) => !left.Equals(right);

    public override string ToString() => $"Edge({P1},{P2})";


    // public void CalcLength(KoreMesh koreMesh)
    // {
    //     var p1 = koreMesh.GetPointByIndex(P1);
    //     var p2 = koreMesh.GetPointByIndex(P2);
    //     Length = Math.Sqrt(Math.Pow(p2.mathPos.X - p1.mathPos.X, 2) + Math.Pow(p2.mathPos.Y - p1.mathPos.Y, 2));
    // }

    public void CalcLine(KoreMesh koreMesh)
    {
        var p1 = koreMesh.GetPointByIndex(P1);
        var p2 = koreMesh.GetPointByIndex(P2);

        var pointLine = new FssXYLine(p1.mathPos, p2.mathPos);
        var insetLine = FssXYLineOperations.InsetLine(pointLine, 0.001);

        Length = p1.mathPos.DistanceTo(p2.mathPos);

        // Update the mathLine to the inset line for drawing purposes
        mathLine = insetLine;
    }

    public bool Intersects(KoreEdge compEdge)
    {
        return FssXYLineOperations.DoesIntersect(mathLine, compEdge.mathLine);
    }

    public bool SharePoint(KoreEdge compEdge)
    {
        return P1 == compEdge.P1 || P1 == compEdge.P2 || P2 == compEdge.P1 || P2 == compEdge.P2;
    }

    public int GetThirdPointIndex(KoreEdge compEdge)
    {
        if (P1 != compEdge.P1 && P1 != compEdge.P2)
            return P1;
        if (P2 != compEdge.P1 && P2 != compEdge.P2)
            return P2;
        throw new InvalidOperationException("No third point index found in the edge comparison.");
    }

}
