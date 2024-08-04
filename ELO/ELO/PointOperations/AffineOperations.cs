using ELO.ECDH;
using ELO.Points;
using ELO.StandardMath;

namespace ELO.PointOperations;

public class AffineOperations(AlgorithmType algorithmType) : IPointOperations
{
    public static AffinePoint AddPoints(AffinePoint point1, AffinePoint point2)
    {
        if (point1.IsAtInfinity) return point2;
        if (point2.IsAtInfinity) return point1;

        if (point1.X == point2.X)
        {
            if (point1.Y == point2.Y)
                return DoubleAffinePoint(point1);

            return AffinePoint.AtInfinity;
        }

        return AddAffinePoints(point1, point2);
    }

    private static AffinePoint AddAffinePoints(AffinePoint point1, AffinePoint point2)
    {
        var lambda = ((point2.Y - point1.Y) * MathUtilities.ModInverse(point2.X - point1.X, Curve.P)).MixedModulo();
        var x3 = (lambda * lambda - point1.X - point2.X).MixedModulo();
        var y3 = (lambda * (point1.X - x3) - point1.Y).MixedModulo();

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }

    public static AffinePoint DoublePoint(AffinePoint point)
    {
        if (point.IsAtInfinity) return point;

        return DoubleAffinePoint(point);
    }

    private static AffinePoint DoubleAffinePoint(AffinePoint point)
    {
        var lambda = ((3 * point.X * point.X + Curve.A) * MathUtilities.ModInverse(2 * point.Y, Curve.P)).MixedModulo();
        var x3 = (lambda * lambda - 2 * point.X).MixedModulo();
        var y3 = (lambda * (point.X - x3) - point.Y).MixedModulo();

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;

        return new AffinePoint(x3, y3);
    }

    public AffinePoint MultiplyPoint(BigInteger k, AffinePoint point)
    {
        if (k < 0) throw new ArgumentException( ExceptionMessages.ValueCanNotBeNegative);
        point.EnsureOnCurve();

        if (k == BigInteger.Zero) return AffinePoint.AtInfinity;
        

        var result = algorithmType switch
        {
            AlgorithmType.AffineLeftToRight => MultiplyPointLeftToRight(k, point),
            AlgorithmType.AffineMontgomeryLadder => MultiplyPointMontgomeryLadder(k, point),
            AlgorithmType.AffineWithNAF => MultiplyPointWithNAF(k, point),
            AlgorithmType.AffineWindowedMethod => MultiplyPointWindowedMethod(k, point),

            _ => throw new InvalidOperationException(ExceptionMessages.UnsupportedAlgorithmType)
        };

        return result;
    }

    private static AffinePoint MultiplyPointLeftToRight(BigInteger k, AffinePoint point)
    {
        AffinePoint result = AffinePoint.AtInfinity;

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            result = DoublePoint(result);

            if (MathUtilities.IsBitSet(k, i))
            {
                result = AddPoints(result, point);
            }
        }

        return result;
    }

    private static AffinePoint MultiplyPointMontgomeryLadder(BigInteger k, AffinePoint point)
    {
        var r0 = AffinePoint.AtInfinity;
        var r1 = point;

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

    private static AffinePoint MultiplyPointWithNAF(BigInteger k, AffinePoint point)
    {
        var naf = MathUtilities.ComputeNAF(k);

        var result = AffinePoint.AtInfinity;

        for (int i = naf.Count - 1; i >= 0; i--)
        {
            result = DoublePoint(result);

            if (naf[i] == -1) result = AddPoints(result, point.Negated);
            else if (naf[i] == 1) result = AddPoints(result, point);
        }

        return result;
    }

    private static AffinePoint MultiplyPointWindowedMethod(BigInteger k, AffinePoint point)
    {
        var savedPoints = PrecomputePoints(point);
        var naf = MathUtilities.GenerateWidthWNAF(k);

        var result = AffinePoint.AtInfinity;

        foreach (var i in naf)
        {
            result = DoublePoint(result);

            if (i != 0)
            {
                result = AddPoints(result, savedPoints[i]);
            }
        }

        return result;
    }

    private static Dictionary<int, AffinePoint> PrecomputePoints(AffinePoint point)
    {
        var p2 = DoubleAffinePoint(point);

        Dictionary<int, AffinePoint> points = new() { [1] = point };

        points[3] = AddAffinePoints(points[1], p2);
        points[5] = AddAffinePoints(points[3], p2);
        points[7] = AddAffinePoints(points[5], p2);

        points[-1] = points[1].Negated;
        points[-3] = points[3].Negated;
        points[-5] = points[5].Negated;
        points[-7] = points[7].Negated;

        return points;
    }
}