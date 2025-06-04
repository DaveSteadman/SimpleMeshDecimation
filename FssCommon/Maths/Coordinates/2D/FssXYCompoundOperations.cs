using System;
using System.Collections.Generic;

/*
FssXYCompound: A shape that is made up of multiple IFssXY shapes, such as a polygon or a multi-line.

Design Decisions:
- The object is immutable
*/

public static class FssXYCompoundOperations
{
    public static FssXYCompound AppendShape(FssXYCompound compound, IFssXY shape)
    {
        List<IFssXY> newShapes = new List<IFssXY>();
        foreach (IFssXY currshape in compound.Shapes)
            newShapes.Add(currshape);

        newShapes.Add(shape);

        return new FssXYCompound(newShapes);
    }

}