
using System;

// IFssXY: Base class for all 2D coordinate types, allowing for a common interface for all 2D coordinate types and a base type for collections.

public interface IFssXY
{
    public abstract FssXYRect? AABB();
    public abstract bool Contains(FssXYPoint xy);

}