using ELO.Points;

namespace ELO.PointOperations;

public interface IPointOperations
{
    AffinePoint MultiplyPoint(BigInteger k, AffinePoint p);
}

