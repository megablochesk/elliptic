using ELO.Points;

namespace ELO.PointOperations;

public class PointContext(IPointOperations pointOperations)
{
    public Point AddPoints(Point p1, Point p2)
    {
        return pointOperations.AddPoints(p1, p2);
    }

    public Point DoublePoint(Point p)
    {
        return pointOperations.DoublePoint(p);
    }
}