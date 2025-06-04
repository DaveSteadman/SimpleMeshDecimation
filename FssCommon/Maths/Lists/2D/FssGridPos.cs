

// Simple struct to hold a grid size, with a 0,0 origin.


public struct FssGridPos
{
    public int X;
    public int Y;

    public FssGridPos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public FssGridPos(FssGridPos other)
    {
        X = other.X;
        Y = other.Y;
    }

    public static FssGridPos PosAbove(FssGridPos pos) => new FssGridPos(pos.X,     pos.Y - 1);
    public static FssGridPos PosBelow(FssGridPos pos) => new FssGridPos(pos.X,     pos.Y + 1);
    public static FssGridPos PosLeft(FssGridPos pos)  => new FssGridPos(pos.X - 1, pos.Y);
    public static FssGridPos PosRight(FssGridPos pos) => new FssGridPos(pos.X + 1, pos.Y);


}

