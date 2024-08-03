using ELO.ECDH;
using ELO.Points;

namespace ELO.PointOperations;

public class JacobianOperations(AlgorithmType algorithmType) : IPointOperations
{
    public static JacobianPoint AddPoints(JacobianPoint point1, AffinePoint point2)
    {
        if (point1.IsAtInfinity) return point2.ToJacobian();
        if (point2.IsAtInfinity) return point1;

        return AddMixedPoints(point1, point2);
    }

    public static JacobianPoint AddPoints(JacobianPoint point1, JacobianPoint p2)
    {
        if (point1.IsAtInfinity) return p2;
        if (p2.IsAtInfinity) return point1;

        return AddJacobianPoints(point1, p2);
    }

    private static JacobianPoint AddMixedPoints(JacobianPoint point1, AffinePoint point2)
    {
        var a = point2.X * BigInteger.Pow(point1.Z, 2) % Curve.P;
        var b = point2.Y * BigInteger.Pow(point1.Z, 3) % Curve.P;

        var c = (a - point1.X) % Curve.P;
        var d = (b - point1.Y) % Curve.P;

        var c2 = BigInteger.Pow(c, 2) % Curve.P;
        var c3 = BigInteger.Multiply(c2, c) % Curve.P;

        var x3 = (d * d - (c3 + (point1.X * c2 << 1))) % Curve.P;
        var y3 = (d * (point1.X * c2 - x3) - point1.Y * c3) % Curve.P;
        var z3 = point1.Z * c % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;
        if (z3 < 0) z3 += Curve.P;

        return new JacobianPoint(x3, y3, z3);
    }

    private static JacobianPoint AddJacobianPoints(JacobianPoint point1, JacobianPoint point2)
    {
        BigInteger Z1Z1 = point1.Z * point1.Z % Curve.P;
        BigInteger Z2Z2 = point2.Z * point2.Z % Curve.P;

        BigInteger u1 = point1.X * Z2Z2 % Curve.P;
        BigInteger u2 = point2.X * Z1Z1 % Curve.P;

        BigInteger Y1Z2 = point1.Y * point2.Z % Curve.P;
        BigInteger Y2Z1 = point2.Y * point1.Z % Curve.P;

        BigInteger s1 = Y1Z2 * Z2Z2 % Curve.P;
        BigInteger s2 = Y2Z1 * Z1Z1 % Curve.P;

        if (u1 == u2)
        {
            if (s1 == s2)
                return DoubleJacobianPoint(point1);
            
            return JacobianPoint.AtInfinity;
        }

        BigInteger h = u2 - u1;
        BigInteger I = BigInteger.ModPow(h << 1, 2, Curve.P);
        BigInteger j = h * I % Curve.P;
        BigInteger r = 2 * (s2 - s1) % Curve.P;
        BigInteger v = u1 * I % Curve.P;

        BigInteger x3 = (r * r - j - 2 * v) % Curve.P;
        BigInteger y3 = (r * (v - x3) - 2 * s1 * j) % Curve.P;
        BigInteger z3 = (BigInteger.ModPow(point1.Z + point2.Z,2, Curve.P) - Z1Z1 - Z2Z2) * h % Curve.P;

        if (x3 < 0) x3 += Curve.P;
        if (y3 < 0) y3 += Curve.P;
        if (z3 < 0) z3 += Curve.P;

        return new JacobianPoint(x3, y3, z3);
    }

    public static JacobianPoint DoublePoint(JacobianPoint point)
    {
        if (point.IsAtInfinity) return JacobianPoint.AtInfinity;

        return DoubleJacobianPoint(point);
    }

    private static JacobianPoint DoubleJacobianPoint(JacobianPoint point)
    {
        var a = (point.X << 2) * BigInteger.Pow(point.Y, 2) % Curve.P;
        var b = (BigInteger.Pow(point.Y, 4) << 3) % Curve.P;


        var z2 = BigInteger.Pow(point.Z, 2) % Curve.P;

        var c = 3 * (point.X - z2) * (point.X + z2) % Curve.P;
        var d = ((-a << 1) + c * c) % Curve.P;    // equal to x3

        var y3 = (c * (a - d) - b) % Curve.P;
        var z3 = ((point.Y * point.Z) << 1) % Curve.P;

        return new JacobianPoint(d, y3, z3);
    }

    public AffinePoint MultiplyPoint(BigInteger k, AffinePoint point)
    {
        if (k == BigInteger.Zero) return AffinePoint.AtInfinity;

        point.EnsureOnCurve();

        var result = algorithmType switch
        {
            AlgorithmType.JacobianLeftToRight      => MultiplyPointLeftToRight(k, point),
            AlgorithmType.JacobianMontgomeryLadder => MultiplyPointMontgomeryLadder(k, point),
            AlgorithmType.JacobianWithNAF          => MultiplyPointWithNAF(k, point),
            AlgorithmType.JacobianWindowedMethod   => MultiplyPointWindowedMethod(k, point),

            _ => throw new InvalidOperationException(ExceptionMessages.UnsupportedAlgorithmType)
        };

        return result.ToAffine();
    }

    private static JacobianPoint MultiplyPointLeftToRight(BigInteger k, AffinePoint point)
    {
        JacobianPoint result = JacobianPoint.AtInfinity;

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

    private static JacobianPoint MultiplyPointMontgomeryLadder(BigInteger k, AffinePoint point)
    {
        var r0 = JacobianPoint.AtInfinity;
        var r1 = point.ToJacobian();

        for (int i = MathUtilities.GetHighestBit(k); i >= 0; i--)
        {
            if (MathUtilities.IsBitSet(k, i))
            {
                r0 = AddPoints(r1, r0);
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

    private static JacobianPoint MultiplyPointWithNAF(BigInteger k, AffinePoint point)
    {
        var naf = MathUtilities.ComputeNAF(k);

        var result = JacobianPoint.AtInfinity;

        for (int i = naf.Count - 1; i >= 0; i--)
        {
            result = DoublePoint(result);

            if (naf[i] == -1) result = AddPoints(result, point.Negated);
            else if (naf[i] == 1) result = AddPoints(result, point);
        }

        return result;
    }

    private static JacobianPoint MultiplyPointWindowedMethod(BigInteger k, AffinePoint point)
    {
        var savedPoints = PrecomputePoints(point);
        var naf = MathUtilities.GenerateWidthWNAF(k);

        var result = JacobianPoint.AtInfinity;

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
        var p2 = DoubleJacobianPoint(point.ToJacobian());

        Dictionary<int, AffinePoint> points = new() { [1] = point };

        points[3] = AddMixedPoints(p2, points[1]).ToAffine();
        points[5] = AddMixedPoints(p2, points[3]).ToAffine();
        points[7] = AddMixedPoints(p2, points[5]).ToAffine();

        points[-1] = points[1].Negated;
        points[-3] = points[3].Negated;
        points[-5] = points[5].Negated;
        points[-7] = points[7].Negated;

        return points;
    }
}