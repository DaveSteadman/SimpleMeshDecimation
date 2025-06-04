using SkiaSharp;

/*
dotnet new console -n MeshDecimationSandbox
dotnet add package SkiaSharp
*/

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Simple Mesh Decimation Demo");

// ------------------------------------------------------------------------------------------------
// Setup the mesh size and draw constants
int numX      = 60;
int numY      = 40;
int spacing   = 60;
int border    = 20;
int lineInset = 8;
int anchorPointSpacing = 10;

int canvasWidth  = ((numX-1) * spacing) + border * 2;
int canvasHeight = ((numY-1) * spacing) + border * 2;
var p = new FssPlotter(canvasWidth, canvasHeight);

// create the mesh
var meshdata = new KoreMesh(numX, numY);

// Decimate a percentage of points, set the corners as anchors
//KoreMeshOperations.DecimateRandom(meshdata, 0.35f);
KoreMeshOperations.DecimateGradient(meshdata);

// Loop through, and set the draw point for every math point.
meshdata.SetupDrawPos(spacing, border);

// setup defined points as anchors
//meshdata.SetPointAnchorByStep(anchorPointSpacing, anchorPointSpacing);
meshdata.SetInteriorPointAnchorByStep(anchorPointSpacing, anchorPointSpacing);
meshdata.SetBoundaryPointAnchorByStep(anchorPointSpacing / 2, anchorPointSpacing / 2);

// Create the subgrid we're going to deal with for triangulation
List<FssSubGridSize> subGridList = meshdata.GetSubGridsByStep(60, 40);
//List<FssSubGridSize> subGridList = meshdata.GetSubGridsByStep(10, 10);

// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

// Loop through the subgrids, and set the anchor point corners
foreach (FssSubGridSize currsubgrid in subGridList)
{
    meshdata.SetCornerAnchors(currsubgrid);
}

// Draw each point in the mesh
foreach (KoreMeshPoint pnt in meshdata.GetPointsAsList())
    p.DrawKorePos(pnt);

// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

// Loop through the subgrids, and set the anchor point corners
foreach (FssSubGridSize currsubgrid in subGridList)
{
    // get the list of points under consideration for triangulation
    var subPoints = meshdata.GetPointsInSubGrid(currsubgrid);

    // Create a line between each valid point in the subgrid
    List<KoreEdge> edges = KoreMeshOperations.CreateEdgesFromActivePoints(subPoints, meshdata);

    // We know that we've created a set of active points across the mesh, so we can safely discard any length beyond what would be used for triangulation.
    double maxLength = anchorPointSpacing * 1.5; // would actually be root-2, but 1.5 is a good approximation for our quick-cull purposes
    edges.RemoveAll(e => e.Length > maxLength);

    // Draw the candidate edges in light gray
    p.DrawSettings.Paint.Color = SKColors.LightGray;
    foreach (var edge in edges)
    {
        p.DrawLineInset(
            meshdata.GetPointByIndex(edge.P1).drawPos,
            meshdata.GetPointByIndex(edge.P2).drawPos,
            lineInset);
    }

    // Sort edges by length, so we'll consider the shortest edges first
    edges.Sort((a, b) => a.Length.CompareTo(b.Length));
    List<KoreEdge> shortestNonIntersectingEdges = KoreMeshOperations.ShortestNonIntersectingLines(edges);

    // Draw the edges that are not intersecting
    p.DrawSettings.Paint.Color = SKColors.Green;
    foreach (var edge in shortestNonIntersectingEdges)
    {
        p.DrawLineInset(
            meshdata.GetPointByIndex(edge.P1).drawPos,
            meshdata.GetPointByIndex(edge.P2).drawPos,
            lineInset);
    }

    List<KoreTriangle> triangles = KoreMeshOperations.CreateTrianglesFromEdges3(shortestNonIntersectingEdges, meshdata);

    List<KoreTriangle> filteredTriangles = KoreMeshOperations.FilterOutEncompassingTriangles2(triangles);


    // Draw the triangles
    foreach (var triangle in filteredTriangles)
    {
        //Console.WriteLine($"Triangle: {triangle.A}, {triangle.B}, {triangle.C}");

        var tp1 = meshdata.GetPointByIndex(triangle.A).drawPos;
        var tp2 = meshdata.GetPointByIndex(triangle.B).drawPos;
        var tp3 = meshdata.GetPointByIndex(triangle.C).drawPos;

        // Fill
        using (var fillPaint = new SKPaint
        {
            //Color = FssSkiaPalette.RandomFillColor(),
            Color = FssSkiaPalette.RandomPaletteColor(),
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        })
        {
            p.DrawInsetTriangle(tp1, tp2, tp3, 3, fillPaint);
        }
    }

    // Overdraw each active point in the mesh
    foreach (KoreMeshPoint pnt in meshdata.GetPointsAsList())
        if (pnt.IsInUse())
            p.DrawKorePos(pnt);

    Console.WriteLine($"Mesh Stats:");
    Console.WriteLine($"- Subgrid   - {currsubgrid.SizeX} x {currsubgrid.SizeY}, {currsubgrid.Size} points");
    Console.WriteLine($"- Points    - {meshdata.GetPointsAsList().Count} total points, {meshdata.GetActivePointsAsList().Count} active");
    Console.WriteLine($"- Edges     - {edges.Count} total, {shortestNonIntersectingEdges.Count} non-intersecting");
    Console.WriteLine($"- Triangles - {triangles.Count} total, {filteredTriangles.Count} filtered");

}



p.Save("output.png");


