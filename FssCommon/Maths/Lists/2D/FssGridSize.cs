

// Simple struct to hold a grid size, with a 0,0 origin.


public struct FssGridSize
{
    public int NumX;
    public int NumY;

    public FssGridSize(int numX, int numY)
    {
        NumX = numX;
        NumY = numY;
    }

    public FssGridSize(FssGridSize other)
    {
        NumX = other.NumX;
        NumY = other.NumY;
    }

    public static FssGridSize operator +(FssGridSize a, FssGridSize b)
    {
        return new FssGridSize(a.NumX + b.NumX, a.NumY + b.NumY);
    }


    public bool Contains(FssGridPos pos)
    {
        return (pos.X >= 0 && pos.X < NumX && pos.Y >= 0 && pos.Y < NumY);
    }
}

