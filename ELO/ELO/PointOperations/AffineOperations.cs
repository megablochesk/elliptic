using ELO.ECDH;
using ELO.Points;
using System.Linq;

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

    private static AffinePoint PointDoubleRepeat(AffinePoint point, int times)
    {
        AffinePoint result = point;

        for (int i = 0; i < times; i++)
        {
            result = DoubleAffinePoint(result);
        }
        return result;
    }

    public AffinePoint MultiplyPoint(BigInteger k, AffinePoint p)
    {
        if (k == BigInteger.Zero) return AffinePoint.AtInfinity;

        p.EnsureOnCurve();

        var result = algorithmType switch
        {
            AlgorithmType.AffineLeftToRight => MultiplyPointLeftToRight(k, p),
            AlgorithmType.AffineMontgomeryLadder => MultiplyPointMontgomeryLadder(k, p),
            AlgorithmType.AffineWithNAF => MultiplyPointWithNAF(k, p),
            AlgorithmType.AffineWindowedMethod => MultiplyPointWindowedMethod(k, p),
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

    private AffinePoint MultiplyPointWithNAF(BigInteger k, AffinePoint p)
    {
        var naf = MathUtilities.ComputeNAF(k);

        var result = AffinePoint.AtInfinity;

        for (int i = naf.Count - 1; i >= 0; i--)
        {
            result = DoublePoint(result);

            if (naf[i] == -1) result = AddPoints(result, p.Negated);
            else if (naf[i] == 1) result = AddPoints(result, p);
        }

        return result;
    }

    private AffinePoint MultiplyPointWindowedMethod(BigInteger k, AffinePoint p)
    {
        var precomputedPoints = PrecomputePoints(p);
        var naf = MathUtilities.GenerateWidthWNAF(k);

        int c = MathUtilities.FindLargestNonZeroDigit(naf);

        AffinePoint Q = precomputedPoints[Math.Abs(naf[c])];
        if (naf[c] < 0)
        {
            Q = Q.Negated;
        }

        for (int i = c - 1; i >= 0; i--)
        {
            Q = DoublePoint(Q);
            if (naf[i] != 0)
            {
                AffinePoint T = precomputedPoints[Math.Abs(naf[i])];
                if (naf[i] < 0)
                {
                    T = T.Negated;
                }
                Q = AddPoints(Q, T);
            }
        }

        return Q;
    }
    
    private AffinePoint MultiplyPointFixedWindowedMethod(BigInteger k, AffinePoint p)
    {
        var precomputedPoints = PrecomputePoints(p);

        var naf = MathUtilities.ComputeNAF(k);
        var segments = SplitNAFIntoSegments(naf);

        int I = ((1 << (Curve.WindowSize + 1)) - 2) / 3;

        var A = AffinePoint.AtInfinity;
        var B = AffinePoint.AtInfinity;

        for (int j = I; j >= 1; j--)
        {
            foreach (var segment in segments)
                foreach (var ki in segment)
                {
                    if (ki == j)
                    {
                        B = B == AffinePoint.AtInfinity ? precomputedPoints[j] : AddAffinePoints(B, precomputedPoints[j]);
                    }
                    else if (ki == -j)
                    {
                        B = B == AffinePoint.AtInfinity ? precomputedPoints[j].Negated : AddAffinePoints(B, precomputedPoints[j].Negated);
                    }
                }

            A = AddAffinePoints(A, B);
        }

        return A;
    }

    public static List<int[]> SplitNAFIntoSegments(List<int> naf)
    {
        var l = naf.Count;
        double d = Math.Ceiling((double)l / Curve.WindowSize);

        List<int[]> segments = [];
        for (int i = 0; i < d; i++)
        {
            var segmentLength = Math.Min(Curve.WindowSize, naf.Count - i * Curve.WindowSize);

            var segment = new int[segmentLength];
            naf.CopyTo(i * Curve.WindowSize, segment, 0, segmentLength);

            segments.Add(segment);

            Console.WriteLine(segment);
        }

        return segments;
    }

    private static AffinePoint[] PrecomputePoints(AffinePoint p)
    {
        int neededPointNumber = 1 << Curve.WindowSize;

        AffinePoint[] points = new AffinePoint[neededPointNumber];

        points[0] = AffinePoint.AtInfinity;

        for (int i = 1; i < neededPointNumber; i++)
        {
            points[i] = AddAffinePoints(points[i - 1], p);
        }

        return points;
    }
}