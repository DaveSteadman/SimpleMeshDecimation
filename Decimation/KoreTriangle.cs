


// triangle: Struct of three point indices, sorted so A < B < C


public struct KoreTriangle : IEquatable<KoreTriangle>
{
    public int A { get; }
    public int B { get; }
    public int C { get; }

    public FssXYTriangle _triangle = FssXYTriangle.Zero;

    /// <summary>
    /// Constructs a KoreTriangle from three vertex indices, sorting them so A < B < C.
    /// </summary>

    // i1 to i3 are the three vertex indices of the triangle - in ascending order
    public KoreTriangle(int i1, int i2, int i3)
    {
        // Sort the indices
        if (i1 > i2) Swap(ref i1, ref i2);
        if (i2 > i3) Swap(ref i2, ref i3);
        if (i1 > i2) Swap(ref i1, ref i2);

        A = i1;
        B = i2;
        C = i3;
    }

    private static void Swap(ref int x, ref int y)
    {
        int temp = x;
        x = y;
        y = temp;
    }

    /// <summary>
    /// Returns the three undirected edges of this KoreTriangle.
    /// </summary>
    public KoreEdge[] GetEdges()
    {
        return new[]
        {
            new KoreEdge(A, B),
            new KoreEdge(B, C),
            new KoreEdge(C, A)
        };
    }

    public bool Equals(KoreTriangle other) => A == other.A && B == other.B && C == other.C;
    public override bool Equals(object? obj) => obj is KoreTriangle t && Equals(t);
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = A;
            hash = (hash * 397) ^ B;
            hash = (hash * 397) ^ C;
            return hash;
        }
    }

    public static bool operator ==(KoreTriangle left, KoreTriangle right) => left.Equals(right);
    public static bool operator !=(KoreTriangle left, KoreTriangle right) => !left.Equals(right);

    public override string ToString() => $"KoreTriangle({A}, {B}, {C})";


    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Returns a triangle with the same three indices, but
    /// whose vertex order has been flipped iff its current
    /// orientation in world‐space doesn’t match the requested winding.
    /// </summary>
    /// <param name="tri">The original triangle (A,B,C).</param>
    /// <param name="mesh">Mesh used to look up 2D positions.</param>
    /// <param name="clockwise">
    /// If true, ensures the returned triangle is wound clockwise;
    /// if false, ensures counter-clockwise.
    /// </param>
    public static KoreTriangle EnsureWinding(
        KoreTriangle tri,
        KoreMesh mesh,
        bool clockwise)
    {
        // Fetch the three points
        var p1 = mesh.GetPointByIndex(tri.A).mathPos;
        var p2 = mesh.GetPointByIndex(tri.B).mathPos;
        var p3 = mesh.GetPointByIndex(tri.C).mathPos;

        // Compute signed area: >0 = CCW, <0 = CW
        double cross = (p2.X - p1.X) * (p3.Y - p1.Y)
                     - (p2.Y - p1.Y) * (p3.X - p1.X);

        bool isCCW = cross > 0;
        // If we want CW but it’s CCW, or want CCW but it’s CW → flip B/C
        if (clockwise ? isCCW : !isCCW)
        {
            return new KoreTriangle(tri.A, tri.C, tri.B);
        }
        // Otherwise the winding already matches
        return tri;
    }

    // --------------------------------------------------------------------------------------------


    public void CalcTriangle(KoreMesh mesh)
    {
        // Fetch the three points
        var p1 = mesh.GetPointByIndex(A).mathPos;
        var p2 = mesh.GetPointByIndex(B).mathPos;
        var p3 = mesh.GetPointByIndex(C).mathPos;

        _triangle = new FssXYTriangle(p1, p2, p3);
    }


    public double Area()
    {
        return _triangle.Area();
    }

    public bool Contains(FssXYPoint pnt)
    {
        // // Fetch the three points
        // var p1 = mesh.GetPointByIndex(A).mathPos;
        // var p2 = mesh.GetPointByIndex(B).mathPos;
        // var p3 = mesh.GetPointByIndex(C).mathPos;

        // FssXYTriangle triangle = new FssXYTriangle(p1, p2, p3);

        return _triangle.Contains(pnt);
    }

    public FssXYPoint CenterPoint()
    {
        // Fetch the three points
        // var p1 = mesh.GetPointByIndex(A).mathPos;
        // var p2 = mesh.GetPointByIndex(B).mathPos;
        // var p3 = mesh.GetPointByIndex(C).mathPos;

        // FssXYTriangle triangle = new FssXYTriangle(p1, p2, p3);
        return _triangle.CenterPoint();
    }


}



