using ELO.Points;

namespace ELO.PointOperations;

public interface IPointOperations<TPoint> where TPoint : Point
{
    TPoint AddPoints(TPoint p1, AffinePoint p2);
    TPoint DoublePoint(TPoint p);
    AffinePoint MultiplyPoint(BigInteger k, AffinePoint p);
}

