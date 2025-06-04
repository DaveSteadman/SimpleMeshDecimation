using System;
using System.Collections.Generic;

/*
FssXYCompound: A shape that is made up of multiple IFssXY shapes, such as a polygon or a multi-line.

Design Decisions:
- The object is immutable
*/

public class FssXYCompound : IFssXY
{
    public List<IFssXY> Shapes { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYCompound()
    {
        Shapes = new List<IFssXY>();
    }


    public FssXYCompound(List<IFssXY> shapes)
    {
        Shapes = shapes;
    }

    public FssXYCompound(FssXYCompound compound)
    {
        Shapes = new List<IFssXY>();
        foreach (IFssXY shape in compound.Shapes)
            Shapes.Add(shape);
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Look at each shape in the compound and offset it by the given amount
    // Design Descisions:
    // - Not wanting to override a common Offset function, and start encumbering the class hierarchy with
    //   a forced set of functions. So accepting the burden of type checking in the compound shape where it will be used.

    public FssXYCompound Offset(double x, double y)
    {
        List<IFssXY> newShapes = new List<IFssXY>();
        foreach (IFssXY shape in Shapes)
        {
            // determine the type of each shape, and then call the appropriate offset method
            if      (shape is FssXYPoint)    { newShapes.Add(((FssXYPoint)shape).Offset(x, y)); }
            else if (shape is FssXYLine)     { newShapes.Add(((FssXYLine)shape).Offset(x, y)); }
            else if (shape is FssXYCircle)   { newShapes.Add(((FssXYCircle)shape).Offset(x, y)); }
            else if (shape is FssXYCompound) { newShapes.Add(((FssXYCompound)shape).Offset(x, y)); }
        }
        return new FssXYCompound(newShapes);
    }

    public void AppendShape(IFssXY shape)
    {
        Shapes.Add(shape);
    }


    public FssXYRect? AABB()
    {
        // loop through all of the objects, requesting their AABB rect. Even if the subshape
        // is a compound, it will return a rect that contains all of the subshapes.

        List<FssXYRect> aabbList = new List<FssXYRect>();

        foreach (IFssXY shape in Shapes)
        {
            FssXYRect? subAABB = shape.AABB();

            if (subAABB == null)
                continue;
            aabbList.Add(subAABB);

        }

        return FssXYRectOperations.AABBFromList(aabbList);
    }


    public bool Contains(FssXYPoint xy)
    {
        foreach (IFssXY currShape in Shapes)
        {
            if (currShape.Contains(xy))
                return true;
        }
        return false;
    }

}