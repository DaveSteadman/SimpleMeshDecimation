
using SkiaSharp;
using System;
using System.Collections.Generic;

public static partial class KoreMeshOperations
{
    // Loop through each point in the mesh, adding a Line association between adjacent points, as they
    // will be present regardless or other triangulation operations.
    // Usage: KoreMeshOperations.JoinAdjacent(mesh);
    public static void JoinAdjacent(KoreMesh mesh)
    {
        for (int x = 0; x < mesh.NumX; x++)
        {
            for (int y = 0; y < mesh.NumY; y++)
            {
                var currPoint = mesh.points[x, y];

                if (!currPoint.IsInUse()) continue;

                int x1 = x + 1;
                int y1 = y + 1;
                int y2 = y - 1;

                bool x1Valid = x1 < mesh.NumX;
                bool y1Valid = y1 < mesh.NumY;
                bool y2Valid = y2 >= 0;

                // Upper-right
                // if (x1Valid && y2Valid && mesh.points[x1, y2].IsInUse())
                //     mesh.AddLine(x, y, x1, y2);

                // Right
                if (x1Valid && mesh.points[x1, y].IsInUse())
                    mesh.AddLine(x, y, x1, y);

                // Lower-right
                if (x1Valid && y1Valid && mesh.points[x1, y1].IsInUse())
                    mesh.AddLine(x, y, x1, y1);

                // Lower
                if (y1Valid && mesh.points[x, y1].IsInUse())
                    mesh.AddLine(x, y, x, y1);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Walk the boundary points, adding a line association between next-valid points.
    // The corners are guaranteed.

    public static void JoinBoundary(KoreMesh mesh)
    {
        // Walk the top and bottom rows
        foreach (int curry in new List<int> { 0, mesh.NumY - 1 })
        {
            int currValidX = -1;

            for (int x = 0; x < mesh.NumX; x++)
            {
                var point = mesh.points[x, curry];

                if (point.IsInUse())
                {
                    if (currValidX >= 0 && (x - currValidX > 1))
                    {
                        // Add line between spaced valid points
                        mesh.AddLine(currValidX, curry, x, curry);
                    }

                    currValidX = x;
                }
            }
        }

        // Walk the left and right columns
        foreach (int currx in new List<int> { 0, mesh.NumX - 1 })
        {
            int currValidY = -1;

            for (int y = 0; y < mesh.NumY; y++)
            {
                var point = mesh.points[currx, y];

                if (point.IsInUse())
                {
                    if (currValidY >= 0 && (y - currValidY > 1))
                    {
                        // Add line between spaced valid points
                        mesh.AddLine(currx, currValidY, currx, y);
                    }

                    currValidY = y;
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------



    // Returns true if the three integer grid points are not colinear (i.e. form a non‐degenerate triangle).
    // Coordinates are small (0–500), so int arithmetic will never overflow here.

    public static bool IsTriangle(int x1, int y1,
                                int x2, int y2,
                                int x3, int y3)
    {
        int dx21 = x2 - x1;
        int dy21 = y2 - y1;
        int dx31 = x3 - x1;
        int dy31 = y3 - y1;

        // Equivalent to checking area≠0:
        //  (x2−x1)*(y3−y1) − (y2−y1)*(x3−x1) ≠ 0
        return dx21 * dy31 != dy21 * dx31;
    }


    public static bool IsInCircumcircle(
        int ax, int ay,
        int bx, int by,
        int cx, int cy,
        int px, int py)
    {
        // Translate so P is at the origin
        long dxA = ax - px, dyA = ay - py;
        long dxB = bx - px, dyB = by - py;
        long dxC = cx - px, dyC = cy - py;

        // Squared radii
        long a2 = dxA * dxA + dyA * dyA;
        long b2 = dxB * dxB + dyB * dyB;
        long c2 = dxC * dxC + dyC * dyC;

        // 3×3 determinant:
        // | dxA  dyA  a2 |
        // | dxB  dyB  b2 |   > 0  ⇒  P is inside the circumcircle
        // | dxC  dyC  c2 |
        long det = dxA * (dyB * c2 - b2 * dyC)
                - dyA * (dxB * c2 - b2 * dxC)
                + a2 * (dxB * dyC - dyB * dxC);

        return det > 0;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    public static List<KoreEdge> CreateEdgesFromActivePoints(List<KoreMeshPoint> points, KoreMesh mesh)
    {
        List<KoreEdge> edges = new List<KoreEdge>();

        // Simplify our loop, create a new list from only the active points.
        List<KoreMeshPoint> activePoints = new List<KoreMeshPoint>();
        foreach (KoreMeshPoint point in points)
        {
            if (point.IsInUse())
                activePoints.Add(point);
        }

        // Now we look to create an edge between each pair of active points. From the current loop index, to the next indexes in the list.
        // Note a full nested loop creates duplicates, so we only loop from the current index to the next indexes in the list.
        for (int i = 0; i < activePoints.Count; i++)
        {
            KoreMeshPoint pointA = activePoints[i];

            for (int j = i + 1; j < activePoints.Count; j++)
            {
                KoreMeshPoint pointB = activePoints[j];

                // Create an edge between pointA and pointB
                var newEdge = new KoreEdge(pointA.index, pointB.index);
                newEdge.CalcLine(mesh); // Calculate edge geometry
                edges.Add(newEdge);
            }
        }

        return edges;
    }

    // Overview: Loops through the input list of edges (previously sorted into priority order), and returns
    // a new list of edges that do not intersect with earlier ones.
    // - Relies on the Edge.Intersect for the heavy-lifting.
    public static List<KoreEdge> ShortestNonIntersectingLines(List<KoreEdge> activeEdges)
    {
        // setup the result list
        List<KoreEdge> resultEdges = new List<KoreEdge>();

        // Loop through each edge in the active edges
        foreach (KoreEdge edge in activeEdges)
        {
            // Check if the edge intersects with any of the edges already in the result list
            bool intersects = false;
            foreach (KoreEdge resultEdge in resultEdges)
            {
                if (edge.Intersects(resultEdge))
                {
                    intersects = true;
                    break;
                }
            }

            // If it does not intersect with any prior edge, add it to the result list
            if (!intersects)
            {
                resultEdges.Add(edge);
            }
        }

        // Return the list of non-intersecting edges
        return resultEdges;
    }

    // --------------------------------------------------------------------------------------------

    // Overview: Creates a list of triangles from the provided edges and mesh.

    public static List<KoreTriangle> CreateTrianglesFromEdges(List<KoreEdge> edges, KoreMesh mesh)
    {
        var triangles = new HashSet<KoreTriangle>();  // Use hashset to avoid duplicates
        var edgeSet = new HashSet<(int, int)>();

        // Create a lookup to test for edge existence
        foreach (var edge in edges)
        {
            // Always store smaller index first to normalize undirected edges
            var key = edge.P1 < edge.P2 ? (edge.P1, edge.P2) : (edge.P2, edge.P1);
            edgeSet.Add(key);
        }

        // Try every combination of 3 points
        for (int i = 0; i < mesh.PointCount; i++)
        {
            for (int j = i + 1; j < mesh.PointCount; j++)
            {
                for (int k = j + 1; k < mesh.PointCount; k++)
                {
                    var e1 = (Math.Min(i, j), Math.Max(i, j));
                    var e2 = (Math.Min(j, k), Math.Max(j, k));
                    var e3 = (Math.Min(k, i), Math.Max(k, i));

                    if (edgeSet.Contains(e1) && edgeSet.Contains(e2) && edgeSet.Contains(e3))
                    {
                        triangles.Add(new KoreTriangle(i, j, k));
                    }
                }
            }
        }

        return triangles.ToList();
    }


    public static List<KoreTriangle> CreateTrianglesFromEdges2(
        List<KoreEdge> edges,
        KoreMesh mesh)
    {
        // 1) Build a neighbor map: vertex → all adjacent vertices
        var adj = new Dictionary<int, HashSet<int>>();
        foreach (var e in edges)
        {
            if (!adj.TryGetValue(e.P1, out var n1)) adj[e.P1] = n1 = new HashSet<int>();
            if (!adj.TryGetValue(e.P2, out var n2)) adj[e.P2] = n2 = new HashSet<int>();
            n1.Add(e.P2);
            n2.Add(e.P1);
        }

        // 2) Enumerate 3‐cliques (u<v<w) in that adjacency
        var tris = new List<KoreTriangle>();
        foreach (var u in adj.Keys)
        {
            // Only look at neighbors > u to avoid duplicates
            var nu = adj[u].Where(v => v > u).ToList();
            for (int i = 0; i < nu.Count; i++)
            {
                for (int j = i + 1; j < nu.Count; j++)
                {
                    int v = nu[i], w = nu[j];
                    // if v and w are connected, (u,v,w) is a triangle
                    if (adj[v].Contains(w))
                    {
                        var newTri = new KoreTriangle(u, v, w);
                        newTri.CalcTriangle(mesh); // Calculate triangle geometry
                        tris.Add(newTri);
                    }
                }
            }
        }

        return tris;
    }

    public static List<KoreTriangle> CreateTrianglesFromEdges3(List<KoreEdge> edges, KoreMesh mesh)
    {
        List<KoreTriangle> triangles = new List<KoreTriangle>();

        // start with a loop through each edge
        foreach (var edge in edges)
        {
            int u = edge.P1;
            int v = edge.P2;

            // Loop thrugh all edges, looping for a V W
            foreach (var edge2 in edges)
            {
                // We only want edges that start with v
                if (edge2.P1 != v)
                    continue;

                // This is the second point in the edge2
                int w = edge2.P2;

                // Ensure we only consider edges in increasing order (u < v < w)
                if (w <= v)
                    continue;

                // Check if we have an edge W U - an implicit loop
                if (!edges.Any(e3 => e3.P1 == u && e3.P2 == w))
                    continue; // If no edge W U, skip to next edge2

                // If we reach here, we have a valid triangle (u, v, w)
                var newTri = new KoreTriangle(u, v, w);
                newTri.CalcTriangle(mesh); // Calculate triangle geometry
                triangles.Add(newTri);
            }
        }
        return triangles;
    }




    // return a list of triangles, filtering out triangles that overlap others.
    // We order triangles by area, smallest first.
    // We add each triangle to the result list if it does not overlap any existing result-triangle.
    // Usage: KoreMeshOperations.FilterOutEncompasingTriangles(triangles, mesh);
    public static List<KoreTriangle> FilterOutEncompasingTriangles(List<KoreTriangle> triangles)
    {
        // Sort triangles by area (smallest first)
        var sortedTriangles = triangles.OrderBy(t => t.Area()).ToList();

        var result = new List<KoreTriangle>();

        foreach (var tri in sortedTriangles)
        {
            // check if the centrepoint of the triangle is inside any existing triangle in the result list
            if (!result.Any(r => r.Contains(tri.CenterPoint())))
            {
                result.Add(tri);
            }
        }

        return result;
    }

    public static List<KoreTriangle> FilterOutEncompassingTriangles2(List<KoreTriangle> triangles)
    {
        // 1) Sort by area ascending (keep small first)
        var sorted = triangles.OrderBy(t => t.Area()).ToList();
        var result = new List<KoreTriangle>();

        foreach (var T in sorted)
        {
            bool overlapsSmaller = false;

            // only need to test against triangles we've already kept (all smaller or equal area)
            foreach (var R in result)
            {
                // if (R.OverlapsAABB(T))
                // {
                //     overlapsSmaller = true;
                //     break;
                // }

                // a) If T contains R’s centroid, it fully or partially covers R
                if (T.Contains(R.CenterPoint()))
                {
                    overlapsSmaller = true;
                    break;
                }
            }

            // if it would swallow any smaller triangle, we skip T
            if (!overlapsSmaller)
                result.Add(T);
        }

        return result;
    }

}



