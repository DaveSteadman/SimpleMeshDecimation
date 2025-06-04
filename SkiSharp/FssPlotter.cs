using SkiaSharp;
using System;
using System.IO;
using System.Collections.Generic;


// Contains the paint settings and provides some level of abstraction in case we ever use a different plotter to SkiaSharp

public class FssPlotterDrawSettings
{
    public SKPaint Paint { get; } = new SKPaint(); // This is a cache of the last paint object created, so we can reuse it if the settings haven't changed
    public float LineWidth
    {
        get { return Paint.StrokeWidth; }
        set { Paint.StrokeWidth = value; }
    }

    public SKColor Color
    {
        get { return Paint.Color; }
        set { Paint.Color = value; }
    }

    public bool IsAntialias
    {
        get { return Paint.IsAntialias; }
        set { Paint.IsAntialias = value; }
    }

    public float LineSpacing { get; set; } = 0;
    public float PointCrossSize { get; set; } = 3;

    public FssPlotterDrawSettings()
    {
        ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        try
        {
            Paint.StrokeWidth = 1;
            Paint.Color = SKColors.Black;
            Paint.Style = SKPaintStyle.Stroke;
            Paint.IsAntialias = true;

            LineSpacing = 0;
            PointCrossSize = 3;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting default paint settings: {ex.Message}");
        }
    }

    // Create a new object configured to fill shapes

    public static FssPlotterDrawSettings FillSettings()
    {
        FssPlotterDrawSettings settings = new ();
        settings.Paint.Style = SKPaintStyle.Fill;
        settings.Paint.StrokeWidth = 0;

        return settings;
    }
}

public class FssPlotter
{
    private SKBitmap canvasBitmap;
    private SKCanvas canvas;
    public FssPlotterDrawSettings DrawSettings = new ();

    public SKPaint Paint = new ();

    // --------------------------------------------------------------------------------------------
    // Constructors and initialization
    // --------------------------------------------------------------------------------------------

    public FssPlotter(int canvasWidth, int canvasHeight)
    {
        this.canvasBitmap = new SKBitmap(canvasWidth, canvasHeight);
        this.canvas       = new SKCanvas(canvasBitmap);

        Paint.StrokeWidth = 1;
        Paint.IsAntialias = true;
        Paint.Color       = SKColors.Black;
        Paint.Style       = SKPaintStyle.Stroke;

        Clear(SKColors.White);
    }

    public void Clear(SKColor color)
    {
        canvas.Clear(color);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Point
    // --------------------------------------------------------------------------------------------

    public void DrawPointAsCross(SKPoint p, int crosssize = 3)
    {
        DrawSettings.Paint.Style = SKPaintStyle.Stroke;

        canvas.DrawLine(p.X - crosssize, p.Y - crosssize, p.X + crosssize, p.Y + crosssize, DrawSettings.Paint);
        canvas.DrawLine(p.X - crosssize, p.Y + crosssize, p.X + crosssize, p.Y - crosssize, DrawSettings.Paint);
    }

    public void DrawPointAsCircle(SKPoint p, int circleRadius = 3)
    {
        DrawSettings.Paint.Style = SKPaintStyle.Stroke;

        canvas.DrawCircle(p, circleRadius, DrawSettings.Paint);
    }

    public void DrawPointAsSquare(SKPoint p, int squareSize = 3)
    {
        DrawSettings.Paint.Style = SKPaintStyle.Stroke;

        SKRect rect = new SKRect(p.X - squareSize, p.Y - squareSize, p.X + squareSize, p.Y + squareSize);
        canvas.DrawRect(rect, DrawSettings.Paint);
    }

    public void DrawPointAsFilledCircle(SKPoint p, float circleRadius = 3)
    {
        using (var fillPaint = new SKPaint
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        })
        {
            canvas.DrawCircle(p, circleRadius, fillPaint);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK:
    // --------------------------------------------------------------------------------------------

    public void DrawKorePos(KoreMeshPoint point, float pointSize = 3)
    {
        SKPoint pnt = point.drawPos;

        float circlePadSize = 7;

        if (point.anchorPoint)
        {
            DrawSettings.Paint.Color = SKColors.Red;
            DrawPointAsFilledCircle(pnt, circlePadSize);
            DrawPointAsSquare(pnt);
        }
        else
        {
            if (point.inUse)
            {
                DrawSettings.Paint.Color = SKColors.Green;
                DrawPointAsFilledCircle(pnt, circlePadSize);
                DrawPointAsCircle(pnt);
            }
            else
            {
                DrawSettings.Paint.Color = SKColors.LightGray;
                DrawPointAsCross(pnt);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Line
    // --------------------------------------------------------------------------------------------

    public void DrawLine(SKPoint p1, SKPoint p2)
    {
        canvas.DrawLine(p1, p2, DrawSettings.Paint);
    }

    // DRaw a line, with the start and end points moved towards each other a defined amount, as to not overdraw the end points
    public void DrawLineInset(SKPoint p1, SKPoint p2, float inset)
    {
        // determine the direction of the line
        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length == 0) return;

        // normalize the direction
        dx /= length;
        dy /= length;

        // draw the inset line
        SKPoint insetStart = new SKPoint(p1.X + dx * inset, p1.Y + dy * inset);
        SKPoint insetEnd   = new SKPoint(p2.X - dx * inset, p2.Y - dy * inset);
        canvas.DrawLine(insetStart, insetEnd, DrawSettings.Paint);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Draw Rounded Rect
    // --------------------------------------------------------------------------------------------

    public void DrawRoundedRect(SKRect rect, float radius)
    {
        canvas.DrawRoundRect(rect, radius, radius, DrawSettings.Paint);
    }

    // We have a rectangle, that we want to draw a boundary box around. We supply that padding
    // value that is used to inflect the rect and define the corner radius.
    // Useful of a selection box around a number of points.

    // Usage: p.DrawEncompassingRoundedRect(new SKRect(0, 0, 100, 100), 10);

    public void DrawEncompassingRoundedRect(SKRect rect, float padding)
    {
        SKRect inflatedRect = SKRect.Inflate(rect, padding, padding);
        float radius = padding * 0.5f;
        canvas.DrawRoundRect(inflatedRect, radius, radius, DrawSettings.Paint);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle
    // --------------------------------------------------------------------------------------------

    // Usage:  p.DrawSettings.Paint.Color = SKColors.Red;
    //         p.DrawSettings.Paint.Style = SKPaintStyle.Fill;
    //         p.DrawTriangle(new SKPoint(0, 0), new SKPoint(100, 0), new SKPoint(50, 100));

    public void DrawTriangle(SKPoint p1, SKPoint p2, SKPoint p3)
    {
        using var path = new SKPath();
        path.MoveTo(p1);
        path.LineTo(p2);
        path.LineTo(p3);
        path.Close();

        canvas.DrawPath(path, DrawSettings.Paint);
    }

    public void DrawTriangle(SKPoint p1, SKPoint p2, SKPoint p3, SKPaint paint)
    {
        using var path = new SKPath();
        path.MoveTo(p1);
        path.LineTo(p2);
        path.LineTo(p3);
        path.Close();

        canvas.DrawPath(path, paint);
    }

    // draw a triangle, with each edge (note: edge, not just the points) inset by a small amount.
    public void DrawInsetTriangle(SKPoint p1, SKPoint p2, SKPoint p3, float inset, SKPaint paint)
    {
        // Compute inward-facing offset for an edge (A → B)
        (SKPoint A1, SKPoint B1) OffsetEdge(SKPoint A, SKPoint B, SKPoint C)
        {
            // Edge vector and normal
            var dx = B.X - A.X;
            var dy = B.Y - A.Y;
            var length = MathF.Sqrt(dx * dx + dy * dy);
            if (length == 0) return (A, B); // degenerate

            var nx = -dy / length;
            var ny = dx / length;

            // Ensure normal points inside the triangle (toward point C)
            var mid = new SKPoint((A.X + B.X) * 0.5f, (A.Y + B.Y) * 0.5f);
            var toC = new SKPoint(C.X - mid.X, C.Y - mid.Y);
            var dot = nx * toC.X + ny * toC.Y;
            if (dot < 0)
            {
                nx = -nx;
                ny = -ny;
            }

            var offset = new SKPoint(nx * inset, ny * inset);
            return (new SKPoint(A.X + offset.X, A.Y + offset.Y), new SKPoint(B.X + offset.X, B.Y + offset.Y));
        }

        // Line-line intersection (returns false if lines are parallel)
        bool LineIntersect(SKPoint a1, SKPoint a2, SKPoint b1, SKPoint b2, out SKPoint result)
        {
            float d = (a1.X - a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X - b2.X);
            if (Math.Abs(d) < 1e-6)
            {
                result = default;
                return false;
            }

            float px = ((a1.X * a2.Y - a1.Y * a2.X) * (b1.X - b2.X) - (a1.X - a2.X) * (b1.X * b2.Y - b1.Y * b2.X)) / d;
            float py = ((a1.X * a2.Y - a1.Y * a2.X) * (b1.Y - b2.Y) - (a1.Y - a2.Y) * (b1.X * b2.Y - b1.Y * b2.X)) / d;
            result = new SKPoint(px, py);
            return true;
        }

        // Offset edges inward
        var e1 = OffsetEdge(p1, p2, p3); // opposite of p3
        var e2 = OffsetEdge(p2, p3, p1); // opposite of p1
        var e3 = OffsetEdge(p3, p1, p2); // opposite of p2

        // Find intersection points of offset edges
        if (!LineIntersect(e1.A1, e1.B1, e2.A1, e2.B1, out SKPoint i1)) return;
        if (!LineIntersect(e2.A1, e2.B1, e3.A1, e3.B1, out SKPoint i2)) return;
        if (!LineIntersect(e3.A1, e3.B1, e1.A1, e1.B1, out SKPoint i3)) return;

        // Draw inset triangle
        using var path = new SKPath();
        path.MoveTo(i1);
        path.LineTo(i2);
        path.LineTo(i3);
        path.Close();

        canvas.DrawPath(path, paint);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Save
    // --------------------------------------------------------------------------------------------

    public void Save(string filePath)
    {
        using var image = SKImage.FromBitmap(canvasBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(filePath);
        data.SaveTo(stream);
    }
}

