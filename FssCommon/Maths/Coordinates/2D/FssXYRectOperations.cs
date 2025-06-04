using System;

public enum RectPosition { 
    TopLeft, TopRight, BottomLeft, BottomRight,
    CenterTopMiddle, CenterLeft, Center, CenterRight, CenterBottomMiddle};

public static class FssXYRectOperations
{
    // FssXYRectOperations.AABBFromList()
    // Accomodates zero rectangles which may end up in the list, thats why is not a direct 
    // relationship but an extract of AABBs from the rects along the way.
    public static FssXYRect? AABBFromList(List<FssXYRect> rectList)
    {
        if (rectList.Count == 0)
            return null;

        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double maxX = double.MinValue;
        double maxY = double.MinValue;

        foreach (FssXYRect currRect in rectList)
        {
            FssXYRect? aabb = currRect.AABB();
            if (aabb == null)
                continue;

            FssXYRect rect = (FssXYRect)aabb;

            if (rect.Left < minX)
                minX = rect.Left;
            if (rect.Right > maxX)
                maxX = rect.Right;
            if (rect.Top < minY)
                minY = rect.Top;
            if (rect.Bottom > maxY)
                maxY = rect.Bottom;
        }

        return new FssXYRect(minX, minY, maxX, maxY);
    }

    // Relocate a rectangle around a set position. Height and Width remain the same.
    public static FssXYRect SetAnchoredPosition(FssXYRect rect, RectPosition posID, FssXYPoint pos)
    {
        double x = rect.Left;
        double y = rect.Top;

        switch (posID)
        {
            case RectPosition.TopLeft:
                x = pos.X;
                y = pos.Y;
                break;
            case RectPosition.TopRight:
                x = pos.X - rect.Width;
                y = pos.Y;
                break;
            case RectPosition.BottomLeft:
                x = pos.X;
                y = pos.Y - rect.Height;
                break;
            case RectPosition.BottomRight:
                x = pos.X - rect.Width;
                y = pos.Y - rect.Height;
                break;
            case RectPosition.CenterTopMiddle:
                x = pos.X - rect.Width / 2;
                y = pos.Y;
                break;
            case RectPosition.CenterLeft:
                x = pos.X;
                y = pos.Y - rect.Height / 2;
                break;
            case RectPosition.Center:
                x = pos.X - rect.Width / 2;
                y = pos.Y - rect.Height / 2;
                break;
            case RectPosition.CenterRight:
                x = pos.X - rect.Width;
                y = pos.Y - rect.Height / 2;
                break;
            case RectPosition.CenterBottomMiddle:
                x = pos.X - rect.Width / 2;
                y = pos.Y - rect.Height;
                break;
        }

        return new FssXYRect(x, y, x + rect.Width, y + rect.Height);
    }
}