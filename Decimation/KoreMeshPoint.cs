


using SkiaSharp;

public class KoreMeshPoint
{
    public int index; // Unique ID in larger mesh (used for triangle indexs)

    public FssXYPoint mathPos = FssXYPoint.Zero; // Maths position
    public SKPoint    drawPos; // drawing position

    public bool inUse;       // Is this point in use? (i.e. not decimated)
    public bool anchorPoint; // Is this point an anchor point? (i.e. setup and always in use for triangulation maths)

    public bool IsInUse()
    {
        return ( inUse || anchorPoint );
    }
}



