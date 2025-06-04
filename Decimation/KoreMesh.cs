


using SkiaSharp;
using System;
using System.Collections.Generic;

public class KoreMesh
{
    public KoreMeshPoint[,] points;
    public int NumX;
    public int NumY;

    public List<(int, int, int, int)> lines = new List<(int, int, int, int)>();

    public int PointCount => NumX * NumY;

    // --------------------------------------------------------------------------------------------

    public KoreMesh(int numX, int numY)
    {
        NumX = numX;
        NumY = numY;
        points = CreatePoints(numX, numY);
    }

    private KoreMeshPoint[,] CreatePoints(int numX, int numY)
    {
        KoreMeshPoint[,] points = new KoreMeshPoint[numX, numY];

        int pntIndex = 0;
        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                var pnt = new KoreMeshPoint
                {
                    index = pntIndex,
                    mathPos = new FssXYPoint(x, y),
                    drawPos = new SKPoint(0, 0),
                    inUse = true,
                    anchorPoint = false
                };
                points[x, y] = pnt;
                pntIndex++;
            }
        }
        return points;
    }

    public KoreMeshPoint[,] GetPoints()
    {
        return points;
    }

    public List<KoreMeshPoint> GetPointsAsList()
    {
        List<KoreMeshPoint> pointList = new List<KoreMeshPoint>();
        for (int x = 0; x < NumX; x++)
        {
            for (int y = 0; y < NumY; y++)
            {
                pointList.Add(points[x, y]);
            }
        }
        return pointList;
    }

    public List<KoreMeshPoint> GetActivePointsAsList()
    {
        List<KoreMeshPoint> activePoints = new List<KoreMeshPoint>();
        for (int x = 0; x < NumX; x++)
        {
            for (int y = 0; y < NumY; y++)
            {
                if (points[x, y].IsInUse())
                {
                    activePoints.Add(points[x, y]);
                }
            }
        }
        return activePoints;
    }

    public KoreMeshPoint GetPoint(int x, int y)
    {
        if (x < 0 || x >= NumX || y < 0 || y >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
        }
        return points[x, y];
    }

    public int GetPointIndex(int x, int y)
    {
        if (x < 0 || x >= NumX || y < 0 || y >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
        }
        return (x * NumY) + y;
    }

    public KoreMeshPoint GetPointByIndex(int index)
    {
        if (index < 0 || index >= NumX * NumY)
            throw new ArgumentOutOfRangeException("Index is out of range.");

        int x = index / NumY;
        int y = index % NumY;

        return points[x, y];
    }

    // [] operator for point access

    public KoreMeshPoint this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= NumX || y < 0 || y >= NumY)
            {
                throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
            }
            return points[x, y];
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Draw Pos
    // --------------------------------------------------------------------------------------------

    public void SetupDrawPos(float spacing, float border)
    {
        for (int x = 0; x < NumX; x++)
        {
            for (int y = 0; y < NumY; y++)
            {
                var pnt = points[x, y];
                pnt.drawPos.X = (float)((pnt.mathPos.X * spacing) + border);
                pnt.drawPos.Y = (float)((pnt.mathPos.Y * spacing) + border);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Index
    // --------------------------------------------------------------------------------------------

    public void SetActiveByIndex(int index, bool active)
    {
        if (index < 0 || index >= NumX * NumY)
        {
            throw new ArgumentOutOfRangeException("Index is out of range.");
        }

        int x = index % NumX;
        int y = index / NumX;

        points[x, y].inUse = active;
    }

    public void SetActiveByPos(int x, int y, bool active)
    {
        if (x < 0 || x >= NumX || y < 0 || y >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
        }
        points[x, y].inUse = active;
    }

    public List<(int, int, int, int)> GetLines()
    {
        return lines;
    }

    public void AddLine(int x1, int y1, int x2, int y2)
    {
        lines.Add((x1, y1, x2, y2));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Subgrid
    // --------------------------------------------------------------------------------------------

    // Get the subgrids based on a set "window" size.
    // Note that adjacent windows overlap on their edge points, as they will be used for triangulation on each side.

    public List<FssSubGridSize> GetSubGridsByStep(int widthCellSize, int heightCellSize)
    {
        List<FssSubGridSize> subGrids = new List<FssSubGridSize>();

        // Create the lists of X and Y array index values based on the specified cell size, so our loop is just going through them
        FssInt1DArray xrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumX - 1, widthCellSize);
        FssInt1DArray yrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumY - 1, heightCellSize);

        for (int x = 0; x < xrange.Length - 1; x++)
        {
            for (int y = 0; y < yrange.Length - 1; y++)
            {
                int x1 = xrange[x];
                int y1 = yrange[y];
                int x2 = Math.Min(xrange[x + 1], NumX - 1);
                int y2 = Math.Min(yrange[y + 1], NumY - 1);

                // Create a subgrid size object and add it to the list
                FssSubGridSize subGrid = new FssSubGridSize(x1, y1, x2, y2);
                subGrids.Add(subGrid);
            }
        }

        return subGrids;
    }

    public List<KoreMeshPoint> GetPointsInSubGrid(FssSubGridSize gridSize)
    {
        List<KoreMeshPoint> subPoints = new List<KoreMeshPoint>();

        for (int x = gridSize.TL.X; x <= gridSize.BR.X; x++)
        {
            for (int y = gridSize.TL.Y; y <= gridSize.BR.Y; y++)
            {
                if (x >= 0 && x < NumX && y >= 0 && y < NumY)
                {
                    subPoints.Add(points[x, y]);
                }
            }
        }

        return subPoints;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Anchor
    // --------------------------------------------------------------------------------------------

    public void SetAnchorPoint(FssGridPos pos, bool anchor)
    {
        if (pos.X < 0 || pos.X >= NumX || pos.Y < 0 || pos.Y >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
        }
        points[pos.X, pos.Y].anchorPoint = anchor;
    }

    public void SetPointAnchor(int x, int y, bool anchor)
    {
        if (x < 0 || x >= NumX || y < 0 || y >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
        }
        points[x, y].anchorPoint = anchor;
    }

    public void SetCornerAnchors(FssSubGridSize gridSize)
    {
        SetAnchorPoint(gridSize.TL, true);
        SetAnchorPoint(gridSize.TR, true);
        SetAnchorPoint(gridSize.BL, true);
        SetAnchorPoint(gridSize.BR, true);
    }

    // creates two 1D array of best-fit increments for the X and Y indexes. Loops through the grid setting
    // anchorpoints to help the triangulation process avoid too many skinny triangles on teh edges.
    public void SetPointAnchorByStep(int x, int y)
    {
        if (x < 0 || x >= NumX || y < 0 || y >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point coordinates are out of range.");
        }

        // Create the lists of X and Y array index values based on the specified cell size, so our loop is just going through them
        FssInt1DArray xrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumX - 1, x);
        FssInt1DArray yrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumY - 1, y);

        for (int i = 0; i < xrange.Length; i++)
        {
            for (int j = 0; j < yrange.Length; j++)
            {
                SetPointAnchor(xrange[i], yrange[j], true);
            }
        }
    }

    // Set interior points as anchors by stepping through the grid.
    public void SetInteriorPointAnchorByStep(int xStep, int yStep)
    {
        if (xStep < 0 || xStep >= NumX || yStep < 0 || yStep >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point step size is out of range.");
        }

        // Create the lists of X and Y array index values based on the specified cell size, so our loop is just going through them
        FssInt1DArray xrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumX - 1, xStep);
        FssInt1DArray yrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumY - 1, yStep);

        for (int i = 1; i < xrange.Length - 1; i++)
        {
            for (int j = 1; j < yrange.Length - 1; j++)
            {
                SetPointAnchor(xrange[i], yrange[j], true);
            }
        }
    }

    public void SetBoundaryPointAnchorByStep(int xStep, int yStep)
    {
        if (xStep < 0 || xStep >= NumX || yStep < 0 || yStep >= NumY)
        {
            throw new ArgumentOutOfRangeException("Point steps are out of range.");
        }

        // Create the lists of X and Y array index values based on the specified cell size, so our loop is just going through them
        FssInt1DArray xrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumX - 1, xStep);
        FssInt1DArray yrange = FssInt1DArrayOperations.CreateRangeByStep(0, NumY - 1, yStep);

        for (int i = 0; i < xrange.Length; i++)
        {
            SetPointAnchor(xrange[i], 0, true); // Top boundary
            SetPointAnchor(xrange[i], NumY - 1, true); // Bottom boundary
        }

        for (int j = 0; j < yrange.Length; j++)
        {
            SetPointAnchor(0, yrange[j], true); // Left boundary
            SetPointAnchor(NumX - 1, yrange[j], true); // Right boundary
        }
    }


}



