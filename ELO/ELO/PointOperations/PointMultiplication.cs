using System.Numerics;
using ELO.Points;

namespace ELO.PointOperations;

public class PointMultiplication(PointContext context)
{
    public T MultiplyPoint<T>(BigInteger k, Point p) where T : Point
    {
        if (k == BigInteger.Zero) return (T)p.AtInfinity;

        p.EnsureOnCurve();

        Point result = MultiplyPointLeftToRight(k, p);

        return ConvertPoint<T>(result);
    }

    private Point MultiplyPointLeftToRight(BigInteger k, Point p)
    {
        Point result = p.AtInfinity;

        for (int i = MathUtils.GetHighestBit(k); i >= 0; i--)
        {
            result = context.DoublePoint(result);

            if (MathUtils.IsBitSet(k, i))
            {
                result = context.AddPoints(result, p);
            }
        }

        return result;
    }

    private static T ConvertPoint<T>(Point point) where T : Point
    {
        if (typeof(T) == typeof(AffinePoint) && point is JacobianPoint)
        {
            return ((JacobianPoint)point).ToAffine() as T;
        }

        return (T)point;
    }
}