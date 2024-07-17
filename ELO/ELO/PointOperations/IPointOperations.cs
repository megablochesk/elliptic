using ELO.Points;

namespace ELO.PointOperations;

public interface IPointOperations
{
    Point AddPoints(Point p1, Point p2);
    Point DoublePoint(Point p);
}