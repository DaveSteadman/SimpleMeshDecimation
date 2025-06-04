using System;
using System.Collections.Generic;

#nullable enable

public static class FssXYPolygonOperations
{
    // --------------------------------------------------------------------------------------------
    // #MARK: ClipLineToPolygon
    // --------------------------------------------------------------------------------------------

    public static FssXYLine? ClipLineToPolygon(this FssXYLine line, FssXYPolygon polygon)
    {
        FssXYLine? clippedLine = null;
        FssXYPoint? p1 = null;
        FssXYPoint? p2 = null;

        // // Iterate over each edge of the polygon
        // for (int i = 0; i < polygon.Vertices.Count; i++)
        // {
        //     // Find intersection between line and polygon edge
        //     FssXYPoint? intersection = FssXYLineOperations.Intersection(line, new FssXYLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.Vertices.Count]));

        //     if (intersection != null)
        //     {
        //         if (p1 == null)
        //         {
        //             p1 = intersection;
        //         }
        //         else if (p2 == null)
        //         {
        //             p2 = intersection;
        //         }
        //         else
        //         {
        //             // Determine which intersection points to keep based on distance
        //             if (p1.DistanceTo(line.P1) > p2.DistanceTo(line.P1))
        //             {
        //                 p1 = p2;
        //                 p2 = intersection;
        //             }
        //             else
        //             {
        //                 p2 = intersection;
        //             }
        //         }
        //     }
        // }

        // // Create the clipped line if there are two intersection points
        // if (p1 != null && p2 != null)
        // {
        //     clippedLine = new FssXYLine(p1, p2);
        // }

        return clippedLine;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: ClipPolygonToRect and helpers
    // --------------------------------------------------------------------------------------------

    // Clipping a polygon to a rectangle using the Sutherland-Hodgman algorithm
    public static List<FssXYPoint> ClipPolygonToRect(List<FssXYPoint> polygon, FssXYRect rect)
    {
        List<FssXYPoint> output = new List<FssXYPoint>(polygon);

        // Clip polygon to each edge of the rectangle
        output = ClipEdgeToRect(output, rect.Left,  rect.Top,    rect.Right, rect.Top);    // Top edge
        output = ClipEdgeToRect(output, rect.Right, rect.Top,    rect.Right, rect.Bottom); // Right edge
        output = ClipEdgeToRect(output, rect.Right, rect.Bottom, rect.Left,  rect.Bottom); // Bottom edge
        output = ClipEdgeToRect(output, rect.Left,  rect.Bottom, rect.Left,  rect.Top);    // Left edge

        return output;
    }

    // Clip polygon edges to a specified rectangle edge
    private static List<FssXYPoint> ClipEdgeToRect(List<FssXYPoint> polygon, double x1, double y1, double x2, double y2)
    {
        List<FssXYPoint> output = new List<FssXYPoint>();
        int count = polygon.Count;

        for (int i = 0; i < count; i++)
        {
            FssXYPoint current = polygon[i];
            FssXYPoint previous = polygon[(i - 1 + count) % count];

            // Determine if current and previous points are inside the clipping boundary
            bool currentInside = IsInside(current, x1, y1, x2, y2);
            bool previousInside = IsInside(previous, x1, y1, x2, y2);

            if (currentInside)
            {
                // If the current point is inside and the previous point is outside, add the intersection point
                if (!previousInside)
                {
                    FssXYPoint? intersection = ComputeIntersection(previous, current, x1, y1, x2, y2);
                    if (intersection != null) output.Add(intersection);
                }
                output.Add(current); // Add the current point if it's inside
            }
            else if (previousInside)
            {
                // If the current point is outside and the previous point is inside, add the intersection point
                FssXYPoint? intersection = ComputeIntersection(previous, current, x1, y1, x2, y2);
                if (intersection != null) output.Add(intersection);
            }
        }

        return output;
    }

    // Check if a point is inside a given boundary
    private static bool IsInside(FssXYPoint point, double x1, double y1, double x2, double y2)
    {
        // Define edge based on coordinates and check if point is inside relative to the edge
        if (x1 == x2) // Vertical edge
        {
            return (x2 >= x1) ? point.X <= x2 : point.X >= x2;
        }
        else // Horizontal edge
        {
            return (y2 >= y1) ? point.Y <= y2 : point.Y >= y2;
        }
    }

    // Compute the intersection point of a line segment with a clipping boundary
    private static FssXYPoint? ComputeIntersection(FssXYPoint p1, FssXYPoint p2, double x1, double y1, double x2, double y2)
    {
        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;

        double denominator = (x1 - x2) * dy - (y1 - y2) * dx;

        if (denominator == 0)
        {
            return null; // Parallel lines
        }

        double ua = ((x1 - p1.X) * dy - (y1 - p1.Y) * dx) / denominator;

        return new FssXYPoint(
            p1.X + ua * dx,
            p1.Y + ua * dy
        );
    }
}
