// simple class to hold a subgrid with a FssGridSize, so we have a potentially non 0,0 top left,
// and a bottom right up to the size of the parent grid.
// We're using index values, so we start from 0's and incrememt to num-entries -1 on each axis.

public class FssSubGridSize
{
    public int StartX;
    public int StartY;
    public int EndX;
    public int EndY;

    public FssSubGridSize(int startX, int startY, int endX, int endY)
    {
        StartX = startX;
        StartY = startY;
        EndX = endX;
        EndY = endY;
    }

    public FssGridPos TL => new FssGridPos(StartX, StartY);
    public FssGridPos BR => new FssGridPos(EndX, EndY);
    public FssGridPos TR => new FssGridPos(EndX, StartY);
    public FssGridPos BL => new FssGridPos(StartX, EndY);

    public int SizeX => EndX - StartX + 1;
    public int SizeY => EndY - StartY + 1;
    public int Size => SizeX * SizeY;

}