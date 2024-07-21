using ELO.Points;

namespace ELO.PointOperations;

public class AffineOperations(AlgorithmType algorithmType) : IPointOperations<AffinePoint>
{
    public AffinePoint AddPoints(AffinePoint p1, AffinePoint p2)
    {
        if (p1.IsAtInfinity) return p2;
        if (p2.IsAtInfinity) return p1;

        if (p1.X == p2.X)
        {
            if (p1.Y == p2.Y)
                return DoubleAffinePoint(p1);

            return AffinePoint.AtInfinity;
        }

        return AddAffinePoints(p1, p2);
    }

    private static AffinePoint AddAffinePoints(AffinePoint p1, AffinePoint p2)
    {
        BigInteger lambda = (p2.Y - p1.Y) * MathUtilities.ModInverse(p2.X - p1.X, Curve.P) % Curve.P;
        BigInteger x3 = (lambda * lambda - p1.X - p2.X) % Curve.P;
        BigInteger y3 = (lambda * (p1.X - x3) - p1.Y) % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }

    public AffinePoint DoublePoint(AffinePoint p)
    {
        if (p.IsAtInfinity) return p;

        return DoubleAffinePoint(p);
    }

    private static AffinePoint DoubleAffinePoint(AffinePoint point)
    {
        BigInteger lambda = (3 * point.X * point.X + Curve.A) * MathUtilities.ModInverse(2 * point.Y, Curve.P) % Curve.P;
        BigInteger x3 = (lambda * lambda - 2 * point.X) % Curve.P;
        BigInteger y3 = (lambda * (point.X - x3) - point.Y) % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }

    public AffinePoint MultiplyPoint(BigInteger k, AffinePoint p)
    {
        if (k == BigInteger.Zero) return AffinePoint.AtInfinity;

        p.EnsureOnCurve();

        var result = algorithmType switch
        {
            AlgorithmType.AffineLeftToRight => MultiplyPointLeftToRight(k, p),
            AlgorithmType.AffineMontgomeryLadder => MultiplyPointMontgomeryLadder(k, p),
            _ => throw new InvalidOperationException("Unsupported algorithm type.")
        };

        return result;
    }

    private AffinePoint MultiplyPointLeftToRight(BigInteger k, AffinePoint p)
    {
        AffinePoint result = AffinePoint.AtInfinity;

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            result = DoublePoint(result);

            if (MathUtilities.IsBitSet(k, i))
            {
                result = AddPoints(result, p);
            }
        }

        return result;
    }

    private AffinePoint MultiplyPointMontgomeryLadder(BigInteger k, AffinePoint p)
    {
        var r0 = AffinePoint.AtInfinity;
        var r1 = p;

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            if (MathUtilities.IsBitSet(k, i))
            {
                r0 = AddPoints(r0, r1);
                r1 = DoublePoint(r1);
            }
            else
            {
                r1 = AddPoints(r0, r1);
                r0 = DoublePoint(r0);
            }
        }

        return r0;
    }
}